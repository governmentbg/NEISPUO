namespace SB.Cli;

using Autofac;
using Microsoft.Extensions.CommandLineUtils;
using SB.Domain;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class HtmlTemplateCommand : ICommand
{
    public string Name { get; } = "html-template";

    public void Configure(CommandLineApplication app, CancellationToken stopped)
    {
        var pdfOption = app.Option("--pdf", "covert the generated html to pdf with wkhtmltopdf", CommandOptionType.NoValue);
        var templateOrClassBookIdArg = app.Argument(
            "templateOrClassBookId",
            "a template or classBookId in schoolYear/classBookId format, e.g. 2022/123456");

        app.OnExecute(async () =>
        {
            string templateOrClassBookId = templateOrClassBookIdArg.Value;
            int schoolYear = 0;
            int classBookId = 0;

            if(string.IsNullOrEmpty(templateOrClassBookId))
            {
                app.ShowHelp();
                return 0;
            }

            if (!HtmlTemplateConfig.AllTemplates.ContainsKey(templateOrClassBookId) &&
                !(templateOrClassBookId.Split('/') is string[] parts &&
                    parts.Length == 2 &&
                    int.TryParse(parts[0], out schoolYear) &&
                    int.TryParse(parts[1], out classBookId)))
            {
                Console.WriteLine($"There is no such template \"{templateOrClassBookId}\" or argument does not match schoolYear/classBookId format");
                Console.WriteLine();

                Console.WriteLine($"Templates:");
                foreach (var t in HtmlTemplateConfig.AllTemplates)
                {
                    Console.WriteLine(t.Value.TemplateName);
                }

                return 0;
            }

            bool createPdf = pdfOption.HasValue();
            bool htmlCreated = false;
            bool pdfCreated = false;
            var htmlFile = GetTempFileName(".html");
            var pdfFile = $"{Path.Combine(Path.GetDirectoryName(htmlFile), Path.GetFileNameWithoutExtension(htmlFile))}.pdf";
            try
            {
                using var container = AutofacSetup.ConfigureServices();
                using var lifetimeScope = container.BeginLifetimeScope();
                var htmlTemplateService = lifetimeScope.Resolve<IHtmlTemplateService>();
                var classBookPrintHtmlService = lifetimeScope.Resolve<IPrintService>();

                using (FileStream fs = new(htmlFile, FileMode.CreateNew))
                using (StreamWriter sw = new(fs, Encoding.UTF8))
                {
                    htmlCreated = true;

                    if (schoolYear != 0 && classBookId != 0)
                    {
                        await classBookPrintHtmlService.RenderHtmlAsync(
                            JsonSerializer.Serialize(
                                new ClassBookPrintParams(
                                    schoolYear,
                                    classBookId)),
                            sw,
                            stopped);
                    }
                    else
                    {
                        await htmlTemplateService.RenderAsync(
                            templateOrClassBookId,
                            HtmlTemplateConfig.Get(templateOrClassBookId).SampleModel,
                            sw,
                            stopped);
                    }
                }

                if (createPdf)
                {
                    // transform the html to pdf by running our alpine-wkhtmltopdf container
                    var stdErrBuffer = new StringBuilder();
                    var result = await CliWrap.Cli.Wrap("docker")
                        .WithValidation(CliWrap.CommandResultValidation.None)
                        .WithArguments(args => args
                            .Add("run")
                            // docker run args
                            .Add("--rm")
                            .Add("-v")
                            .Add($"{Path.GetDirectoryName(htmlFile)}/:/local")
                            .Add("neispuoprod.azurecr.io/debian-wkhtmltopdf:0.12.6-bullseye-slim")
                            // wkhtmltopdf args
                            .Add("-L 0 -R 0 -B 0 -T 0 -s A4 --disable-smart-shrinking", false)
                            .Add($"/local/{Path.GetFileName(htmlFile)}")
                            .Add("-"))
                        .WithStandardOutputPipe(CliWrap.PipeTarget.ToFile(pdfFile))
                        .WithStandardErrorPipe(CliWrap.PipeTarget.ToStringBuilder(stdErrBuffer))
                        .ExecuteAsync(stopped);

                    if (result.ExitCode != 0)
                    {
                        throw new Exception($"Docker failed\n{stdErrBuffer}");
                    }

                    pdfCreated = true;
                }

                // Open the file by executing it.
                // For whatever reason the CliWrap throws an exception,
                // so using a simple Process.Start
                await Process.Start(new ProcessStartInfo
                {
                    FileName = createPdf ? pdfFile : htmlFile,
                    UseShellExecute = true
                }).WaitForExitAsync(stopped);

                // wait for 1sec for the pdf to open
                await Task.Delay(1000);
            }
            catch
            {
                if (!stopped.IsCancellationRequested)
                {
                    throw;
                }
            }
            finally
            {
                if (htmlCreated)
                {
                    File.Delete(htmlFile);
                }
                if (pdfCreated)
                {
                    File.Delete(pdfFile);
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
