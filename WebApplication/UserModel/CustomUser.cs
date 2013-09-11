using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserModel
{
    public class CustomUser : IUser
    {
        [Key]
        public string Id
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public virtual ICollection<CustomToken> Roles
        {
            get;
            set;
        }

        public virtual ICollection<CustomUserLogin> Logins
        {
            get;
            set;
        }

        public CustomUser()
            : this("", "")
        {
        }

        public CustomUser(string userName, string email)
        {
            UserName = userName;
            Email = email;
            Id = Guid.NewGuid().ToString();
        }
    }
}
