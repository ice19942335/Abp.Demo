using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Abp.Demo.Bookings;

public interface IBookingRepository : IRepository<Booking, Guid>
{
    Task<bool> HasConflictAsync(
        Guid resourceId,
        DateTime startTime,
        DateTime endTime,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default);
}
