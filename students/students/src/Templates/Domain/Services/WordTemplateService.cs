using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Domain
{
    public class WordTemplateService : IWordTemplateService
    {
        private IFileProvider embeddedFileProvider;

        public WordTemplateService()
        {
            this.embeddedFileProvider = new ManifestEmbeddedFileProvider(typeof(WordTemplateService).GetTypeInfo().Assembly);
        }

        public async Task<(string fileName, byte[] fileContents, string contentType)> GenerateWordDocument<T>(T reportModel, string templateName, CancellationToken cancellationToken) where T : class
        {
            using var ms = new MemoryStream();
            WordTemplateConfig templateConfig = await TransformAsync(
                templateName,
                JsonSerializer.Serialize(reportModel),
                ms,
                cancellationToken);

            return (templateConfig.TemplateFileName, ms.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        public async Task<WordTemplateConfig> TransformAsync(string templateName, string jsonContext, Stream outputStream, CancellationToken ct)
        {
            var templateConfig = WordTemplateConfig.Get(templateName);
            using var templateStream = embeddedFileProvider
                .GetFileInfo($@"WordTemplates/Templates/{templateConfig.TemplateFileName}")
                .CreateReadStream();

            await templateStream.CopyToAsync(outputStream, ct);

            WordTemplateTransformer.Transform(outputStream, jsonContext);

            return templateConfig;
        }
    }
}
