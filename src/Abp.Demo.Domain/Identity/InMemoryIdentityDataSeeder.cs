using System;
using System.Threading.Tasks;
using Abp.Demo.Database;
using Abp.Demo.Demo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Roles;
using Volo.Abp.Uow;

namespace Abp.Demo.Identity;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IIdentityDataSeeder))]
public class InMemoryIdentityDataSeeder(
    IGuidGenerator guidGenerator,
    IIdentityRoleRepository roleRepository,
    IIdentityUserRepository userRepository,
    ILookupNormalizer lookupNormalizer,
    IdentityUserManager userManager,
    IdentityRoleManager roleManager,
    ICurrentTenant currentTenant,
    IOptions<IdentityOptions> identityOptions,
    IOptions<DatabaseOptions> databaseOptions)
    : IdentityDataSeeder(
        guidGenerator,
        roleRepository,
        userRepository,
        lookupNormalizer,
        userManager,
        roleManager,
        currentTenant,
        identityOptions), IIdentityDataSeeder
{
    private readonly DatabaseOptions databaseOptions = databaseOptions.Value;

    [UnitOfWork]
    public override async Task<IdentityDataSeedResult> SeedAsync(
        string adminEmail,
        string adminPassword,
        Guid? tenantId = null,
        string? adminUserName = null)
    {
        if (!databaseOptions.IsInMemory)
        {
            return await base.SeedAsync(adminEmail, adminPassword, tenantId, adminUserName);
        }

        Check.NotNullOrWhiteSpace(adminEmail, nameof(adminEmail));
        Check.NotNullOrWhiteSpace(adminPassword, nameof(adminPassword));

        using (CurrentTenant.Change(tenantId))
        {
            await IdentityOptions.SetAsync();

            var result = new IdentityDataSeedResult();
            if (adminUserName.IsNullOrWhiteSpace())
            {
                adminUserName = IdentityDataSeedContributor.AdminUserNameDefaultValue;
            }

            var adminUser = await UserRepository.FindByNormalizedUserNameAsync(
                LookupNormalizer.NormalizeName(adminUserName));

            if (adminUser != null)
            {
                return result;
            }

            adminUser = new IdentityUser(
                DemoUserDefinitions.AdminUserId,
                adminUserName,
                adminEmail,
                tenantId)
            {
                Name = adminUserName
            };
            adminUser.SetEmailConfirmed(true);

            (await UserManager.CreateAsync(adminUser, adminPassword, validatePassword: false)).CheckErrors();
            result.CreatedAdminUser = true;

            const string adminRoleName = AbpRoleConsts.AdminRoleName;
            var adminRole =
                await RoleRepository.FindByNormalizedNameAsync(LookupNormalizer.NormalizeName(adminRoleName));
            if (adminRole == null)
            {
                adminRole = new IdentityRole(
                    GuidGenerator.Create(),
                    adminRoleName,
                    tenantId)
                {
                    IsStatic = true,
                    IsPublic = true
                };

                (await RoleManager.CreateAsync(adminRole)).CheckErrors();
                result.CreatedAdminRole = true;
            }

            (await UserManager.AddToRoleAsync(adminUser, adminRoleName)).CheckErrors();

            return result;
        }
    }
}
