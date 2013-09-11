using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserModel
{
    public class CustomUserLogin : IUserLogin
    {
        [Key, Column(Order = 0)]
        public string LoginProvider
        {
            get;
            set;
        }

        [Key, Column(Order = 1)]
        public string ProviderKey
        {
            get;
            set;
        }

        public string UserId
        {
            get;
            set;
        }
    }
}
