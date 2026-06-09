using Volo.Abp.Modularity;

namespace Abp.Demo;

public abstract class DemoApplicationTestBase<TStartupModule> : DemoTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
