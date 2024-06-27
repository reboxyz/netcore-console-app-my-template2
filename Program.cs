// Note! Source URL: https://mehdihadeli.com/console-dependency-injection/


using ConsoleAppTemplateExampleWithSerilog.Extensions;
using ConsoleAppTemplateExampleWithSerilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

// Approach: Using the HostBuilder class Without Background Worker
// https://github.com/serilog/serilog-aspnetcore#two-stage-initialization

Log.Logger = new LoggerConfiguration().MinimumLevel
    .Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console()
    .CreateBootstrapLogger();

// Load some envs like `ASPNETCORE_ENVIRONMENT` for accessing in `HostingEnvironment`, default is Production
DotNetEnv.Env.TraversePath().Load();

try
{
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host#default-builder-settings
    var hostBuilder = Host.CreateDefaultBuilder(args);

    hostBuilder
        /*
         .ConfigureLogging(logging =>
         {
             logging.AddConsole();
             logging.AddDebug();
         })
        */
        .UseSerilog(
            (context, sp, loggerConfiguration) =>
            {
                loggerConfiguration.Enrich
                    .WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .ReadFrom.Configuration(context.Configuration, sectionName: "Serilog")
                    .WriteTo.Console(
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} - {Message:lj}{NewLine}{Exception}"
                    )
                   .WriteTo.File("Serilogs\\AppLogs.log", rollingInterval: RollingInterval.Day)
                   ;
            }
        )
        
        // setup configurations - CreateDefaultBuilder do this for us, but we can override that configuration
        .ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile(
                "appsettings.json",
                optional: true,
                reloadOnChange: true
            );
            configurationBuilder.AddEnvironmentVariables();
            configurationBuilder.AddCommandLine(args);
        })
        .ConfigureServices(
            (hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                var environment = hostContext.HostingEnvironment;
                var appOptions = configuration.GetSection("AppOptions").Get<AppOptions>();

                // setup dependencies
                services.AddOptions<AppOptions>().BindConfiguration(nameof(AppOptions));
                services.AddSingleton<MyService>();
                services.AddSingleton<ConsoleRunner>();
            }
        );

    // build our HostBuilder to IHost
    var host = hostBuilder.Build(); 

    // run our console app
    //await host.ExecuteConsoleRunner();  // Note! This does not exit the program automatically
    // Or
    await AppConsoleRunner.RunAsync(host.Services);  // Note! This exits the program automatically
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}