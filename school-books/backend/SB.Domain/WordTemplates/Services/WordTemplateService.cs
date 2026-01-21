namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.FileProviders;

class WordTemplateService : IWordTemplateService
{
    private IFileProvider embeddedFileProvider;

    public WordTemplateService(
        [KeyFilter(DomainModule.DomainEmbeddedFileProviderRegistrationName)]IFileProvider embeddedFileProvider)
    {
        this.embeddedFileProvider = embeddedFileProvider;
    }

    public async Task TransformAsync(string templateName, string jsonContext, Stream outputStream, bool highlightContentControls, CancellationToken ct)
    {
        var templateConfig = WordTemplateConfig.Get(templateName);
        using var templateStream = this.embeddedFileProvider
            .GetFileInfo($@"WordTemplates/Templates/{templateConfig.TemplateFileName}")
            .CreateReadStream();

        await templateStream.CopyToAsync(outputStream, ct);

        WordTemplateTransformer.Transform(outputStream, jsonContext, highlightContentControls);
    }
}
