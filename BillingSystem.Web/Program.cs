using BillingSystem.Data;
using BillingSystem.Models.Entities;
using BillingSystem.Handlers.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC
builder.Services.AddControllersWithViews();

// 2. Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// 3. Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// 4. MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(CreateUserCommand).Assembly));

// 5. Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// 6. Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Seed Roles and First Admin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<User>>();

    // Create Roles
    string[] roles = { "Admin", "Agent" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(
                new IdentityRole(role));
    }

    // Create First Admin
    string adminEmail = "admin@billing.com";
    string adminPassword = "admin123";

    var adminExists = await userManager
        .FindByEmailAsync(adminEmail);

    if (adminExists == null)
    {
        var now = DateTime.Now;

        var admin = new User
        {
            FullName = "Super Admin",
            Email = adminEmail,
            UserName = adminEmail,
            EmailConfirmed = true,
            Role = BillingSystem.Models.Enums
                .UserRole.Admin,
            CreatedBy = "System",
            CreatedDate = now
        };

        var result = await userManager
            .CreateAsync(admin, adminPassword);

        if (result.Succeeded)
        {
            await userManager
                .AddToRoleAsync(admin, "Admin");
        }
    }
}

app.Run();