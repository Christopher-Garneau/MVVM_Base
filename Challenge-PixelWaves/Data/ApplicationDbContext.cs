using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Configuration;
using Challenge_PixelWaves.Services.Interfaces;
using Challenge_PixelWaves.Services;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
         : base(options)
    {
    }


    public void SeedData()
    {
    }
}

