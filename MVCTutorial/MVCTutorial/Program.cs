using Microsoft.EntityFrameworkCore;
using MVCTutorial.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace MVCTutorial
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cultureInfo = new CultureInfo("tr-TR");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ",";
            cultureInfo.NumberFormat.NumberGroupSeparator = ""; // disable thousands separator

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<EmployeeContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDB")));

            var app = builder.Build();

            // Apply localization for model binding
            var supportedCultures = new[] { cultureInfo };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(cultureInfo),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Employees}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
