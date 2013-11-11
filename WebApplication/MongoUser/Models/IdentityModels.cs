using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
namespace MongoUser.Models
{
    public class ApplicationUser : IIdentityUser
    {
        public string Id {
            get
            {
                return _Id.ToString();
            }
        }

        [BsonId]
        public ObjectId _Id { get; set; }
        public string UserName { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }

        private List<UserLoginInfo> _logins;
        public virtual IList<UserLoginInfo> Logins
        {
            get
            {
                if (_logins == null)
                {
                    _logins = new List<UserLoginInfo>();
                }
                return _logins;
            }
            set
            {

                _logins = value != null ? new List<UserLoginInfo>(value) : null;
            }
        }
    }
}