using MiniBlog.Helper;

namespace MiniBlog.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using MiniBlog.Models;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MiniBlog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "MiniBlog.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            string[] roles = new string[] { Const.AdminRole, Const.EditorRole };

            foreach (string role in roles)
            {
                if (!context.Roles.Any(r => r.Name == role))
                {
                    context.Roles.Add(new IdentityRole(role));
                }
            }

            var user = new ApplicationUser
            {
                Email = "admin@miniblog.com",
                UserName = "admin@miniblog.com",
                PhoneNumber = "+111111111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher();
                var hashed = password.HashPassword("Admin@123");
                user.PasswordHash = hashed;

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                userManager.Create(user);
                userManager.AddToRoles(user.Id, roles);
            }

            context.SaveChanges();
        }
    }
}
