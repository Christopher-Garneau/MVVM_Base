using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Challenge_Base.Services;
using Challenge_Base.Services.Interfaces;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IConfigurationService, ConfigurationService>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var configurationService = new ConfigurationService();
            string rawDbPath = configurationService.GetDbPath();
            string dbPath = Environment.ExpandEnvironmentVariables(rawDbPath);
            var connectionString = $"Data Source={dbPath}";
            options.UseSqlite(connectionString);
        });

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<ApplicationDbContext>();
    }
}