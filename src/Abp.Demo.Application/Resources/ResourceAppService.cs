using System;
using System.Threading.Tasks;
using Abp.Demo.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Abp.Demo.Resources;

public class ResourceAppService
    : CrudAppService<Resource, ResourceDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateResourceDto>,
      IResourceAppService
{
    public ResourceAppService(IRepository<Resource, Guid> repository)
        : base(repository)
    {
        GetPolicyName = DemoPermissions.Resources.Default;
        GetListPolicyName = DemoPermissions.Resources.Default;
        CreatePolicyName = DemoPermissions.Resources.Create;
        UpdatePolicyName = DemoPermissions.Resources.Edit;
        DeletePolicyName = DemoPermissions.Resources.Delete;
    }

    protected override Task<Resource> MapToEntityAsync(CreateUpdateResourceDto createInput)
    {
        var resource = new Resource(
            GuidGenerator.Create(),
            createInput.Name,
            createInput.Description,
            createInput.Location,
            createInput.Capacity,
            createInput.Type,
            createInput.IsActive);

        return Task.FromResult(resource);
    }

    protected override Task MapToEntityAsync(CreateUpdateResourceDto updateInput, Resource entity)
    {
        entity.SetName(updateInput.Name);
        entity.SetDescription(updateInput.Description);
        entity.SetLocation(updateInput.Location);
        entity.SetCapacity(updateInput.Capacity);

        if (updateInput.IsActive)
            entity.Activate();
        else
            entity.Deactivate();

        return Task.CompletedTask;
    }
}
