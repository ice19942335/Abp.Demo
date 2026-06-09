using System;
using Abp.Demo.Booking;

namespace Abp.Demo.Bookings;

public class NextAvailableSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public ResourceType ResourceType { get; set; }
}
