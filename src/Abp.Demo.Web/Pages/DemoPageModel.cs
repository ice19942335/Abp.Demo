using Abp.Demo.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Abp.Demo.Web.Pages;

public abstract class DemoPageModel : AbpPageModel
{
    protected DemoPageModel()
    {
        LocalizationResourceType = typeof(DemoResource);
    }
}
