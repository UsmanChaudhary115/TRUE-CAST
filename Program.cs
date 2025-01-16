using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OVS.Models;

namespace OVS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add DbContext for Entity Framework Core
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Identity services
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddRazorPages();

            // Configure Identity options
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // Enforce HTTPS in production.
            }

            app.UseHttpsRedirection(); // Redirect HTTP to HTTPS.
            app.UseStaticFiles(); // Serve static files like CSS, JS, images.

            app.UseRouting(); // Enable endpoint routing.

            // Middleware for authentication and authorization.
            app.UseAuthentication(); // Ensure this is added if using authentication.
            app.UseAuthorization();

            // Configure the default route for the application.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=UserLogInInterface}/{id?}");

            // Run the application.
            app.Run();
        }
    }
}
