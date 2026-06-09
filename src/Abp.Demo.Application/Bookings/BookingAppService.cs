using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Demo.Permissions;
using Abp.Demo.Resources;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Abp.Demo.Bookings;

[Authorize(DemoPermissions.Bookings.Default)]
public class BookingAppService : DemoAppService, IBookingAppService
{
    private readonly IBookingRepository bookingRepository;
    private readonly BookingManager bookingManager;
    private readonly IRepository<Resource, Guid> resourceRepository;

    public BookingAppService(
        IBookingRepository bookingRepository,
        BookingManager bookingManager,
        IRepository<Resource, Guid> resourceRepository)
    {
        this.bookingRepository = bookingRepository;
        this.bookingManager = bookingManager;
        this.resourceRepository = resourceRepository;
    }

    public async Task<BookingDto> GetAsync(Guid id)
    {
        var booking = await bookingRepository.GetAsync(id);
        return await ToBookingDtoAsync(booking);
    }

    public async Task<PagedResultDto<BookingDto>> GetListAsync(GetBookingListDto input)
    {
        var queryable = await bookingRepository.GetQueryableAsync();

        if (input.ResourceId.HasValue)
            queryable = queryable.Where(b => b.ResourceId == input.ResourceId.Value);

        if (input.Status.HasValue)
            queryable = queryable.Where(b => b.Status == input.Status.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = ApplySorting(queryable, input);
        queryable = ApplyPaging(queryable, input);

        var bookings = await AsyncExecuter.ToListAsync(queryable);
        var resourceIds = bookings.Select(b => b.ResourceId).Distinct().ToList();
        var resources = await AsyncExecuter.ToListAsync(
            (await resourceRepository.GetQueryableAsync()).Where(r => resourceIds.Contains(r.Id)));
        var resourceDict = resources.ToDictionary(r => r.Id, r => r.Name);

        var dtos = bookings.Select(b => MapToDto(b, resourceDict)).ToList();

        return new PagedResultDto<BookingDto>(totalCount, dtos);
    }

    [Authorize(DemoPermissions.Bookings.Create)]
    public async Task<BookingDto> CreateAsync(CreateBookingDto input)
    {
        var booking = await bookingManager.CreateAsync(
            input.ResourceId,
            CurrentUser.Id!.Value,
            input.StartTime,
            input.EndTime,
            input.Purpose);

        await bookingRepository.InsertAsync(booking);

        return await ToBookingDtoAsync(booking);
    }

    public async Task<BookingDto> ConfirmAsync(Guid id)
    {
        var booking = await bookingRepository.GetAsync(id);
        booking.Confirm();
        await bookingRepository.UpdateAsync(booking);
        return await ToBookingDtoAsync(booking);
    }

    public async Task<BookingDto> CompleteAsync(Guid id)
    {
        var booking = await bookingRepository.GetAsync(id);
        booking.Complete();
        await bookingRepository.UpdateAsync(booking);
        return await ToBookingDtoAsync(booking);
    }

    [Authorize(DemoPermissions.Bookings.Cancel)]
    public async Task<BookingDto> CancelAsync(Guid id, string reason)
    {
        var booking = await bookingRepository.GetAsync(id);
        booking.Cancel(reason);
        await bookingRepository.UpdateAsync(booking);
        return await ToBookingDtoAsync(booking);
    }

    public async Task<NextAvailableSlotDto> GetNextAvailableSlotAsync(Guid resourceId)
    {
        var resource = await resourceRepository.GetAsync(resourceId);
        var now = DateTime.UtcNow;

        DateTime slotStart;
        DateTime slotEnd;

        if (resource.Type == Abp.Demo.Booking.ResourceType.Workspace)
        {
            slotStart = now.Date.AddDays(1);
            slotEnd = slotStart.AddDays(1);
        }
        else
        {
            slotStart = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddHours(1);
            slotEnd = slotStart.AddHours(1);
        }

        var hasConflict = await bookingRepository.HasConflictAsync(resourceId, slotStart, slotEnd);

        return new NextAvailableSlotDto
        {
            StartTime = slotStart,
            EndTime = slotEnd,
            IsAvailable = !hasConflict,
            ResourceType = resource.Type
        };
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var now = DateTime.UtcNow;

        var resourceQueryable = await resourceRepository.GetQueryableAsync();
        var totalResources = await AsyncExecuter.CountAsync(resourceQueryable);
        var activeResources = await AsyncExecuter.CountAsync(resourceQueryable.Where(r => r.IsActive));

        var bookingQueryable = await bookingRepository.GetQueryableAsync();
        var totalBookings = await AsyncExecuter.CountAsync(bookingQueryable);
        var pendingBookings = await AsyncExecuter.CountAsync(
            bookingQueryable.Where(b => b.Status == Abp.Demo.Booking.BookingStatus.Pending));
        var confirmedBookings = await AsyncExecuter.CountAsync(
            bookingQueryable.Where(b => b.Status == Abp.Demo.Booking.BookingStatus.Confirmed));
        var completedBookings = await AsyncExecuter.CountAsync(
            bookingQueryable.Where(b => b.Status == Abp.Demo.Booking.BookingStatus.Completed));
        var cancelledBookings = await AsyncExecuter.CountAsync(
            bookingQueryable.Where(b => b.Status == Abp.Demo.Booking.BookingStatus.Cancelled));

        var upcomingBookings = await AsyncExecuter.ToListAsync(
            bookingQueryable
                .Where(b => b.EndTime > now
                            && b.Status != Abp.Demo.Booking.BookingStatus.Cancelled
                            && b.Status != Abp.Demo.Booking.BookingStatus.Completed)
                .OrderBy(b => b.StartTime)
                .Take(5));

        var resourceIds = upcomingBookings.Select(b => b.ResourceId).Distinct().ToList();
        var resources = await AsyncExecuter.ToListAsync(
            resourceQueryable.Where(r => resourceIds.Contains(r.Id)));
        var resourceDict = resources.ToDictionary(r => r.Id, r => r.Name);

        return new DashboardDto
        {
            TotalResources = totalResources,
            ActiveResources = activeResources,
            TotalBookings = totalBookings,
            PendingBookings = pendingBookings,
            ConfirmedBookings = confirmedBookings,
            CompletedBookings = completedBookings,
            CancelledBookings = cancelledBookings,
            UpcomingBookings = upcomingBookings.Select(b => MapToDto(b, resourceDict)).ToList()
        };
    }

    private async Task<BookingDto> ToBookingDtoAsync(Booking booking)
    {
        var resource = await resourceRepository.GetAsync(booking.ResourceId);
        return MapToDto(booking, new() { { resource.Id, resource.Name } });
    }

    private static BookingDto MapToDto(Booking booking, Dictionary<Guid, string> resourceNames)
    {
        return new BookingDto
        {
            Id = booking.Id,
            ResourceId = booking.ResourceId,
            ResourceName = resourceNames.TryGetValue(booking.ResourceId, out var name) ? name : string.Empty,
            UserId = booking.UserId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Purpose = booking.Purpose,
            Status = booking.Status,
            CancellationReason = booking.CancellationReason,
            CreationTime = booking.CreationTime,
            CreatorId = booking.CreatorId,
            LastModificationTime = booking.LastModificationTime,
            LastModifierId = booking.LastModifierId
        };
    }

    private static IQueryable<Booking> ApplySorting(IQueryable<Booking> queryable, GetBookingListDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Sorting))
            return queryable.OrderByDescending(b => b.StartTime);

        return input.Sorting.ToLowerInvariant() switch
        {
            "starttime" or "starttime asc" => queryable.OrderBy(b => b.StartTime),
            "starttime desc" => queryable.OrderByDescending(b => b.StartTime),
            "status" or "status asc" => queryable.OrderBy(b => b.Status),
            "status desc" => queryable.OrderByDescending(b => b.Status),
            _ => queryable.OrderByDescending(b => b.StartTime)
        };
    }

    private static IQueryable<Booking> ApplyPaging(IQueryable<Booking> queryable, GetBookingListDto input)
    {
        return queryable.Skip(input.SkipCount).Take(input.MaxResultCount);
    }
}
