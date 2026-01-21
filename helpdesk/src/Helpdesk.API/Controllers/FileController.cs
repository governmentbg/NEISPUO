namespace Helpdesk.API.Controllers
{
    using Helpdesk.Models.Configuration;
    using Helpdesk.Services.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class FileController : BaseApiController
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;

        public FileController(IBlobService blobService, IOptions<BlobServiceConfig> blobServiceConfig)
        {
            _blobService = blobService; 
            _blobServiceConfig = blobServiceConfig.Value;
        }

        [HttpPost]
        public async Task<string> Upload(List<IFormFile> file)
        {
            if (file != null && file.Count() == 1) {
                var uploadedFile = file.First();
                long length = uploadedFile.Length;
                using var fileStream = uploadedFile.OpenReadStream();
                byte[] bytes = new byte[length];
                fileStream.Read(bytes, 0, (int)uploadedFile.Length);
                var result = await _blobService.UploadFileAsync(bytes, uploadedFile.FileName, uploadedFile.ContentType);
                if (!result.IsError)
                {
                    return await Task.FromResult<String>($"{result.Data.BlobId}");
                }
                else
                {
                    return $"{result.Message}";
                }
            }
            else
            {
                return "";
            }
        }

    }
}
