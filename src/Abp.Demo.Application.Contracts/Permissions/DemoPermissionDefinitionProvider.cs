using Abp.Demo.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Abp.Demo.Permissions;

public class DemoPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var bookingGroup = context.AddGroup(DemoPermissions.GroupName);

        var resources = bookingGroup.AddPermission(DemoPermissions.Resources.Default, L("Permission:Resources"));
        resources.AddChild(DemoPermissions.Resources.Create, L("Permission:Resources.Create"));
        resources.AddChild(DemoPermissions.Resources.Edit, L("Permission:Resources.Edit"));
        resources.AddChild(DemoPermissions.Resources.Delete, L("Permission:Resources.Delete"));

        var bookings = bookingGroup.AddPermission(DemoPermissions.Bookings.Default, L("Permission:Bookings"));
        bookings.AddChild(DemoPermissions.Bookings.Create, L("Permission:Bookings.Create"));
        bookings.AddChild(DemoPermissions.Bookings.Cancel, L("Permission:Bookings.Cancel"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DemoResource>(name);
    }
}
