using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SplendoreMVP.Data;
using SplendoreMVP.Models;
using SplendoreMVP.Repositories;
using SplendoreMVP.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddTransient<IHomeRepository, HomeRepository>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddTransient<CartRepository>();
builder.Services.AddScoped<UserOrderRepository>();
builder.Services.AddScoped<ReportRepository>();
builder.Services.AddTransient<OrderRepository>();
builder.Services.AddTransient<UserProductRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
{ options.User.RequireUniqueEmail = true; } )
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Authorization Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrSupervisor", policy =>
        policy.RequireRole("Admin", "Supervisor"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}"
          ).WithStaticAssets(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//seed data
var seeder = new IdentityDataSeeder();
await seeder.SeedIdentityAsync(app);
await seeder.SeedProductsAsync(app);


app.Run();
