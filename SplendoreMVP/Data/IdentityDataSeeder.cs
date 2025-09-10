using Microsoft.AspNetCore.Identity;
using SplendoreMVP.Models;

namespace SplendoreMVP.Data
{
    public class IdentityDataSeeder
    {
        public async Task SeedIdentityAsync(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                // Define roles
                string[] roles = { "Admin", "Customer", "Supervisor" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole
                        {
                            Name = role
                        });
                    }
                }

                // Seed Admin User
                string adminEmail = "admin@shop.com";
                string adminPassword = "Admin@123";

                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "System",
                        LastName = "Admin",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
        public async Task SeedProductsAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ApplicationDbContext>();

            if (!context.Categories.Any())
            {
                var electronics = new Category
                {
                    Name = "Electronics",
                    Description = "Gadgets, devices, and accessories"
                };

                var clothing = new Category
                {
                    Name = "Clothing",
                    Description = "Apparel for men, women, and children"
                };

                var books = new Category
                {
                    Name = "Books",
                    Description = "Fiction, non-fiction, and educational books"
                };

                context.Categories.AddRange(electronics, clothing, books);
                await context.SaveChangesAsync();

                // Seed products after categories
                var products = new List<Product>
        {
            // Electronics
            new Product { Name = "Smartphone", Description = "Latest Android smartphone", Price = 699.99M, StockQuantity = 50, ImageUrl = "/images/products/smartphone.jpg", CategoryID = electronics.Id },
            new Product { Name = "Laptop", Description = "Powerful laptop for work and gaming", Price = 1299.00M, StockQuantity = 30, ImageUrl = "/images/products/laptop.jpg", CategoryID = electronics.Id },
            new Product { Name = "Wireless Headphones", Description = "Noise-cancelling Bluetooth headphones", Price = 199.99M, StockQuantity = 100, ImageUrl = "/images/products/headphones.jpg", CategoryID = electronics.Id },
            new Product { Name = "Smartwatch", Description = "Fitness tracking smartwatch", Price = 249.99M, StockQuantity = 70, ImageUrl = "/images/products/smartwatch.jpg", CategoryID = electronics.Id },
            new Product { Name = "Tablet", Description = "10-inch Android tablet", Price = 349.99M, StockQuantity = 40, ImageUrl = "/images/products/tablet.jpg", CategoryID = electronics.Id },

            // Clothing
            new Product { Name = "T-Shirt", Description = "100% cotton T-shirt", Price = 19.99M, StockQuantity = 200, ImageUrl = "/images/products/tshirt.jpg", CategoryID = clothing.Id },
            new Product { Name = "Jeans", Description = "Slim fit blue jeans", Price = 49.99M, StockQuantity = 150, ImageUrl = "/images/products/jeans.jpg", CategoryID = clothing.Id },
            new Product { Name = "Jacket", Description = "Warm winter jacket", Price = 89.99M, StockQuantity = 80, ImageUrl = "/images/products/jacket.jpg", CategoryID = clothing.Id },
            new Product { Name = "Sneakers", Description = "Comfortable everyday sneakers", Price = 59.99M, StockQuantity = 120, ImageUrl = "/images/products/sneakers.jpg", CategoryID = clothing.Id },
            new Product { Name = "Dress", Description = "Elegant evening dress", Price = 99.99M, StockQuantity = 60, ImageUrl = "/images/products/dress.jpg", CategoryID = clothing.Id },

            // Books
            new Product { Name = "Novel", Description = "Bestselling fiction novel", Price = 12.99M, StockQuantity = 100, ImageUrl = "/images/products/novel.jpg", CategoryID = books.Id },
            new Product { Name = "Science Book", Description = "Introduction to physics", Price = 29.99M, StockQuantity = 90, ImageUrl = "/images/products/science.jpg", CategoryID = books.Id },
            new Product { Name = "History Book", Description = "World history overview", Price = 24.99M, StockQuantity = 70, ImageUrl = "/images/products/history.jpg", CategoryID = books.Id },
            new Product { Name = "Children's Storybook", Description = "Illustrated bedtime stories", Price = 14.99M, StockQuantity = 150, ImageUrl = "/images/products/children.jpg", CategoryID = books.Id },
            new Product { Name = "Cookbook", Description = "Recipes from around the world", Price = 34.99M, StockQuantity = 60, ImageUrl = "/images/products/cookbook.jpg", CategoryID = books.Id }
        };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
