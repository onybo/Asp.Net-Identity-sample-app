using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserModel
{
    public class CustomUser : IdentityUser
    {
        public string Email
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
