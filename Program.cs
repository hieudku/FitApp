using FitApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FitApp.Models;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Authentication and Authorization setup
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Database Context Configuration
builder.Services.AddDbContext<FitAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FitAppContext") ?? throw new InvalidOperationException("Connection string 'FitAppContext' not found.")));

// Add Identity with default options (RequireConfirmedAccount set to true for now)
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()  // Add support for Roles
    .AddEntityFrameworkStores<FitAppContext>();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();

// Seed admin role and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        // Initialize predefined workout templates (if any)
        WorkoutTemplates.Initialize(services);

        // Seed Admin role and optionally an Admin user
        await CreateRoles(services, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the admin role and user.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

// Method to create roles and seed an admin user
async Task CreateRoles(IServiceProvider serviceProvider, ILogger logger)
{
    // Fetch RoleManager and UserManager services
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Check if the "Admin" role exists, create if not
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        logger.LogInformation("Creating Admin role...");
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Optionally: Create an Admin user if not present
    var adminEmail = "admin@fitapp.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        logger.LogInformation("Creating Admin user...");

        var admin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createAdminResult = await userManager.CreateAsync(admin, "Admin123!");

        if (createAdminResult.Succeeded)
        {
            logger.LogInformation("Admin user created successfully. Assigning Admin role...");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        else
        {
            logger.LogError("Failed to create Admin user: {Errors}", string.Join(", ", createAdminResult.Errors.Select(e => e.Description)));
        }
    }
    else
    {
        logger.LogInformation("Admin user already exists.");
    }
}
