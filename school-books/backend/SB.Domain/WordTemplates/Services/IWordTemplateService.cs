namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IWordTemplateService
{
    Task TransformAsync(string templateName, string jsonContext, Stream outputStream, bool highlightContentControls, CancellationToken ct);
}
