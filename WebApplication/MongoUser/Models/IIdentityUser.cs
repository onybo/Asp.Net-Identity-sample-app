using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoUser.Models
{
    public interface IIdentityUser : IUser
    {
        string PasswordHash { get; set; }
        string SecurityStamp { get; set; }
        IList<UserLoginInfo> Logins {get;}
    }
}
