using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleAppTemplateExampleWithSerilog
{
    internal class ConsoleRunner
    {
        private readonly MyService _service;
        private readonly AppOptions _options;
        private readonly ILogger<ConsoleRunner> _logger;

        public ConsoleRunner(IOptions<AppOptions> options, MyService service, ILogger<ConsoleRunner> logger)
        {
            _service = service;
            _options = options.Value;
            _logger = logger;
        }

        public Task ExecuteAsync()
        {
            Console.WriteLine($"Starting '{_options.ApplicationName}' App...");
            _logger.LogInformation("Starting ExecuteAsync ... ");

            _service.DoSomething();

            Console.ReadKey();

            Console.WriteLine("Application Stopped.");
            _logger.LogInformation("Ending ExecuteAsync ... ");

            return Task.CompletedTask;
        }
    }
}
