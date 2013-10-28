using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace CustomUser.Models.Configurations
{
    public class IdentityRoleConfiguration : EntityTypeConfiguration<IdentityRole>
    {
        public IdentityRoleConfiguration()
        {
            Property(r => r.Name).IsRequired();
        }
    }
}