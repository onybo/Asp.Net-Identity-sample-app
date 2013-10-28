using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;

namespace CustomUser.Models
{
    public class ApplicationDbContext : DbContext
    {
        public virtual IDbSet<ApplicationUser> Users { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException("modelBuilder");
            modelBuilder.Configurations.AddFromAssembly(typeof(CustomUser.Models.Configurations.ApplicationUserConfiguration).Assembly);
        }
    }
}