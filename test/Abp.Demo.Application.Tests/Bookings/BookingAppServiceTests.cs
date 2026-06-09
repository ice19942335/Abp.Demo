using System;
using System.Threading.Tasks;
using Abp.Demo.Booking;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Modularity;
using Xunit;

namespace Abp.Demo.Bookings;

public abstract class BookingAppServiceTests<TStartupModule> : DemoApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IBookingAppService bookingAppService;

    protected BookingAppServiceTests()
    {
        bookingAppService = GetRequiredService<IBookingAppService>();
    }

    [Fact]
    public async Task Should_Get_Booking()
    {
        var booking = await bookingAppService.GetAsync(BookingSystemTestDataSeedContributor.PendingBookingId);

        booking.ShouldNotBeNull();
        booking.Status.ShouldBe(BookingStatus.Pending);
        booking.Purpose.ShouldBe("Team standup");
    }

    [Fact]
    public async Task Should_Get_List()
    {
        var result = await bookingAppService.GetListAsync(new GetBookingListDto());

        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Should_Filter_By_Status()
    {
        var result = await bookingAppService.GetListAsync(new GetBookingListDto
        {
            Status = BookingStatus.Confirmed
        });

        result.Items.ShouldAllBe(b => b.Status == BookingStatus.Confirmed);
    }

    [Fact]
    public async Task Should_Confirm_Booking()
    {
        var result = await bookingAppService.ConfirmAsync(BookingSystemTestDataSeedContributor.PendingBookingId);
        result.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public async Task Should_Complete_Booking()
    {
        var result = await bookingAppService.CompleteAsync(BookingSystemTestDataSeedContributor.ConfirmedBookingId);
        result.Status.ShouldBe(BookingStatus.Completed);
    }

    [Fact]
    public async Task Should_Cancel_Booking()
    {
        var result = await bookingAppService.CancelAsync(
            BookingSystemTestDataSeedContributor.PendingBookingId, "No longer needed");

        result.Status.ShouldBe(BookingStatus.Cancelled);
        result.CancellationReason.ShouldBe("No longer needed");
    }

    [Fact]
    public async Task Should_Create_Booking()
    {
        var baseTime = DateTime.UtcNow.AddDays(5).Date.AddHours(14);

        var result = await bookingAppService.CreateAsync(new CreateBookingDto
        {
            ResourceId = BookingSystemTestDataSeedContributor.TestResourceId,
            StartTime = baseTime,
            EndTime = baseTime.AddHours(1),
            Purpose = "Design review"
        });

        result.ShouldNotBeNull();
        result.Status.ShouldBe(BookingStatus.Pending);
        result.Purpose.ShouldBe("Design review");
    }

    [Fact]
    public async Task Should_Throw_On_Inactive_Resource()
    {
        var baseTime = DateTime.UtcNow.AddDays(5).Date.AddHours(14);

        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
            await bookingAppService.CreateAsync(new CreateBookingDto
            {
                ResourceId = BookingSystemTestDataSeedContributor.InactiveResourceId,
                StartTime = baseTime,
                EndTime = baseTime.AddHours(1),
                Purpose = "Test"
            }));

        exception.Code.ShouldBe(DemoDomainErrorCodes.ResourceNotActive);
    }

    [Fact]
    public async Task Should_Throw_On_Conflicting_Booking()
    {
        var baseTime = DateTime.UtcNow.AddDays(1).Date.AddHours(10);

        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
            await bookingAppService.CreateAsync(new CreateBookingDto
            {
                ResourceId = BookingSystemTestDataSeedContributor.TestResourceId,
                StartTime = baseTime,
                EndTime = baseTime.AddHours(1),
                Purpose = "Conflicting meeting"
            }));

        exception.Code.ShouldBe(DemoDomainErrorCodes.BookingConflict);
    }

    [Fact]
    public async Task Should_Get_NextAvailableSlot_For_MeetingRoom()
    {
        var slot = await bookingAppService.GetNextAvailableSlotAsync(
            BookingSystemTestDataSeedContributor.TestResourceId);

        slot.ShouldNotBeNull();
        slot.ResourceType.ShouldBe(ResourceType.MeetingRoom);
        slot.StartTime.ShouldBeLessThan(slot.EndTime);
        (slot.EndTime - slot.StartTime).TotalHours.ShouldBe(1);
    }

    [Fact]
    public async Task Should_Get_NextAvailableSlot_For_Workspace()
    {
        var slot = await bookingAppService.GetNextAvailableSlotAsync(
            SlotTestDataSeedContributor.WorkspaceResourceId);

        slot.ShouldNotBeNull();
        slot.ResourceType.ShouldBe(ResourceType.Workspace);
        (slot.EndTime - slot.StartTime).TotalDays.ShouldBe(1);
    }

    [Fact]
    public async Task Should_Get_NextAvailableSlot_For_Car()
    {
        var slot = await bookingAppService.GetNextAvailableSlotAsync(
            SlotTestDataSeedContributor.CarResourceId);

        slot.ShouldNotBeNull();
        slot.ResourceType.ShouldBe(ResourceType.Car);
        slot.IsAvailable.ShouldBeTrue();
        (slot.EndTime - slot.StartTime).TotalHours.ShouldBe(1);
    }
}
