using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace CustomUser.Models.Configurations
{
    public class IdentityUserClaimConfiguration : EntityTypeConfiguration<IdentityUserClaim>
    {
        public IdentityUserClaimConfiguration()
        {
            HasRequired(u => u.User);
        }
    }
}

