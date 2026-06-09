using System;
using System.Threading.Tasks;
using Abp.Demo.Resources;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Abp.Demo.Bookings;

public class BookingManager : DomainService
{
    private readonly IBookingRepository bookingRepository;
    private readonly IRepository<Resource, Guid> resourceRepository;

    public BookingManager(
        IBookingRepository bookingRepository,
        IRepository<Resource, Guid> resourceRepository)
    {
        this.bookingRepository = bookingRepository;
        this.resourceRepository = resourceRepository;
    }

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
