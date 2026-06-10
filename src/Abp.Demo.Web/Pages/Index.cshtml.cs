using System.Threading.Tasks;
using Abp.Demo.Bookings;

namespace Abp.Demo.Web.Pages;

public class IndexModel(IBookingAppService bookingAppService) : DemoPageModel
{
    public DashboardDto Dashboard { get; set; } = new();

    public async Task OnGetAsync()
    {
        if (!CurrentUser.IsAuthenticated)
        {
            return;
        }

        Dashboard = await bookingAppService.GetDashboardAsync();
    }
}
