using System.Collections.Generic;

namespace Abp.Demo.Bookings;

public class DashboardDto
{
    public int TotalResources { get; set; }
    public int ActiveResources { get; set; }
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public List<BookingDto> UpcomingBookings { get; set; } = new();
}
