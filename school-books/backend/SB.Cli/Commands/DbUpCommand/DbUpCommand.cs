namespace SB.Cli;

using System;
using System.Linq;
using System.Threading;
using Autofac;
using DbUp;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
using SB.Data;

public class DbUpCommand : ICommand
{
    public string Name { get; } = "dbup";

    public void Configure(CommandLineApplication app, CancellationToken stopped)
    {
        var runOpt = app.Option("-r | --run", "Perform the upgrade instead of a dry run", CommandOptionType.NoValue);

        app.OnExecute(() =>
        {
            using var container = AutofacSetup.ConfigureServices();
            using var lifetimeScope = container.BeginLifetimeScope();
            var successful = this.Update(lifetimeScope, runOpt.HasValue(), stopped);
            return successful ? 0 : 1;
        });
    }

    private bool Update(ILifetimeScope lifetimeScope, bool run, CancellationToken ct)
    {
        var options = lifetimeScope.Resolve<IOptions<DataOptions>>().Value;
        var connectionString = options.GetConnectionString();

        var upgradeEngineBuilder = DeployChanges.To
            .SqlDatabase(connectionString)
            .LogTo(new CustomConsoleUpgradeLog())
            .JournalToSqlTable("school_books", "UpdateScript")
            .WithScripts(new CustomEmbeddedScriptProvider())
            .WithExecutionTimeout(TimeSpan.FromMinutes(30))
            .WithTransaction();

        var upgradeEngine = upgradeEngineBuilder.Build();

        if (run)
        {
            var result = upgradeEngine.PerformUpgrade();
            return result.Successful;
        }
        else
        {
            var scripts = upgradeEngine.GetScriptsToExecute().Select(s => s.Name);
            if (scripts.Any())
            {
                Console.WriteLine("The following scripts would be executed:");
                foreach (var script in scripts)
                {
                    Console.WriteLine($"    {script}");
                }
                Console.WriteLine("To perform the upgrade run the command with '--run'");
            }
            else
            {
                Console.WriteLine("No scripts to execute.");
            }
            return true;
        }
    }
}
