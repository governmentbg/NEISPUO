namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IHtmlTemplateService
{
    Task RenderAsync<TModel>(string templateName, TModel model, TextWriter textWriter, CancellationToken ct);
}
