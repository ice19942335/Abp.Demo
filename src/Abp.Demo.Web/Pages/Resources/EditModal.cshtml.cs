using System;
using System.Threading.Tasks;
using Abp.Demo.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Abp.Demo.Web.Pages.Resources;

public class EditModalModel : DemoPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateResourceDto Resource { get; set; } = null!;

    private readonly IResourceAppService resourceAppService;

    public EditModalModel(IResourceAppService resourceAppService)
    {
        this.resourceAppService = resourceAppService;
    }

    public async Task OnGetAsync()
    {
        var dto = await resourceAppService.GetAsync(Id);
        Resource = new CreateUpdateResourceDto
        {
            Name = dto.Name,
            Description = dto.Description,
            Location = dto.Location,
            Capacity = dto.Capacity,
            Type = dto.Type,
            IsActive = dto.IsActive
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await resourceAppService.UpdateAsync(Id, Resource);
        return NoContent();
    }
}
