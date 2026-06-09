using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Abp.Demo.Bookings;

public interface IBookingAppService : IApplicationService
{
    Task<BookingDto> GetAsync(Guid id);
    Task<PagedResultDto<BookingDto>> GetListAsync(GetBookingListDto input);
    Task<BookingDto> CreateAsync(CreateBookingDto input);
    Task<BookingDto> ConfirmAsync(Guid id);
    Task<BookingDto> CompleteAsync(Guid id);
    Task<BookingDto> CancelAsync(Guid id, string reason);
    Task<NextAvailableSlotDto> GetNextAvailableSlotAsync(Guid resourceId);
    Task<DashboardDto> GetDashboardAsync();
}
