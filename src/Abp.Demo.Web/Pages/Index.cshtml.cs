using System.Threading.Tasks;
using Abp.Demo.Bookings;

namespace Abp.Demo.Web.Pages;

public class IndexModel : DemoPageModel
{
    public DashboardDto Dashboard { get; set; } = null!;

    private readonly IBookingAppService bookingAppService;

    public IndexModel(IBookingAppService bookingAppService)
    {
        this.bookingAppService = bookingAppService;
    }

    public async Task OnGetAsync()
    {
        Dashboard = await bookingAppService.GetDashboardAsync();
    }
}
