namespace SB.Cli;

using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Extensions.CommandLineUtils;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;
using SB.ExtApi.IntegrationTests;

public class GenerateExtApiTestClientCommand : ICommand
{
    private readonly string DefaultSwaggerUrl = "http://localhost:5200/swagger/v1/swagger.json";

    public string Name { get; } = "generate-extapi-test-client";

    public void Configure(CommandLineApplication app, CancellationToken stopped)
    {
        app.OnExecute(async () =>
        {
            try
            {
                using var extApiWebApplicationFactory = new ExtApiWebApplicationFactory();
                using HttpClient httpClient = extApiWebApplicationFactory.CreateClient();
                HttpResponseMessage response = await httpClient.GetAsync(DefaultSwaggerUrl);
                response.EnsureSuccessStatusCode();
                string swaggerJson = await response.Content.ReadAsStringAsync();

                var document = await OpenApiDocument.FromJsonAsync(swaggerJson);

                var settings = new CSharpClientGeneratorSettings
                {
                    ClassName = "ExtApiTestClient",
                    CSharpGeneratorSettings =
                    {
                        Namespace = "SB.ExtApi.IntegrationTests"
                    },
                    OperationNameGenerator = new SingleClientFromPathSegmentsOperationNameGenerator()
                };

                var generator = new CSharpClientGenerator(document, settings);
                var code = generator.GenerateFile();

                await File.WriteAllTextAsync(
                    "../SB.ExtApi.IntegrationTests/Utils/ExtApiTestClient.cs",
                    code,
                    Encoding.UTF8,
                    stopped);
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
}
