using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Demo.Booking;
using Abp.Demo.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Abp.Demo.Bookings;

public class EfCoreBookingRepository(IDbContextProvider<DemoDbContext> dbContextProvider)
    : EfCoreRepository<DemoDbContext, Booking, Guid>(dbContextProvider), IBookingRepository
{
    public async Task<bool> HasConflictAsync(
        Guid resourceId,
        DateTime startTime,
        DateTime endTime,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.AnyAsync(
            b => b.ResourceId == resourceId
                 && b.Status != BookingStatus.Cancelled
                 && b.Status != BookingStatus.Completed
                 && b.StartTime < endTime
                 && b.EndTime > startTime
                 && (!excludeBookingId.HasValue || b.Id != excludeBookingId.Value),
            GetCancellationToken(cancellationToken));
    }
}
