using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using Abp.Demo.Localization;

namespace Abp.Demo.Web;

[Dependency(ReplaceServices = true)]
public class DemoBrandingProvider(IStringLocalizer<DemoResource> localizer) : DefaultBrandingProvider
{
    public override string AppName => localizer["AppName"];
}
