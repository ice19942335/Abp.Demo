using System;
using System.ComponentModel.DataAnnotations;
using Abp.Demo.Booking;

namespace Abp.Demo.Bookings;

public class CreateBookingDto
{
    [Required]
    public Guid ResourceId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [StringLength(BookingConsts.MaxPurposeLength)]
    public string Purpose { get; set; } = null!;
}
