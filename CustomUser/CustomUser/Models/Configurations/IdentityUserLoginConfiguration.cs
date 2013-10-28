using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace CustomUser.Models.Configurations
{
    public class IdentityUserLoginConfiguration : EntityTypeConfiguration<IdentityUserLogin>
    {
        public IdentityUserLoginConfiguration()
        {
            HasKey(l => new 
            {
                l.UserId,
                l.LoginProvider,
                l.ProviderKey
            });
            HasRequired<IdentityUser>(u => u.User);
        }
    }
}
