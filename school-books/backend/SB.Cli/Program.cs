namespace SB.Cli;

using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Globalization;
using System.Text;
using System.Threading;

class Program
{
    public static int Main(string[] args)
    {
        // set default culture to bg-BG
        var cultureInfo = new CultureInfo("bg-BG");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        using CancellationTokenSource cts = new();

        Console.OutputEncoding = Encoding.UTF8;
        Console.CancelKeyPress += (s, e) => {
            e.Cancel = true;
            cts.Cancel();
        };

        var app = new CommandLineApplication(throwOnUnexpectedArg: false);

        void addCommand(ICommand cmd) => app.Command(cmd.Name, (subApp) => cmd.Configure(subApp, cts.Token));

        addCommand(new WordTemplateCommand());
        addCommand(new HtmlTemplateCommand());
        addCommand(new GenerateExtApiTestClientCommand());
        addCommand(new DbUpCommand());

        app.OnExecute(() =>
        {
            app.ShowHelp();
            return 2;
        });

        return app.Execute(args);
    }
}
