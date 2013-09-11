using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserModel
{
    public class CustomUserRole : IUserRole
    {
        [Key, Column(Order = 0)]
        public string RoleId
        {
            get;
            set;
        }

        [Key, Column(Order = 1)]
        public string UserId
        {
            get;
            set;
        }
    }
}
