using System;
using System.Threading.Tasks;
using Abp.Demo.Booking;
using Abp.Demo.Bookings;
using Abp.Demo.Resources;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Abp.Demo;

public class BookingSystemTestDataSeedContributor(
    IRepository<Resource, Guid> resourceRepository,
    IBookingRepository bookingRepository,
    IGuidGenerator guidGenerator)
    : IDataSeedContributor, ITransientDependency
{
    public static readonly Guid TestResourceId = Guid.Parse("a0000000-0000-0000-0000-000000000001");
    public static readonly Guid InactiveResourceId = Guid.Parse("a0000000-0000-0000-0000-000000000002");
    public static readonly Guid PendingBookingId = Guid.Parse("b0000000-0000-0000-0000-000000000001");
    public static readonly Guid ConfirmedBookingId = Guid.Parse("b0000000-0000-0000-0000-000000000002");
    public static readonly Guid TestUserId = Guid.Parse("c0000000-0000-0000-0000-000000000001");

    private readonly IGuidGenerator _guidGenerator = guidGenerator;

    public async Task SeedAsync(DataSeedContext context)
    {
        var activeResource = new Resource(
            TestResourceId,
            "Conference Room A",
            "Main floor conference room",
            "Building 1, Floor 1",
            10,
            ResourceType.MeetingRoom);

        await resourceRepository.InsertAsync(activeResource, autoSave: true);

        var inactiveResource = new Resource(
            InactiveResourceId,
            "Old Office Car",
            "Decommissioned vehicle",
            "Parking Lot B",
            4,
            ResourceType.Car,
            isActive: false);

        await resourceRepository.InsertAsync(inactiveResource, autoSave: true);

        var baseTime = DateTime.UtcNow.AddDays(1).Date.AddHours(10);

        var pendingBooking = new Abp.Demo.Bookings.Booking(
            PendingBookingId,
            TestResourceId,
            TestUserId,
            baseTime,
            baseTime.AddHours(1),
            "Team standup");

        await bookingRepository.InsertAsync(pendingBooking, autoSave: true);

        var confirmedBooking = new Abp.Demo.Bookings.Booking(
            ConfirmedBookingId,
            TestResourceId,
            TestUserId,
            baseTime.AddHours(2),
            baseTime.AddHours(3),
            "Sprint planning");
        confirmedBooking.Confirm();

        await bookingRepository.InsertAsync(confirmedBooking, autoSave: true);
    }
}
