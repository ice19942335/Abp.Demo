using System;
using Abp.Demo.Booking;
using Volo.Abp.Application.Dtos;

namespace Abp.Demo.Bookings;

public class BookingDto : FullAuditedEntityDto<Guid>
{
    public Guid ResourceId { get; set; }
    public string ResourceName { get; set; } = null!;
    public Guid UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Purpose { get; set; } = null!;
    public BookingStatus Status { get; set; }
    public string? CancellationReason { get; set; }
}
