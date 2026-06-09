using System;
using Abp.Demo.Booking;
using Shouldly;
using Xunit;

namespace Abp.Demo.Resources;

public class ResourceTests
{
    [Fact]
    public void Should_Create_Active_By_Default()
    {
        var resource = new Resource(
            Guid.NewGuid(), "Room A", "Description", "Floor 1", 10, ResourceType.MeetingRoom);

        resource.IsActive.ShouldBeTrue();
        resource.Type.ShouldBe(ResourceType.MeetingRoom);
    }

    [Fact]
    public void Should_Deactivate()
    {
        var resource = new Resource(
            Guid.NewGuid(), "Room A", "Description", "Floor 1", 10, ResourceType.MeetingRoom);

        resource.Deactivate();
        resource.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Should_Activate()
    {
        var resource = new Resource(
            Guid.NewGuid(), "Room A", "Description", "Floor 1", 10, ResourceType.MeetingRoom, isActive: false);

        resource.Activate();
        resource.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Should_Throw_For_Invalid_Capacity()
    {
        Assert.Throws<ArgumentException>(() => new Resource(
            Guid.NewGuid(), "Room A", "Description", "Floor 1", 0, ResourceType.MeetingRoom));
    }

    [Fact]
    public void Should_Set_Properties()
    {
        var resource = new Resource(
            Guid.NewGuid(), "Room A", "Description", "Floor 1", 10, ResourceType.MeetingRoom);

        resource.SetName("Room B");
        resource.SetDescription("Updated");
        resource.SetLocation("Floor 2");
        resource.SetCapacity(20);

        resource.Name.ShouldBe("Room B");
        resource.Description.ShouldBe("Updated");
        resource.Location.ShouldBe("Floor 2");
        resource.Capacity.ShouldBe(20);
    }
}
