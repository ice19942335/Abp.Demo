using System;
using System.Threading.Tasks;
using Abp.Demo.Booking;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Xunit;

namespace Abp.Demo.Resources;

public abstract class ResourceAppServiceTests<TStartupModule> : DemoApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IResourceAppService _resourceAppService;

    protected ResourceAppServiceTests()
    {
        _resourceAppService = GetRequiredService<IResourceAppService>();
    }

    [Fact]
    public async Task Should_Get_List()
    {
        var result = await _resourceAppService.GetListAsync(new PagedAndSortedResultRequestDto());
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Should_Create_Resource()
    {
        var result = await _resourceAppService.CreateAsync(new CreateUpdateResourceDto
        {
            Name = "New Workspace",
            Description = "Open plan workspace",
            Location = "Floor 3",
            Capacity = 20,
            Type = ResourceType.Workspace,
            IsActive = true
        });

        result.ShouldNotBeNull();
        result.Name.ShouldBe("New Workspace");
        result.Type.ShouldBe(ResourceType.Workspace);
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Get_Resource()
    {
        var resource = await _resourceAppService.GetAsync(BookingSystemTestDataSeedContributor.TestResourceId);

        resource.ShouldNotBeNull();
        resource.Name.ShouldBe("Conference Room A");
    }

    [Fact]
    public async Task Should_Update_Resource()
    {
        var resource = await _resourceAppService.UpdateAsync(
            BookingSystemTestDataSeedContributor.TestResourceId,
            new CreateUpdateResourceDto
            {
                Name = "Updated Room",
                Description = "Updated description",
                Location = "Floor 2",
                Capacity = 15,
                Type = ResourceType.MeetingRoom,
                IsActive = true
            });

        resource.Name.ShouldBe("Updated Room");
        resource.Capacity.ShouldBe(15);
    }

    [Fact]
    public async Task Should_Delete_Resource()
    {
        var beforeDelete = await _resourceAppService.GetListAsync(new PagedAndSortedResultRequestDto());
        var countBefore = beforeDelete.TotalCount;

        await _resourceAppService.DeleteAsync(BookingSystemTestDataSeedContributor.TestResourceId);

        var afterDelete = await _resourceAppService.GetListAsync(new PagedAndSortedResultRequestDto());
        afterDelete.TotalCount.ShouldBe(countBefore - 1);
    }
}
