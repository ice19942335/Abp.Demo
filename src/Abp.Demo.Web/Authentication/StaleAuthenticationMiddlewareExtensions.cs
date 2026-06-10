using Microsoft.AspNetCore.Builder;

namespace Abp.Demo.Web.Authentication;

public static class StaleAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseStaleAuthenticationCleanup(this IApplicationBuilder app)
    {
        return app.UseMiddleware<StaleAuthenticationMiddleware>();
    }
}
