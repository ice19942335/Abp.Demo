using System.Collections.Generic;
using Abp.Demo.Database;
using Abp.Demo.Demo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Volo.Abp.Account.Web;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;

namespace Abp.Demo.Web.Pages.Account;

public class LoginModel(
    IAuthenticationSchemeProvider schemeProvider,
    IOptions<AbpAccountOptions> accountOptions,
    IOptions<Microsoft.AspNetCore.Identity.IdentityOptions> identityOptions,
    IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
    IWebHostEnvironment webHostEnvironment,
    IOptions<DatabaseOptions> databaseOptions)
    : Volo.Abp.Account.Web.Pages.Account.LoginModel(
        schemeProvider,
        accountOptions,
        identityOptions,
        identityDynamicClaimsPrincipalContributorCache,
        webHostEnvironment)
{
    public bool ShowDemoUserPicker => databaseOptions.Value.IsInMemory;

    public IReadOnlyList<DemoUserDefinition> DemoUsers => DemoUserDefinitions.All;
}
