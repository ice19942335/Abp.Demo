using System.Threading.Tasks;
using Abp.Demo.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Users;

namespace Abp.Demo.Web.Authentication;

public class StaleAuthenticationMiddleware(
    RequestDelegate next,
    IOptions<DatabaseOptions> databaseOptions)
{
    public async Task InvokeAsync(
        HttpContext context,
        ICurrentUser currentUser,
        IIdentityUserRepository userRepository,
        IdentityDynamicClaimsPrincipalContributorCache claimsCache)
    {
        if (!databaseOptions.Value.IsInMemory)
        {
            await next(context);
            return;
        }

        if (currentUser.IsAuthenticated && currentUser.Id.HasValue)
        {
            var user = await userRepository.FindAsync(currentUser.Id.Value);
            if (user == null)
            {
                await claimsCache.ClearAsync(currentUser.Id.Value, currentUser.TenantId);
                await context.SignOutAsync(IdentityConstants.ApplicationScheme);
            }
        }

        await next(context);
    }
}
