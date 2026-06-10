using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Abp.Demo.Data;
using Abp.Demo.Database;
using Volo.Abp.DependencyInjection;

namespace Abp.Demo.EntityFrameworkCore;

public class EntityFrameworkCoreDemoDbSchemaMigrator(
    IServiceProvider serviceProvider,
    IOptions<DatabaseOptions> databaseOptions)
    : IDemoDbSchemaMigrator, ITransientDependency
{
    private readonly DatabaseOptions _databaseOptions = databaseOptions.Value;

    public async Task MigrateAsync()
    {
        var dbContext = serviceProvider.GetRequiredService<DemoDbContext>();

        if (_databaseOptions.IsInMemory)
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }
        else
        {
            await dbContext.Database.MigrateAsync();
        }
    }
}
