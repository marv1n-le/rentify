using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Repositories.Helper;

public static class HttpContextExtensions
{
    public static string? GetCurrentUserId(this IHttpContextAccessor contextAccessor)
    {
        if (contextAccessor.HttpContext == null)
        {
            return null;
        }
        var userId = contextAccessor.HttpContext.Request.Cookies.TryGetValue("userId", out var value) ? value.ToString() : null;
        return userId;
    }
}
