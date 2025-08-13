using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Rentify.BusinessObjects.ApplicationDbContext;

public class DbContextFactory : IDesignTimeDbContextFactory<MilkyShopDbContext>
{
    public MilkyShopDbContext CreateDbContext(string[] args)
    {
        Console.WriteLine("Current Directory: " + Directory.GetCurrentDirectory());

        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Rentify.RazorWebApp"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<MilkyShopDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new MilkyShopDbContext(optionsBuilder.Options);
    }
}