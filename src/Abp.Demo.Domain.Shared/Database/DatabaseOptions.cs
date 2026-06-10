namespace Abp.Demo.Database;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; set; } = DatabaseProviderNames.InMemory;

    public bool InitializeOnStartup { get; set; } = true;

    public string InMemoryDatabaseName { get; set; } = "BookingSystemDb";

    public bool IsInMemory => Provider == DatabaseProviderNames.InMemory;
}
