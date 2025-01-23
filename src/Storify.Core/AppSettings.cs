namespace Storify.Core;

public class AppSettings
{
    public SporifyDbContextOptions SporifyDbContextOptions { get; set; } = null!;
}

public class SporifyDbContextOptions
{
    public string ConnectionString { get; set; } = null!;
    
    public string Schema { get; set; } = null!;
    
    public string MigrationsHistoryTable { get; set; } = null!;
}