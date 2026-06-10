using System;
using System.Threading.Tasks;
using Abp.Demo.Bookings;
using Shouldly;
using Xunit;

namespace Abp.Demo.EntityFrameworkCore.Bookings;

[Collection(DemoTestConsts.CollectionDefinitionName)]
public class EfCoreBookingRepositoryTests : DemoEntityFrameworkCoreTestBase
{
    private readonly IBookingRepository _bookingRepository;

    public EfCoreBookingRepositoryTests()
    {
        _bookingRepository = GetRequiredService<IBookingRepository>();
    }

    [Fact]
    public async Task Should_Detect_Conflict_On_Overlapping_Time()
    {
        var baseTime = DateTime.UtcNow.AddDays(1).Date.AddHours(10);

        var hasConflict = await _bookingRepository.HasConflictAsync(
            BookingSystemTestDataSeedContributor.TestResourceId,
            baseTime,
            baseTime.AddHours(1));

        hasConflict.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Not_Detect_Conflict_On_Non_Overlapping_Time()
    {
        var baseTime = DateTime.UtcNow.AddDays(1).Date.AddHours(10);

        var hasConflict = await _bookingRepository.HasConflictAsync(
            BookingSystemTestDataSeedContributor.TestResourceId,
            baseTime.AddHours(5),
            baseTime.AddHours(6));

        hasConflict.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Exclude_Specific_Booking_From_Conflict_Check()
    {
        var baseTime = DateTime.UtcNow.AddDays(1).Date.AddHours(10);

        var hasConflict = await _bookingRepository.HasConflictAsync(
            BookingSystemTestDataSeedContributor.TestResourceId,
            baseTime,
            baseTime.AddHours(1),
            excludeBookingId: BookingSystemTestDataSeedContributor.PendingBookingId);

        hasConflict.ShouldBeFalse();
    }
}
