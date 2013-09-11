using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserModel
{
    public class CustomUserClaim : IUserClaim
    {
        [Key]
        public int Id { get; set; }
        public string ClaimType
        {
            get;
            set;
        }

        public string ClaimValue
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
