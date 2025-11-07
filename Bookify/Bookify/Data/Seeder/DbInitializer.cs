using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bookify.Models;

namespace Bookify.Data.Seeder
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(AppDbContext context,
                             UserManager<ApplicationUser> userManager,
                             RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            // Apply pending migrations
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }

            // Seed roles
            if (!_roleManager.Roles.Any())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                _roleManager.CreateAsync(new IdentityRole("User")).Wait();
            }

            // Seed admin user
            if (!_userManager.Users.Any())
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@bookify.com",
                    Email = "admin@bookify.com",
                    FullName = "System Administrator",
                    Address = "HQ",
                    NationalId = "00000000000000",
                    Nationality = "N/A"
                };

                _userManager.CreateAsync(admin, "Admin@123").Wait();
                _userManager.AddToRoleAsync(admin, "Admin").Wait();
            }

            _context.SaveChanges();
        }
    }
}