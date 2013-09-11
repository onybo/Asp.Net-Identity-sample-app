using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserModel
{
    public class CustomUserManagement : IUserManagement
    {
        public bool DisableSignIn
        {
            get;
            set;
        }

        public DateTime LastSignInTimeUtc
        {
            get;
            set;
        }

        [Key]
        public string UserId
        {
            get;
            set;        
        }

        public CustomUserManagement()
		{
			LastSignInTimeUtc = DateTime.UtcNow;
		}
    }
}
