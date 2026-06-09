using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Demo.Bookings;
using Abp.Demo.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;

namespace Abp.Demo.Web.Pages.Bookings;

public class CreateModalModel : DemoPageModel
{
    [BindProperty]
    public CreateBookingDto Booking { get; set; } = null!;

    public List<SelectListItem> ResourceList { get; set; } = null!;

    public Dictionary<string, int> ResourceTypes { get; set; } = null!;

    private readonly IBookingAppService bookingAppService;
    private readonly IResourceAppService resourceAppService;

    public CreateModalModel(
        IBookingAppService bookingAppService,
        IResourceAppService resourceAppService)
    {
        this.bookingAppService = bookingAppService;
        this.resourceAppService = resourceAppService;
    }

    public async Task OnGetAsync()
    {
        Booking = new CreateBookingDto();

        var resources = await resourceAppService.GetListAsync(
            new PagedAndSortedResultRequestDto { MaxResultCount = 1000 });

        var activeResources = resources.Items.Where(r => r.IsActive).ToList();

        ResourceList = activeResources
            .Select(r => new SelectListItem(r.Name, r.Id.ToString()))
            .ToList();

        ResourceTypes = activeResources
            .ToDictionary(r => r.Id.ToString(), r => (int)r.Type);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await bookingAppService.CreateAsync(Booking);
        return NoContent();
    }
}
