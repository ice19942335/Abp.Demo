using Abp.Demo.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Abp.Demo.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(DemoEntityFrameworkCoreModule),
    typeof(DemoApplicationContractsModule)
)]
public class DemoDbMigratorModule : AbpModule
{
}
