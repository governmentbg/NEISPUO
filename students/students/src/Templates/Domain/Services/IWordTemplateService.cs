using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Domain
{
    public interface IWordTemplateService
    {
        Task<WordTemplateConfig> TransformAsync(string templateName, string jsonContext, Stream outputStream, CancellationToken ct);
        Task<(string fileName, byte[] fileContents, string contentType)> GenerateWordDocument<T>(T reportModel, string templateName, CancellationToken cancellationToken) where T : class;
    }
}
