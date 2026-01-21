namespace SB.Domain;

using RazorLight;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

internal class HtmlTemplateService : IHtmlTemplateService
{
    private static Lazy<RazorLightEngine> RazorLightEngine =
        new(
            () =>
                new RazorLightEngineBuilder()
                    .UseEmbeddedResourcesProject(typeof(DomainModule).Assembly, "SB.Domain.HtmlTemplates.Templates")
                    .UseMemoryCachingProvider()
                    .Build(),
            LazyThreadSafetyMode.ExecutionAndPublication);

    public async Task RenderAsync<TModel>(string templateName, TModel model, TextWriter textWriter, CancellationToken ct)
    {
        string templateFileName = HtmlTemplateConfig.Get(templateName).TemplateFileName;

        ITemplatePage template = await RazorLightEngine.Value.CompileTemplateAsync(templateFileName);
        await RazorLightEngine.Value.RenderTemplateAsync(template, model, textWriter);
    }
}
