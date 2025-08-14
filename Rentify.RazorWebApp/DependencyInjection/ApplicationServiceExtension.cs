using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;
using Rentify.Repositories.Repository;
using Rentify.Services.Interface;
using Rentify.Services.Service;

namespace Rentify.RazorWebApp.DependencyInjection;

public static class ApplicationServiceExtension
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
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