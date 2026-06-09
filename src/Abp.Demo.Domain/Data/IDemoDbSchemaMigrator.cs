using System.Threading.Tasks;

namespace Abp.Demo.Data;

public interface IDemoDbSchemaMigrator
{
    Task MigrateAsync();
}
