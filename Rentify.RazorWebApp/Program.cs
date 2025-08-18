using Microsoft.AspNetCore.Authentication.Cookies;
using Rentify.RazorWebApp.DependencyInjection;
using Rentify.RazorWebApp.Hubs;
using Rentify.RazorWebApp.Pages.ChatPages;
using Rentify.Repositories.Helper;

namespace Rentify.RazorWebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddScoped<ChatModel>();
        builder.Services.AddDatabase(builder.Configuration);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Services.AddHttpClientServices();
        builder.Services.AddSignalR();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Forbidden";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });

        builder.Services.Configure<CloudinarySettings>(
            builder.Configuration.GetSection("CloudinarySettings"));
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

        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.MapHub<ChatHub>("/chatHub");
        app.Run();
    }
}