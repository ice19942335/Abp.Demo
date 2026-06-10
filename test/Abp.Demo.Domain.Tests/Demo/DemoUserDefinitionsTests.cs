using Abp.Demo.Demo;
using Shouldly;
using Xunit;

namespace Abp.Demo.Demo;

public class DemoUserDefinitionsTests
{
    [Fact]
    public void Should_Define_Four_Demo_Users_With_Unique_Usernames()
    {
        DemoUserDefinitions.All.Count.ShouldBe(4);
        DemoUserDefinitions.All.ShouldContain(u => u.UserName == DemoUserDefinitions.AdminUserName);
        DemoUserDefinitions.All.ShouldContain(u => u.UserName == "manager" && u.RoleDisplayName == "Manager");
        DemoUserDefinitions.All.ShouldContain(u => u.UserName == "employee" && u.RoleDisplayName == "Employee");
        DemoUserDefinitions.All.ShouldContain(u => u.UserName == "viewer" && u.RoleDisplayName == "Viewer");
    }

    [Fact]
    public void Should_Use_Shared_Default_Password()
    {
        foreach (var user in DemoUserDefinitions.All)
        {
            user.Password.ShouldBe(DemoUserDefinitions.DefaultPassword);
        }
    }
}
