using MON.DataAccess;
using MON.Models;
using MON.Models.Blob;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IBlobService
    {
        Task<ResultModel<BlobDO>> UploadFileAsync(byte[] Contents, string fileName, string fileType, CancellationToken cancellationToken = default);
        Task<Document> UploadDocument(DocumentModel model, CancellationToken cancellationToken = default);
        Task<byte[]> DownloadByteArrayAsync(IBlobDownloadable blob, CancellationToken cancellationToken = default);
        Task<System.IO.Stream> DownloadStreamAsync(IBlobDownloadable blob, CancellationToken cancellationToken = default);
    }
}
