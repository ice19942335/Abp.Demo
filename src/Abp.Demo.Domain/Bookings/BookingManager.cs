using System;
using System.Threading.Tasks;
using Abp.Demo.Resources;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Abp.Demo.Bookings;

public class BookingManager(
    IBookingRepository bookingRepository,
    IRepository<Resource, Guid> resourceRepository)
    : DomainService
{
    public async Task<Booking> CreateAsync(
        Guid resourceId,
        Guid userId,
        DateTime startTime,
        DateTime endTime,
        string purpose)
    {
        var resource = await resourceRepository.GetAsync(resourceId);

        if (!resource.IsActive)
            throw new BusinessException(DemoDomainErrorCodes.ResourceNotActive);

        var hasConflict = await bookingRepository.HasConflictAsync(resourceId, startTime, endTime);

        if (hasConflict)
            throw new BusinessException(DemoDomainErrorCodes.BookingConflict);

        return new Booking(
            GuidGenerator.Create(),
            resourceId,
            userId,
            startTime,
            endTime,
            purpose);
    }
}
