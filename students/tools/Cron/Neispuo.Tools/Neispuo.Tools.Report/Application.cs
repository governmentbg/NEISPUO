using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neispuo.Tools.DataAccess;
using Neispuo.Tools.Services.Implementations;
using Neispuo.Tools.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neispuo.Tools.Report
{
    internal class Application
    {
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IStatsService _statsService;
        private readonly TaskService _taskService;
        private readonly NeispuoContext _context;
        private readonly IHostEnvironment _hostEnvironment;

        public Application(NeispuoContext context, ILogger<Application> logger, IEmailService emailService, IConfiguration configuration, IStatsService statsService, TaskService taskService, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
            _context = context;
            _statsService = statsService;
            _taskService = taskService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task RunAsync(string[] args)
        {
            //string environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            //Console.Out.WriteLine($"DOTNET_ENVIRONMENT: {environmentName}, Environment {_hostEnvironment.EnvironmentName}");

            Option<int> periodOption = new("--period")
            {
                Description = "Период в минути",
                DefaultValueFactory = parseResult => 24 * 60 // 24 часа по подразбиране
            };

            Command statsCommand = new(
                       name: "stats",
                       description: "Генерира общи статистики за системата")
                    {
                        periodOption
                    };


            Command processCommand = new(name: "process", description: "Обработва необработени задачи");
            Command executeCommand = new(name: "execute", description: "Изпълнява задачи");

            Command taskCommand = new(
                       name: "task",
                       description: "Обработва и изпълнява задачи");
            taskCommand.Add(processCommand);
            taskCommand.Add(executeCommand);


            var rootCommand = new RootCommand("Генератор на отчети за НЕИСПУО");
            rootCommand.Subcommands.Add(statsCommand);
            rootCommand.Subcommands.Add(taskCommand);

            statsCommand.SetAction(async (parseResult) =>
            {
                await _statsService.GenerateCommonStatsAsync(parseResult.GetValue(periodOption));
            });

            processCommand.SetAction(async (parseResult) =>
            {
                _logger.LogInformation("Обработване на необработени задачи...");

                await _taskService.ProcessTasksAsync();
            });

            executeCommand.SetAction(async (parseResult) =>
            {
                _logger.LogInformation("Изпълнение на задачи в статус Pending...");

                await _taskService.ExecuteTasksAsync();
            });
            await rootCommand.Parse(args).InvokeAsync();
        }
    }
}
