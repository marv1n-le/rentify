using Microsoft.EntityFrameworkCore;
using MilkyShop.BusinessObjects.ApplicationDbContext;
using MilkyShop.Repositories.Implement;
using MilkyShop.Repositories.Interface;
using MilkyShop.Repositories.Repository;
using MilkyShop.Services.Interface;
using MilkyShop.Services.Service;

namespace MilkyShop.RazorWebApp.DependencyInjection;

public static class ApplicationServiceExtension
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBrandService, BrandService>();
    }
    
    public static IServiceCollection AddGhtkClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("GhtkClient", client =>
        {
            client.BaseAddress = new Uri(configuration["GhtkSettings:BaseUri"]!);
            client.DefaultRequestHeaders.Add("Token", configuration["GhtkSettings:ApiToken"]);
            client.Timeout = TimeSpan.FromSeconds(15);
        });
        return services;
    }
    
    public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        return services;
    }
    
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<MilkyShopDbContext>(options =>
            options.UseNpgsql(connectionString)
        );
        return services;
    }
    
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories();
        services.AddServices();
    }
}