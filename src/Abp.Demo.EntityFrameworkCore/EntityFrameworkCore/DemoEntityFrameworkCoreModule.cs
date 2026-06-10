using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Abp.Demo.Database;
using Volo.Abp.Uow;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Studio;
using Volo.Abp.EntityFrameworkCore.GlobalFilters;

namespace Abp.Demo.EntityFrameworkCore;

[DependsOn(
    typeof(DemoDomainModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule)
    )]
public class DemoEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        DemoEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        context.Services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        context.Services.AddAbpDbContext<DemoDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        if (AbpStudioAnalyzeHelper.IsInAnalyzeMode)
        {
            return;
        }

        var provider = configuration["Database:Provider"] ?? DatabaseProviderNames.InMemory;
        var inMemoryDatabaseName = configuration["Database:InMemoryDatabaseName"] ?? "BookingSystemDb";
        var isInMemory = provider == DatabaseProviderNames.InMemory;

        if (isInMemory)
        {
            Configure<AbpEfCoreGlobalFilterOptions>(filterOptions =>
            {
                filterOptions.UseDbFunction = false;
            });
        }

        Configure<AbpDbContextOptions>(options =>
        {
            if (isInMemory)
            {
                options.Configure(context =>
                {
                    context.DbContextOptions.UseInMemoryDatabase(inMemoryDatabaseName);
                });
            }
            else
            {
                options.UseNpgsql();
            }
        });
    }
}
