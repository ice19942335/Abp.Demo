using Microsoft.AspNetCore.Builder;
using Abp.Demo;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Abp.Demo.Web.csproj"); 
await builder.RunAbpModuleAsync<DemoWebTestModule>(applicationName: "Abp.Demo.Web");

public partial class Program
{
}
