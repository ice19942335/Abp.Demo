using System;
using Abp.Demo.Booking;
using Volo.Abp.Application.Dtos;

namespace Abp.Demo.Bookings;

public class GetBookingListDto : PagedAndSortedResultRequestDto
{
    public Guid? ResourceId { get; set; }
    public BookingStatus? Status { get; set; }
}
