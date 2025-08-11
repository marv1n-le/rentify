using MilkyShop.RazorWebApp.DependencyInjection;
using MilkyShop.Repositories.Helper;

namespace MilkyShop.RazorWebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddDatabase(builder.Configuration);
        builder.Services.AddHttpContextAccessor();
        builder.Services.Configure<SepaySettings>(builder.Configuration.GetSection("SepaySettings"));
        builder.Services.Configure<GhtkSettings>(builder.Configuration.GetSection("GhtkSettings"));
        builder.Services.AddGhtkClient(builder.Configuration);
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Services.AddHttpClientServices();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}