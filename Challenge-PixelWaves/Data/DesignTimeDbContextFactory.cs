using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Challenge_PixelWaves.Services;
using Challenge_PixelWaves.Services.Interfaces;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = DbContextOptionsFactory.Create();
        return new ApplicationDbContext(options);
    }
}

