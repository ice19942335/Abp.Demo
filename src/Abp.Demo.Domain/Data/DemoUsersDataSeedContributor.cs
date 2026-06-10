using System;
using System.Threading.Tasks;
using Abp.Demo.Database;
using Abp.Demo.Demo;
using Abp.Demo.Permissions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Abp.Demo.Data;

public class DemoUsersDataSeedContributor(
    IOptions<DatabaseOptions> databaseOptions,
    IGuidGenerator guidGenerator,
    IIdentityRoleRepository roleRepository,
    IIdentityUserRepository userRepository,
    ILookupNormalizer lookupNormalizer,
    IdentityUserManager userManager,
    IdentityRoleManager roleManager,
    IPermissionDataSeeder permissionDataSeeder)
    : IDataSeedContributor, ITransientDependency
{
    private readonly DatabaseOptions databaseOptions = databaseOptions.Value;

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (!databaseOptions.IsInMemory)
        {
            return;
        }

        await CreateRoleWithPermissionsAsync(
            context.TenantId,
            DemoRoles.Manager,
            DemoPermissionNames.Resources.Default,
            DemoPermissionNames.Resources.Create,
            DemoPermissionNames.Resources.Edit,
            DemoPermissionNames.Resources.Delete,
            DemoPermissionNames.Bookings.Default,
            DemoPermissionNames.Bookings.Create,
            DemoPermissionNames.Bookings.Cancel);

        await CreateRoleWithPermissionsAsync(
            context.TenantId,
            DemoRoles.Employee,
            DemoPermissionNames.Resources.Default,
            DemoPermissionNames.Bookings.Default,
            DemoPermissionNames.Bookings.Create);

        await CreateRoleWithPermissionsAsync(
            context.TenantId,
            DemoRoles.Viewer,
            DemoPermissionNames.Resources.Default,
            DemoPermissionNames.Bookings.Default);

        foreach (var demoUser in DemoUserDefinitions.AdditionalUsers)
        {
            await CreateUserIfNotExistsAsync(demoUser);
        }
    }

    private async Task CreateRoleWithPermissionsAsync(
        Guid? tenantId,
        string roleName,
        params string[] permissions)
    {
        var normalizedRoleName = lookupNormalizer.NormalizeName(roleName);
        var role = await roleRepository.FindByNormalizedNameAsync(normalizedRoleName);

        if (role == null)
        {
            role = new IdentityRole(guidGenerator.Create(), roleName, tenantId)
            {
                IsPublic = true
            };
            (await roleManager.CreateAsync(role)).CheckErrors();
        }

        await permissionDataSeeder.SeedAsync(
            RolePermissionValueProvider.ProviderName,
            roleName,
            permissions,
            tenantId);
    }

    private async Task CreateUserIfNotExistsAsync(DemoUserDefinition demoUser)
    {
        var normalizedUserName = lookupNormalizer.NormalizeName(demoUser.UserName);
        if (await userRepository.FindByNormalizedUserNameAsync(normalizedUserName) != null)
        {
            return;
        }

        var user = new IdentityUser(
            demoUser.Id,
            demoUser.UserName,
            demoUser.Email)
        {
            Name = demoUser.UserName
        };
        user.SetEmailConfirmed(true);

        (await userManager.CreateAsync(user, demoUser.Password, validatePassword: false)).CheckErrors();
        (await userManager.AddToRoleAsync(user, demoUser.RoleName)).CheckErrors();
    }
}
