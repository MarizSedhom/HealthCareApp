using HealthCareApp.Data;
using HealthCareApp.RepositoryServices;
//using Mapster;
//using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthCareApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs")));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true) || // -> RequireConfirmedAccount = true means that users must confirm their account (e.g., via email) before they can sign in. This is a common setting for applications that require email verification.
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddScoped(typeof(IGenericRepoServices<>), typeof(GenericRepo<>)); // -> typeof() allows you to register open generic types by providing a Type object that represents the generic type definition, and ASP.NET Core DI resolves the specific type at runtime.

            //builder.Services.AddScoped(typeof(IGenericService<,>), typeof(IGenericService<>));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            // -> This method is used to configure ASP.NET Core Identity to use Entity Framework Core (EF Core) as the underlying data store for managing user-related data (like users, roles, etc.).

            // -> Register Mapster TypeAdapterConfig
            //var config = new TypeAdapterConfig();
            //builder.Services.AddSingleton(config);

            // -> Mapster Configuration
            //MapsterConfig.RegisterMappings();

            //builder.Services.AddSingleton<IMapper, ServiceMapper>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                // -> This endpoint is useful for executing database migrations directly from the browser without using the command line (dotnet ef database update).
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                // -> HSTS forces browsers to only use HTTPS when communicating with your site.
            }
            app.UseHttpsRedirection(); // -> When a user accesses your site over HTTP(non-secure), this middleware will redirect the request to the same URL using HTTPS (secure connection).
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapStaticAssets(); // -> It maps and serves static files (like images, CSS, JavaScript, etc.) from a specific location in the web application.
            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
            app.MapRazorPages()
            .WithStaticAssets();

            app.Run();
        }
    }
}
