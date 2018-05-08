using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Vytals.Models
{
    public class VfDbContext : IdentityDbContext<ApplicationUser, MyRole, int>
    {
        public VfDbContext(DbContextOptions<VfDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("User");

            });
            modelBuilder.Entity<IdentityUserRole<int>>(entity =>
            {
                entity.ToTable("UserRole");
            });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.ToTable("UserLogin");
            });
            modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
            {
                entity.ToTable("UserClaim");
            });
            modelBuilder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.ToTable("UserToken");
            });
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
            {
                entity.ToTable("RoleClaim");
            });
        }

    }

    public class MyRole : IdentityRole<int>
    {
        public MyRole() : base()
        {
            
        }

        public MyRole(string roleName)
        {
            Name = roleName;
        }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<VfDbContext>
    {
        public VfDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Production.json",true)
                .Build();
            var builder = new DbContextOptionsBuilder<VfDbContext>();
            var connectionString = "Server=tcp:beauty-advisor.database.windows.net,1433;Initial Catalog=LocalSkills;Persist Security Info=False;User ID=beautyadvisor;Password=Sofus71204;MultipleActiveResultSets=False;TrustServerCertificate=False;Connection Timeout=30;";
            builder.UseSqlServer(connectionString);
            return new VfDbContext(builder.Options);
        }
    }
}
