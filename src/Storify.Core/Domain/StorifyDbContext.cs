using Microsoft.EntityFrameworkCore;

namespace Storify.Core.Domain;

public sealed class StorifyDbContext(DbContextOptions<StorifyDbContext> options, AppSettings appSettings) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(appSettings.SporifyDbContextOptions.Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
    }
}