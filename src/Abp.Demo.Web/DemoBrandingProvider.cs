using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using Abp.Demo.Localization;

namespace Abp.Demo.Web;

[Dependency(ReplaceServices = true)]
public class DemoBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DemoResource> _localizer;

    public DemoBrandingProvider(IStringLocalizer<DemoResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
