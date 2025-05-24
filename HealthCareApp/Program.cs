using HealthCareApp.Account;
using HealthCare.DAL.Repository;
using HealthCare.DAL.Data;
using HealthCare.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Microsoft.AspNetCore.Authentication.Google;
using HealthCare.BLL.Interface.Repository;
using HealthCare.BLL.Interface.Service;
using HealthCare.BLL.Service;
using HealthCare.PL.Middleware;

namespace HealthCareApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication()
             .AddGoogle(options =>
             {
                 options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                 options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
             });

            // Add services to the container.
            // var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("myConn")));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            builder.Services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();

            builder.Services.AddScoped<IFileService, HealthCare.BLL.Service.FileService>();

            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<INotificationObserver, AppNotificationObserver>();

            // builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            //.AddEntityFrameworkStores<ApplicationDbContext>();

            //allowing sending emails
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddDefaultIdentity<ApplicationUser>(
                options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>() // Add this line for roles
                .AddEntityFrameworkStores<ApplicationDbContext>();



            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();

            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await RoleSeeder.SeedRolesAsync(services);

                    await AdminUserSeeder.SeedAdminUserAsync(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding roles.");
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.Use(async (context, next) =>
            //{
            //    await next();

            //    // Check if the response is 403 Forbidden (Access Denied)
            //    if (context.Response.StatusCode == 403)
            //    {
            //        // Example: Redirect to custom Access Denied page
            //        context.Response.Redirect("/Home/AccessDenied");
            //    }
            //});

            app.UseStaticFiles();
            
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Doctor}/{action=GetAllDoctorsInfo}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
