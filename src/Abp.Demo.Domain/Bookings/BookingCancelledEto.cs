using System;

namespace Abp.Demo.Bookings;

public class BookingCancelledEto
{
    public Guid BookingId { get; set; }
    public Guid ResourceId { get; set; }
    public Guid UserId { get; set; }
    public string Reason { get; set; } = null!;
}
