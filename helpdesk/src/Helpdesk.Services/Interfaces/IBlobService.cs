namespace Helpdesk.Services.Interfaces
{
    using Helpdesk.Models;
    using System.Threading.Tasks;

    public interface IBlobService
    {
        Task<ResultModel<BlobDO>> UploadFileAsync(byte[] Contents, string fileName, string fileType);
    }
}
