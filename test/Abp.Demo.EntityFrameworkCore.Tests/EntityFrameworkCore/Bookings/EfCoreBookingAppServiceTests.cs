using Abp.Demo.Bookings;
using Xunit;

namespace Abp.Demo.EntityFrameworkCore.Bookings;

[Collection(DemoTestConsts.CollectionDefinitionName)]
public class EfCoreBookingAppServiceTests : BookingAppServiceTests<DemoEntityFrameworkCoreTestModule>
{
}
