using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SplendoreMVP.Models;
using SplendoreMVP.Services;
using System.Net.Http;

namespace SplendoreMVP.Data
{
    public class IdentityDataSeeder
    {
        private readonly IFileService _fileService;

        public IdentityDataSeeder(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task SeedIdentityAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Roles
            string[] roles = { "Admin", "Customer", "Supervisor" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin user
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
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        public async Task SeedProductsAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();

            if (!context.Categories.Any())
            {
                var faker = new Faker();
                using var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd("SplendoreMVP-Seeder/1.0");
                string placeholderBase = "https://placehold.co";
                var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg" };

                // Create 10 categories
                var categories = new List<Category>();
                for (int i = 0; i < 10; i++)
                {
                    var category = new Category
                    {
                        Name = faker.Commerce.Categories(1)[0],
                        Description = faker.Commerce.ProductDescription()
                    };
                    categories.Add(category);
                }

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();

                foreach (var category in categories)
                {
                    for (int i = 0; i < 13; i++)
                    {
                        var productName = faker.Commerce.ProductName();
                        var product = new Product
                        {
                            Name = productName,
                            Description = faker.Commerce.ProductDescription(),
                            Price = decimal.Parse(faker.Commerce.Price(10, 1000)),
                            StockQuantity = faker.Random.Int(10, 200),
                            CategoryID = category.Id
                        };

                        try
                        {
                            // Try explicit PNG endpoint first
                            string requestUrl = $"{placeholderBase}/300x300.png?text={Uri.EscapeDataString(productName)}";
                            HttpResponseMessage response = null;

                            try
                            {
                                response = await http.GetAsync(requestUrl);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Seeder] HTTP error requesting PNG for '{productName}': {ex.Message}");
                            }

                            // Fallback to generic endpoint if PNG failed or returned non-success
                            if (response == null || !response.IsSuccessStatusCode)
                            {
                                requestUrl = $"{placeholderBase}/300x300?text={Uri.EscapeDataString(productName)}";
                                try
                                {
                                    response = await http.GetAsync(requestUrl);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"[Seeder] HTTP error requesting fallback image for '{productName}': {ex.Message}");
                                    response = null;
                                }
                            }

                            if (response == null || !response.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"[Seeder] Image request failed for '{productName}'. Skipping image.");
                                // add product without image (optional)
                                context.Products.Add(product);
                                continue;
                            }

                            var mediaType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
                            if (!mediaType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine($"[Seeder] Expected image/* but got '{mediaType}' for '{productName}'. Skipping image.");
                                context.Products.Add(product);
                                continue;
                            }

                            var imageBytes = await response.Content.ReadAsByteArrayAsync();
                            if (imageBytes == null || imageBytes.Length == 0)
                            {
                                Console.WriteLine($"[Seeder] Empty image bytes for '{productName}'. Skipping image.");
                                context.Products.Add(product);
                                continue;
                            }

                            // Determine extension from media type
                            string extension = mediaType switch
                            {
                                "image/png" => ".png",
                                "image/jpeg" => ".jpg",
                                "image/jpg" => ".jpg",
                                "image/svg+xml" => ".svg",
                                _ => ".png"
                            };

                            // Create IFormFile from bytes with correct extension and reset stream position
                            var ms = new MemoryStream(imageBytes);
                            ms.Position = 0;
                            var generatedFileName = $"{Guid.NewGuid()}{extension}";
                            IFormFile formFile = new FormFile(ms, 0, ms.Length, "file", generatedFileName)
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = mediaType
                            };

                            // Save using your FileService (allow .svg as well)
                            product.ImageUrl = await _fileService.SaveFile(formFile, allowedExtensions);

                            context.Products.Add(product);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Seeder] Error saving image for '{productName}': {ex.Message}");
                            // still add product without image to avoid losing data seeding
                            context.Products.Add(product);
                        }
                    }

                    // Optionally save per category to reduce memory usage
                    await context.SaveChangesAsync();
                }

                // Final save (redundant if saved per category, but safe)
                await context.SaveChangesAsync();
            }
        }

    }
}
