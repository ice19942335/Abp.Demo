using System;

namespace Abp.Demo.Bookings;

public class BookingCreatedEto
{
    public Guid BookingId { get; set; }
    public Guid ResourceId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
