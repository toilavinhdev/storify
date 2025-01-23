using Microsoft.EntityFrameworkCore;
using Storify.Core.Domain;
using Storify.Core.Extensions;
using Storify.Core.Services;

WebApplication.CreateBuilder(args).WithStorifyCore((services, appSettings) =>
{
    services.AddDbContext<StorifyDbContext>(options =>
    {
        options.UseNpgsql(appSettings.SporifyDbContextOptions.ConnectionString, builder =>
        {
            builder.MigrationsAssembly(typeof(Program).Assembly.GetName().FullName);
            builder.MigrationsHistoryTable(
                appSettings.SporifyDbContextOptions.MigrationsHistoryTable, 
                appSettings.SporifyDbContextOptions.Schema);
        });
    });
    services.AddTransient<IStorageService, StorageService>();
}, (app, appSettings) =>
{ 
    app.MapGet("/ping", () => Results.Ok("Hello World!"));
});