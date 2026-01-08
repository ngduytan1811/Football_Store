using FBS.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace FBS.Infrastructure.DataAccess.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = { "Admin", "Quanlysanpham", "Quanlydonhang", "Baiviet", "Lienhe" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new Role { Name = role });
            }

          
            await CreateAdminUser(
                userManager,
                "Admin", 
                "admin@gmail.com",
                "Admin@123", 
                "Admin", 
                Array.Empty<string>());
            await CreateAdminUser(
                userManager,
                "Lienhe",
                "lienhe@gmail.com",
                "Lienhe@123",
                "Lienhe",
                new[] {
                    "Customer.Create",
                    "Customer.Edit",
                     "Customer.Update",
                    "Customer.Delete",
                    "Contact.Manage",
                    "Contact.Delete",
                    "Revenue.View" });

            await CreateAdminUser(
                userManager, 
                "Quanlysanpham",
                "quanlysanpham@gmail.com",
                "Sanpham@123",
                "Quanlysanpham",
                new[] {
                    "Product.Create",
                    "Product.Edit",
                    "Product.Delete",
                    "Category.Create",
                    "Category.Edit", 
                    "Category.Delete",
                    "Revenue.View" });
            await CreateAdminUser(
                userManager,
                "Quanlydonhang",
                "quanlydonhang@gmail.com",
                "Donhang@123", 
                "Quanlydonhang",
                new[] { 
                    "Order.View",
                    "Order.Update",
                    "Revenue.View" });
            await CreateAdminUser(
                userManager, 
                "Baiviet", 
                "baiviet@gmail.com",
                "Baiviet@123",
                "Baiviet",
                new[] { 
                    "Blog.Create",
                    "Blog.Edit", 
                    "Blog.Delete",
                    "Review.Manage", 
                    "Revenue.View" });
        }

        private static async Task CreateAdminUser(
            UserManager<User> userManager,
            string username,
            string email,
            string password,
            string role,
            string[] claims)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = new User
                {
                    UserName = username,
                    Email = email.ToLowerInvariant(),
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    throw new Exception($"Tạo user {username} thất bại: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);

            var existingClaims = await userManager.GetClaimsAsync(user);
            foreach (var claim in claims)
            {
                if (!existingClaims.Any(c => c.Type == "Permission" && c.Value == claim))
                    await userManager.AddClaimAsync(user, new Claim("Permission", claim));
            }
        }
    }
}
