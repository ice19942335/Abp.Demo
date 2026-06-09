using Abp.Demo.Resources;
using Xunit;

namespace Abp.Demo.EntityFrameworkCore.Resources;

[Collection(DemoTestConsts.CollectionDefinitionName)]
public class EfCoreResourceAppServiceTests : ResourceAppServiceTests<DemoEntityFrameworkCoreTestModule>
{
}
