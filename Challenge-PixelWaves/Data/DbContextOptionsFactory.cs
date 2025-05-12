using Challenge_PixelWaves.Services;
using Challenge_PixelWaves.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public static class DbContextOptionsFactory
{
    public static DbContextOptions<ApplicationDbContext> Create(IConfigurationService configurationService = null)
    {
        configurationService ??= new ConfigurationService();
        string rawDbPath = configurationService.GetDbPath();
        string dbPath = Environment.ExpandEnvironmentVariables(rawDbPath);
        var connectionString = $"Data Source={dbPath}";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return optionsBuilder.Options;
    }
}

