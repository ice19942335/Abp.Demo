using System.Threading;
using System.Threading.Tasks;
using Abp.Demo.Data;
using Abp.Demo.Database;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Abp.Demo.Web.Database;

public class DatabaseInitializationHostedService(
    IOptions<DatabaseOptions> databaseOptions,
    DemoDbMigrationService dbMigrationService,
    ILogger<DatabaseInitializationHostedService> logger)
    : IHostedService, ITransientDependency
{
    private readonly DatabaseOptions _databaseOptions = databaseOptions.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_databaseOptions.InitializeOnStartup)
        {
            return;
        }

        logger.LogInformation(
            "Initializing database on startup (Provider: {Provider})...",
            _databaseOptions.Provider);

        await dbMigrationService.MigrateAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
