using System;
using Abp.Demo.Booking;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Abp.Demo.Bookings;

public class Booking : FullAuditedAggregateRoot<Guid>
{
    public Guid ResourceId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string Purpose { get; private set; } = null!;
    public BookingStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }

    protected Booking() { }

    internal Booking(
        Guid id,
        Guid resourceId,
        Guid userId,
        DateTime startTime,
        DateTime endTime,
        string purpose)
        : base(id)
    {
        ResourceId = resourceId;
        UserId = userId;
        SetTimeRange(startTime, endTime);
        Purpose = Check.NotNullOrWhiteSpace(purpose, nameof(purpose), BookingConsts.MaxPurposeLength);
        Status = BookingStatus.Pending;

        AddDistributedEvent(new BookingCreatedEto
        {
            BookingId = id,
            ResourceId = resourceId,
            UserId = userId,
            StartTime = startTime,
            EndTime = endTime
        });
    }

    public void Confirm()
    {
        EnsureValidTransition(BookingStatus.Confirmed);
        Status = BookingStatus.Confirmed;
    }

    public void Complete()
    {
        EnsureValidTransition(BookingStatus.Completed);
        Status = BookingStatus.Completed;
    }

    public void Cancel(string reason)
    {
        EnsureValidTransition(BookingStatus.Cancelled);
        CancellationReason = Check.NotNullOrWhiteSpace(reason, nameof(reason), BookingConsts.MaxCancellationReasonLength);
        Status = BookingStatus.Cancelled;

        AddDistributedEvent(new BookingCancelledEto
        {
            BookingId = Id,
            ResourceId = ResourceId,
            UserId = UserId,
            Reason = reason
        });
    }

    private void SetTimeRange(DateTime startTime, DateTime endTime)
    {
        if (endTime <= startTime)
            throw new BusinessException(DemoDomainErrorCodes.BookingInvalidTimeRange);

        StartTime = startTime;
        EndTime = endTime;
    }

    private void EnsureValidTransition(BookingStatus targetStatus)
    {
        var isValid = (Status, targetStatus) switch
        {
            (BookingStatus.Pending, BookingStatus.Confirmed) => true,
            (BookingStatus.Pending, BookingStatus.Cancelled) => true,
            (BookingStatus.Confirmed, BookingStatus.Completed) => true,
            (BookingStatus.Confirmed, BookingStatus.Cancelled) => true,
            _ => false
        };

        if (!isValid)
            throw new BusinessException(DemoDomainErrorCodes.InvalidBookingStatusTransition);
    }
}
