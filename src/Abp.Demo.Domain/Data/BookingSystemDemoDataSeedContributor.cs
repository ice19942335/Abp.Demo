using System;
using System.Threading.Tasks;
using Abp.Demo.Booking;
using Abp.Demo.Bookings;
using BookingEntity = Abp.Demo.Bookings.Booking;
using Abp.Demo.Database;
using Abp.Demo.Demo;
using Abp.Demo.Resources;
using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace Abp.Demo.Data;

public class BookingSystemDemoDataSeedContributor(
    IOptions<DatabaseOptions> databaseOptions,
    IRepository<Resource, Guid> resourceRepository,
    IBookingRepository bookingRepository,
    IGuidGenerator guidGenerator)
    : IDataSeedContributor, ITransientDependency
{
    private static readonly Guid ConferenceRoomId = Guid.Parse("d0000000-0000-0000-0000-000000000001");
    private static readonly Guid WorkspaceId = Guid.Parse("d0000000-0000-0000-0000-000000000002");
    private readonly DatabaseOptions databaseOptions = databaseOptions.Value;

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (!databaseOptions.IsInMemory)
        {
            return;
        }

        if (await resourceRepository.GetCountAsync() > 0)
        {
            return;
        }

        await resourceRepository.InsertAsync(
            new Resource(
                ConferenceRoomId,
                "Conference Room A",
                "Main floor conference room with projector",
                "Building 1, Floor 1",
                10,
                ResourceType.MeetingRoom),
            autoSave: true);

        await resourceRepository.InsertAsync(
            new Resource(
                WorkspaceId,
                "Open Workspace",
                "Shared desk area",
                "Building 1, Floor 2",
                20,
                ResourceType.Workspace),
            autoSave: true);

        var startTime = DateTime.UtcNow.AddDays(1).Date.AddHours(10);

        await bookingRepository.InsertAsync(
            new BookingEntity(
                guidGenerator.Create(),
                ConferenceRoomId,
                DemoUserDefinitions.AdminUserId,
                startTime,
                startTime.AddHours(1),
                "Team standup"),
            autoSave: true);

        var confirmedBooking = new BookingEntity(
            guidGenerator.Create(),
            ConferenceRoomId,
            DemoUserDefinitions.AdminUserId,
            startTime.AddHours(2),
            startTime.AddHours(3),
            "Sprint planning");
        confirmedBooking.Confirm();

        await bookingRepository.InsertAsync(confirmedBooking, autoSave: true);
    }
}
