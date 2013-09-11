using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserModel
{
    public class CustomUserSecret : IUserSecret
    {
        public string Secret
        {
            get;
            set;
        }

        [Key]
        public string UserName
        {
            get;
            set;
        }

        public CustomUserSecret()
        {
        }

        public CustomUserSecret(string userName, string secret)
        {
            UserName = userName;
            Secret = secret;
        }
    }
}
