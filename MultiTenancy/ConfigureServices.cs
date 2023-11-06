using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;
using MultiTenancy.Setting;

namespace MultiTenancy
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddTenancy(this IServiceCollection services,
            ConfigurationManager configuration)
        {
            services.AddScoped<ITenantServices, TenantServices>();
            services.Configure<TenantSetting>(configuration.GetSection(nameof(TenantSetting)));

            TenantSetting options = new();
            configuration.GetSection(nameof(TenantSetting)).Bind(options);

            var defaultDbProvider = options.Defaults.DBProvider;

            if (defaultDbProvider.ToLower() == "mssql")
            {
                services.AddDbContext<ApplicationDbContext>(m => m.UseSqlServer());
            }

            foreach (var tenant in options.Tenant)
            {
                var connectionString = tenant.ConnectionString ?? options.Defaults.ConnectionString;

                using var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.Database.SetConnectionString(connectionString);

                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    dbContext.Database.Migrate();
                }
            }

            return services;
        }
    }
}