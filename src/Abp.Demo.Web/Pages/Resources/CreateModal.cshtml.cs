using System.Threading.Tasks;
using Abp.Demo.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Abp.Demo.Web.Pages.Resources;

public class CreateModalModel(IResourceAppService resourceAppService) : DemoPageModel
{
    [BindProperty]
    public CreateUpdateResourceDto Resource { get; set; } = null!;

    public void OnGet()
    {
        Resource = new CreateUpdateResourceDto { IsActive = true };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await resourceAppService.CreateAsync(Resource);
        return NoContent();
    }
}
