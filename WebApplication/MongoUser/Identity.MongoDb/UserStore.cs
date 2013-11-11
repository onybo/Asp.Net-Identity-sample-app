using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoUser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MongoUser.Identity.MongoDb
{
    public class UserStore<TUser> : IUserStore<TUser>, IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>, IUserLoginStore<TUser> where TUser : IIdentityUser
    {
        private bool _disposed;
        private readonly MongoDatabase _database;

        public UserStore(MongoDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("database");

            _database = database;
        }

        #region IUserStore
        public virtual Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var collection = _database.GetCollection<TUser>("users");
            return Task.FromResult(collection.Insert(user));
        }

        public virtual Task DeleteAsync(TUser user)
        {
            var collection = _database.GetCollection<TUser>("users");
            var query = Query<TUser>.EQ(u => u.Id, user.Id);
            return Task.FromResult(collection.Remove(query));
        }

        public virtual Task<TUser> FindByIdAsync(string userId)
        {
            var collection = _database.GetCollection<TUser>("users");
            var query = Query<TUser>.EQ(u => u.Id, userId);
            return Task.FromResult(collection.FindOne(query));
        }

        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            var collection = _database.GetCollection<TUser>("users");
            var query = Query.Matches("UserName", new BsonRegularExpression(new Regex(userName, RegexOptions.IgnoreCase)));
            return Task.FromResult(collection.FindOne(query));
        }

        public virtual Task UpdateAsync(TUser user)
        {
            var collection = _database.GetCollection<TUser>("users");
            collection.Save(user);
            return Task.FromResult<int>(0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {            
            _disposed = true;
        }
        #endregion

        #region IUserPasswordStore
        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<string>(user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult<bool>(user.PasswordHash != null && user.PasswordHash.Length > 0);
        }

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            user.PasswordHash = passwordHash;
            return Task.FromResult<int>(0);
        }

        #endregion

        #region IUserSecurityStampStore
        public virtual Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<string>(user.SecurityStamp);
        }

        public virtual Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return Task.FromResult<int>(0);
        }

        #endregion

        #region IUserLoginStore
        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (login == null)
                throw new ArgumentNullException("login");
            
            user.Logins.Add(login);
            return Task.FromResult<int>(0);
        }

        public virtual Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");

            var collection = _database.GetCollection<TUser>("users");
            var query = Query<TUser>.Where(u => u.Logins.Any(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey));
            return Task.FromResult(collection.FindOne(query));
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<IList<UserLoginInfo>>(new List<UserLoginInfo>(user.Logins));
        }

        public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (login == null)
                throw new ArgumentNullException("login");
            var userLogin = user.Logins.First(l => l.ProviderKey == login.ProviderKey && l.LoginProvider == login.LoginProvider);
            if (userLogin == null)
                throw new InvalidOperationException("login does not exist");
            user.Logins.Remove(userLogin);
            var collection = _database.GetCollection<TUser>("users");
            collection.Save(user);
            return Task.FromResult<int>(0);
        }
        #endregion
    }
}