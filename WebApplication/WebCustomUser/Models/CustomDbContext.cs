using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UserModel;

namespace WebCustomUser.Models
{
    public class CustomDbContext : IdentityDbContext<CustomUser>
    {
        public CustomDbContext()
            : base("DefaultConnection")
        {
        }
    }
}