using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;
using Rentify.Repositories.Repository;
using Rentify.Services.ExternalService.CloudinaryService;
using Rentify.Services.ExternalService.MailGun;
using Rentify.Services.ExternalService.Redis;
using Rentify.Services.Interface;
using Rentify.Services.Mapper;
using Rentify.Services.Service;
using StackExchange.Redis;

namespace Rentify.RazorWebApp.DependencyInjection;

public static class ApplicationServiceExtension
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IRentalItemRepository, RentalItemRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IInquiryRepository, InquiryRepository>();
        services.AddScoped<IOtpRepository, OtpRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IRentalService, RentalService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IInquiryService, InquiryService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
        services.AddScoped<IOtpService, OtpService>();
    }

    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["RedisSettings:ConnectionString"];
        });
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(configuration["RedisSettings:ConnectionString"]!)
        );
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
        services.AddDbContext<RentifyDbContext>(options =>
            options.UseNpgsql(connectionString)
        );
        return services;
    }

    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(
            cfg => { },
            typeof(MapperEntities).Assembly
        );
        services.AddRepositories();
        services.AddServices();
    }
}