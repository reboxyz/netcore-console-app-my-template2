using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace ConsoleAppTemplateExampleWithSerilog
{
    internal static class AppConsoleRunner
    {
        public static Task RunAsync(IServiceProvider serviceProvider)
        {
            var appOptions = serviceProvider.GetRequiredService<IOptions<AppOptions>>();
            Console.WriteLine($"Starting '{appOptions.Value.ApplicationName}' App...");
            var myService = serviceProvider.GetRequiredService<MyService>();
                myService.DoSomething();

            Console.WriteLine("Application Stopped.");

            Console.ReadKey();

            return Task.CompletedTask;
        }
    }
}
