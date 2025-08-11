using System.Text.Json;
using MilkyShop.Repositories.Implement;
using MilkyShop.Repositories.Infrastructure;

namespace MilkyShop.RazorWebApp.Middleware;

public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
        {
            try
            {
                await _next(context);
            }
            catch (CoreException ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = ex.StatusCode;
                var result = JsonSerializer.Serialize(new { ex.Code, ex.Message });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
            catch (ErrorException ex)
            {
                _logger.LogError(ex, ex.ErrorDetail.ErrorMessage.ToString());
                context.Response.StatusCode = ex.StatusCode;
                var result = JsonSerializer.Serialize(ex.ErrorDetail);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new { error = $"An unexpected error occurred. Detail: {ex.Message}" });
                await context.Response.WriteAsync(result);
            }
        }
    }