namespace SB.Cli;

using Autofac;
using Microsoft.Extensions.CommandLineUtils;
using SB.Domain;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public class WordTemplateCommand : ICommand
{
    public string Name { get; } = "word-template";

    public void Configure(CommandLineApplication app, CancellationToken stopped)
    {
        var templateArg = app.Argument("template", "a template to open");

        app.OnExecute(async () =>
        {
            string template = templateArg.Value;

            if (string.IsNullOrEmpty(template) ||
                !WordTemplateConfig.AllTemplates.ContainsKey(template))
            {
                if (!string.IsNullOrEmpty(template) &&
                    !WordTemplateConfig.AllTemplates.ContainsKey(template))
                {
                    Console.WriteLine($"There is no such template \"{template}\"");
                    Console.WriteLine();
                }

                Console.WriteLine($"Templates:");
                foreach (var t in WordTemplateConfig.AllTemplates)
                {
                    Console.WriteLine(t.Value.TemplateName);
                }

                return 0;
            }

            try
            {
                using var container = AutofacSetup.ConfigureServices();
                using var lifetimeScope = container.BeginLifetimeScope();
                var wordTemplateService = lifetimeScope.Resolve<IWordTemplateService>();

                var file = GetTempFileName(".docx");
                using (var fs = new FileStream(file, FileMode.CreateNew))
                {
                    await wordTemplateService.TransformAsync(
                        template,
                        WordTemplateConfig.Get(template).JsonSampleModel,
                        fs,
                        false,
                        stopped);
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = file,
                    UseShellExecute = true
                });
            }
            catch
            {
                if (!stopped.IsCancellationRequested)
                {
                    throw;
                }
            }

            return 0;
        });
    }

    public static string GetTempFileName(string extension)
    {
        return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + extension);
    }
}
