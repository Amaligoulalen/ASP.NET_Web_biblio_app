using System;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using gestion_biblio.Data;
using gestion_biblio.Models;

var builder = WebApplication.CreateBuilder(args);

// Web root setup


// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity with VERY relaxed password rules
builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();


// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("MemberOnly", policy => policy.RequireRole("Member"));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
    options.Conventions.AuthorizeFolder("/User", "MemberOnly");
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// 🔧 SEEDING - Runs automatically on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();


        // Apply migrations
        await db.Database.MigrateAsync();
        Console.WriteLine("✅ Database migrations applied");

        // Create roles
        var roles = new[] { "Admin", "Member" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                Console.WriteLine($"✅ Role '{roleName}' created");
            }
        }

        // Seed admins
        var admins = await db.Admins.Where(a => a.IsActive).ToListAsync();
        Console.WriteLine($"\n📊 Found {admins.Count} active admin(s)");

        if (admins.Count == 0)
        {
            var fallbackAdminEmail = "admin@local.test";
            var existingFallback = await userManager.FindByEmailAsync(fallbackAdminEmail);
            if (existingFallback == null)
            {
                var identityUser = new ApplicationUser
                {
                    UserName = fallbackAdminEmail,
                    Email = fallbackAdminEmail,
                    FullName = "Default Admin",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(identityUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(identityUser, "Admin");
                    Console.WriteLine($"✅ Fallback admin created: {fallbackAdminEmail} / Admin123!");
                }
            }
        }

        foreach (var admin in admins)
        {
            var existingUser = await userManager.FindByEmailAsync(admin.Email);
            
            if (existingUser == null)
            {
                 Console.WriteLine($"🔐 Creating admin identity: {admin.Email} | {admin.Password}");
                var identityUser = new ApplicationUser 
                { 
                    UserName = admin.Email, 
                    Email = admin.Email,
                    FullName = admin.FullName,
                    EmailConfirmed = true 
                };

                var result = await userManager.CreateAsync(identityUser, admin.Password);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(identityUser, "Admin");
                    Console.WriteLine($"✅ Admin created: {admin.Email} / {admin.Password}");
                }
                else
                {
                    Console.WriteLine($"❌ Failed: {admin.Email}");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"   - {error.Description}");
                    }
                }
            }
            else
            {
                // Ensure admin role
                if (!await userManager.IsInRoleAsync(existingUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(existingUser, "Admin");
                }
            }
        }

        // Seed members
        var members = await db.Members.ToListAsync();
        Console.WriteLine($"\n📊 Found {members.Count} member(s)");
        
        if (members.Count == 0)
        {
            var fallbackMemberEmail = "member@local.test";
            var existingFallback = await userManager.FindByEmailAsync(fallbackMemberEmail);
            if (existingFallback == null)
            {
                var identityUser = new ApplicationUser
                {
                    UserName = fallbackMemberEmail,
                    Email = fallbackMemberEmail,
                    FullName = "Default Member",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(identityUser, "Member123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(identityUser, "Member");
                    Console.WriteLine($"✅ Fallback member created: {fallbackMemberEmail} / Member123!");
                }
            }
        }

        foreach (var member in members)
        {
            var existingUser = await userManager.FindByEmailAsync(member.Email);
            
            if (existingUser == null)
            {
                var identityUser = new ApplicationUser 
                { 
                    UserName = member.Email, 
                    Email = member.Email,
                    FullName = $"{member.FirstName} {member.LastName}",
                    EmailConfirmed = true 
                };

                var defaultPassword = $"{member.FirstName}1234";
                var result = await userManager.CreateAsync(identityUser, defaultPassword);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(identityUser, "Member");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(existingUser, "Member"))
                {
                    await userManager.AddToRoleAsync(existingUser, "Member");
                }
            }
        }

        Console.WriteLine("\n🎉 Seeding completed! You can now login.\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ SEEDING ERROR: {ex.Message}\n");
    }
}

// HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();

await app.RunAsync();
