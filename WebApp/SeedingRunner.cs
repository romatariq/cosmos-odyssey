using DAL;
using Microsoft.EntityFrameworkCore;

namespace WebApp;

public static class SeedingRunner
{
    public static async Task SetupDb(IApplicationBuilder webApp, IConfiguration appConfiguration)
    {
        using var serviceScope = webApp.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
        await using var context = serviceScope.ServiceProvider.GetService<AppDbContext>();


        if (context == null)
        {
            throw new ApplicationException("Problem in services. Can't initialize Application DB Context");
        }

        var logger = serviceScope.ServiceProvider.GetService<ILogger<IApplicationBuilder>>();
        if (logger == null)
        {
            throw new ApplicationException("Problem in services. Can't initialize logger");
        }


        // wait for db connection
        var startedAt = DateTime.UtcNow;
        var isDbConnectable = context.Database.CanConnectAsync().Result;
        while (!isDbConnectable)
        {
            isDbConnectable = context.Database.CanConnectAsync().Result;
            if (!isDbConnectable && (DateTime.UtcNow - startedAt).Seconds > 5)
            {
                break;
            }
        }

        if (appConfiguration.GetValue<bool>("InitializeData:DropDatabase"))
        {
            logger.LogWarning("Dropping database");
            await context.Database.EnsureDeletedAsync();
        }

        if (appConfiguration.GetValue<bool>("InitializeData:MigrateDatabase"))
        {
            logger.LogInformation("Migrating database");
            await context.Database.MigrateAsync();
        }
    }
}