﻿using Microsoft.Extensions.Logging;

namespace ConsoleAppTemplateExampleWithSerilog
{
    internal class MyService
    {
        private readonly ILogger<MyService> _logger;

        public MyService(ILogger<MyService> logger)
        {
            _logger = logger;
        }

        public void DoSomething()
        {
            _logger.LogInformation("Doing something...");
        }
    }
}
