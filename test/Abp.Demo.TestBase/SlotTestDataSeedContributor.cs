using System;
using System.Threading.Tasks;
using Abp.Demo.Booking;
using Abp.Demo.Resources;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Abp.Demo;

public class SlotTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public static readonly Guid WorkspaceResourceId = Guid.Parse("a0000000-0000-0000-0000-000000000003");
    public static readonly Guid CarResourceId = Guid.Parse("a0000000-0000-0000-0000-000000000004");

    private readonly IRepository<Resource, Guid> resourceRepository;

    public SlotTestDataSeedContributor(IRepository<Resource, Guid> resourceRepository)
    {
        this.resourceRepository = resourceRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var workspace = new Resource(
            WorkspaceResourceId,
            "Hot Desk #1",
            "Open floor workspace",
            "Building 2, Floor 3",
            1,
            ResourceType.Workspace);

        await resourceRepository.InsertAsync(workspace, autoSave: true);

        var car = new Resource(
            CarResourceId,
            "Company Car BMW",
            "Pool car for business trips",
            "Parking Lot A",
            5,
            ResourceType.Car);

        await resourceRepository.InsertAsync(car, autoSave: true);
    }
}
