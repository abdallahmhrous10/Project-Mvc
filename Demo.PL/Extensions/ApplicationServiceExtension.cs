
using Demo.BLL.Implementation;
using Demo.BLL.Interfaces;
using Demo.BLL.Services;
using Demo.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Demo.PL.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // MVC Services
            services.AddControllersWithViews();

            // Authorization
            services.AddAuthorization();

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "DefaultConnection"));
            });

            // Unit Of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Application Services
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            // AutoMapper
            services.AddAutoMapper(config =>
                config.AddMaps(Assembly.GetEntryAssembly()));

            return services;
        }
    }
}
