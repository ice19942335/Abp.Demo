using System.Threading.Tasks;
using Abp.Demo.Localization;
using Abp.Demo.Permissions;
using Abp.Demo.MultiTenancy;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;

namespace Abp.Demo.Web.Menus;

public class DemoMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<DemoResource>();

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DemoMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fa fa-home",
                order: 1
            )
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DemoMenus.Resources,
                l["Menu:Resources"],
                "~/Resources",
                icon: "fa fa-building",
                order: 2
            ).RequirePermissions(DemoPermissions.Resources.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DemoMenus.Bookings,
                l["Menu:Bookings"],
                "~/Bookings",
                icon: "fa fa-calendar-check",
                order: 3
            ).RequirePermissions(DemoPermissions.Bookings.Default)
        );

        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        return Task.CompletedTask;
    }
}
