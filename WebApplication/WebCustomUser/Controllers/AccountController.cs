using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Validation;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using WebCustomUser.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading;

namespace WebApplication.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
            IdentityManager = new IdentityManager(new IdentityStore());
            AuthenticationManager = new AuthenticationManager(new IdentityAuthenticationOptions(), IdentityManager);
        }

        public AccountController(IdentityManager storeManager, AuthenticationManager authManager)
        {
            IdentityManager = storeManager;
            AuthenticationManager = authManager;
        }

        public IdentityManager IdentityManager { get; private set; }
        public AuthenticationManager AuthenticationManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        private Microsoft.Owin.Security.IAuthenticationManager OwinAuthManager
        {
            get
            {
                return HttpContextBaseExtensions.GetOwinContext(HttpContext).Authentication;
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await AuthenticationManager.CheckPasswordAndSignInAsync(OwinAuthManager, model.UserName, model.Password, model.RememberMe);
                // Validate the user password
                if (result.Success)
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    AddModelError(result);
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                // Create a profile, password, and link the local login before signing in the user
                User user = new User(model.UserName);
                var result = await new UserManager(IdentityManager).CreateLocalUserAsync(user, model.Password);
                if (result.Success)
                {
                    var cts = new CancellationTokenSource();
                    var claims = AuthenticationManager.GetUserIdentityClaimsAsync(user.Id, new Claim[0], cts.Token);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddModelError(result, "Failed to register user name: " + model.UserName);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            string userId = User.Identity.GetUserId();
            var result = await new LoginManager(IdentityManager).RemoveLoginAsync(User.Identity.GetUserId(), loginProvider, providerKey);
            if (result.Success)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public async Task<ActionResult> Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = await new LoginManager(IdentityManager).HasLocalLoginAsync(User.Identity.GetUserId());
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            string userId = User.Identity.GetUserId();
            bool hasLocalLogin = await new LoginManager(IdentityManager).HasLocalLoginAsync(userId);
            ViewBag.HasLocalPassword = hasLocalLogin;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalLogin)
            {
                if (ModelState.IsValid)
                {
                    var changePasswordSucceeded = await new PasswordManager(IdentityManager).ChangePasswordAsync(User.Identity.GetUserName(), model.OldPassword, model.NewPassword);
                    if (changePasswordSucceeded.Success)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddModelError(changePasswordSucceeded, "The current password is incorrect or the new password is invalid");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        // Create the local login info and link the local account to the user
                        var result = await new LoginManager(IdentityManager).AddLocalLoginAsync(userId, User.Identity.GetUserName(), model.NewPassword);
                        if (result.Success)
                        {
                            return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                        }
                        else
                        {
                            AddModelError(result, "Failed to set password");
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void AddModelError(IdentityResult result, string message = "")
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
            if (!string.IsNullOrWhiteSpace(message))
                ModelState.AddModelError("", message);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { loginProvider = provider, ReturnUrl = returnUrl }), AuthenticationManager);
        }

        private bool VerifyExternalIdentity(ClaimsIdentity id, string loginProvider)
        {
            if (id == null)
            {
                return false;
            }
            Claim claim = id.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            return claim != null && claim.Issuer == loginProvider;
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string loginProvider, string returnUrl)
        {
            var id = await AuthenticationManager.GetExternalIdentityAsync(OwinAuthManager);
            if (!VerifyExternalIdentity(id, loginProvider))
            {
                return View("ExternalLoginFailure");
            }

            // Sign in this external identity if its already linked
            var result = await AuthenticationManager.SignInExternalIdentityAsync(OwinAuthManager, id);
            if (result.Success)
            {
                return RedirectToLocal(returnUrl);
            }
            else if (User.Identity.IsAuthenticated)
            {
                // Try to link if the user is already signed in
                if ((await AuthenticationManager.LinkExternalIdentityAsync(id, User.Identity.GetUserId())).Success)
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    return View("ExternalLoginFailure");
                }
            }
            else
            {
                // Otherwise prompt to create a local user
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = id.Name, LoginProvider = loginProvider });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                if ((await AuthenticationManager.CreateAndSignInExternalUserAsync(OwinAuthManager, new User(model.UserName))).Success)
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    return View("ExternalLoginFailure");
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            OwinAuthManager.SignOut(new string[0]);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return (ActionResult)PartialView("_ExternalLoginsListPartial", new List<AuthenticationDescription>(
                OwinAuthManager.GetExternalAuthenticationTypes()
                ));
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            return Task.Run(async () =>
            {
                var linkedAccounts = await new LoginManager(IdentityManager).GetLoginsAsync(User.Identity.GetUserId());
                ViewBag.ShowRemoveButton = linkedAccounts.Count() > 1;
                return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
            }).Result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && IdentityManager != null)
            {
                IdentityManager.Dispose();
                IdentityManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUrl, AuthenticationManager manager)
            {
                LoginProvider = provider;
                RedirectUrl = redirectUrl;
                Manager = manager;
            }

            public string LoginProvider { get; set; }
            public string RedirectUrl { get; set; }
            public AuthenticationManager Manager { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                HttpContextBaseExtensions.GetOwinContext(context.HttpContext).Authentication.Challenge(new AuthenticationProperties { RedirectUrl = RedirectUrl }, LoginProvider);
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        #endregion
    }
}