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

            // Seed roles (Admin, Employee, User)
            if (!_roleManager.Roles.Any())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).Wait(); 
                _roleManager.CreateAsync(new IdentityRole("Employee")).Wait(); 
                _roleManager.CreateAsync(new IdentityRole("User")).Wait(); 
            }

            // Seed admin user
            if (!_userManager.Users.Any(u => u.Email == "admin@bookify.com"))
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@bookify.com",
                    Email = "admin@bookify.com",
                    FullName = "System Administrator",
                    Address = "HQ",
                    NationalId = "00000000000000",
                    Nationality = "N/A",
                    BirthDate = new DateTime(1980, 1, 1) 
                };

                var result = _userManager.CreateAsync(admin, "Admin@123").Result;
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(admin, "Admin").Wait(); 
                }
                else
                {
                    result.Errors.ToList().ForEach(e => 
                    {
                        Console.WriteLine($"Error creating admin user: {e.Description}");
                    });
                }
            }

            _context.SaveChanges();
        }
    }
}