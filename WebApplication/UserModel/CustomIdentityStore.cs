using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserModel
{
    public class CustomIdentityStore : IIdentityStore
    {
        private DbContext _context;

        public IUserLoginStore Logins
        {
            get;
            private set;
        }

        public IRoleStore Roles
        {
            get;
            private set;
        }

        public virtual async Task<IdentityResult> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return IdentityResult.Succeeded();
            }
            catch (DbEntityValidationException ex)
            {
                return IdentityResult.Failed(ex.EntityValidationErrors.SelectMany(ve => ve.ValidationErrors.Select(e => e.ErrorMessage)).ToArray());
            }            
        }

        public IUserSecretStore Secrets
        {
            get;
            private set;
        }

        public ITokenStore Tokens
        {
            get;
            private set;
        }

        public IUserClaimStore UserClaims
        {
            get;
            private set;
        }

        public IUserManagementStore UserManagement
        {
            get;
            private set;
        }

        public IUserStore Users
        {
            get;
            private set;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _context != null)
            {
                _context.Dispose();
            }
            _context = null;
            Logins = null;
            Roles = null;
            Secrets = null;
            Tokens = null;
            UserClaims = null;
            UserManagement = null;
            Users = null;
        }

        public CustomIdentityStore(System.Data.Entity.DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
            Logins = new UserLoginStore<CustomUserLogin>(_context);
            Roles = new RoleStore<CustomRole, CustomUserRole>(_context);
            Secrets = new UserSecretStore<CustomUserSecret>(_context);
            Tokens = new TokenStore<CustomToken>(_context);
            UserClaims = new UserClaimStore<CustomUserClaim>(_context);
            UserManagement = new UserManagementStore<CustomUserManagement>(_context);
            Users = new UserStore<CustomUser>(_context);
        }
    }
}
