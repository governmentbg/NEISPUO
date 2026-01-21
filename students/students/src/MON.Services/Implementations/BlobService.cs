using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MON.DataAccess;
using MON.Models;
using MON.Models.Blob;
using MON.Models.Configuration;
using MON.Services.Interfaces;
using nClam;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Implementations
{
    /// <summary>
    /// Връзка с BlobService услугата
    /// </summary>
    public class BlobService : BaseService<BlobService>, IBlobService
    {
        private readonly BlobServiceConfig _config;
        private readonly IAntiVirusService _antiVirusService;

        public BlobService(DbServiceDependencies<BlobService> dependencies,
            IAntiVirusService antiVirusService, IOptions<BlobServiceConfig> config)
            : base(dependencies)
        {
            _config = config.Value;
            _antiVirusService = antiVirusService;
        }

        /// <summary>
        /// Изпращане на файл към blob service-a
        /// </summary>
        /// <param name="Contents"></param>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public async Task<ResultModel<BlobDO>> UploadFileAsync(byte[] Contents, string fileName, string fileType, CancellationToken cancellationToken = default)
        {
            var antiVirusResult = await _antiVirusService.ScanAsync(Contents, cancellationToken);
            if (antiVirusResult.Result == ClamScanResults.VirusDetected)
            {
                _logger.LogError($"Открит вирус: {antiVirusResult.RawResult} във файл [{fileName}] [{fileType}] изпратен от потребител [{_userInfo?.SysUserID}]");
                return new ErrorResultModel<BlobDO>(null, $"Открит вирус: {antiVirusResult.RawResult}");
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", _userInfo.AccessToken);

            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(Contents);

            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                FileName = fileName
            };
            content.Add(fileContent);

            HttpResponseMessage result = await client.PostAsync(_config.Url, content, cancellationToken);
            if (result.IsSuccessStatusCode)
            {
                return new OKResultModel<BlobDO>(JsonConvert.DeserializeObject<BlobDO>(await result.Content.ReadAsStringAsync()));
            }
            else
            {
                _logger.LogError(JsonConvert.SerializeObject(result));
                return null;
            }
        }

        public async Task<Document> UploadDocument(DocumentModel model, CancellationToken cancelationtoken = default)
        {
            try
            {
                if (model == null) return null;
                var result = await UploadFileAsync(model.NoteContents, model.NoteFileName, model.NoteFileType, cancelationtoken);
                var doc = new Document
                {
                    ContentType = model.NoteFileType,
                    FileName = model.NoteFileName,
                    BlobId = result.Data.BlobId
                };

                return doc;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while uploading document for model:{model}", ex);

                throw;
            }

        }

        public async Task<byte[]> DownloadByteArrayAsync(IBlobDownloadable blob, CancellationToken cancelation = default)
        {
            if (blob == null)
            {
                throw new ArgumentNullException(nameof(blob), nameof(IBlobDownloadable));
            }

            string url = $"{_config.Url}/{blob.BlobId}?t={blob.UnixTimeSeconds}&h={blob.Hmac}";
            using var client = new HttpClient();
            return await client.GetByteArrayAsync(url);
        }

        public async Task<System.IO.Stream> DownloadStreamAsync(IBlobDownloadable blob, CancellationToken cancellationToken = default)
        {
            if (blob == null)
            {
                throw new ArgumentNullException(nameof(blob), nameof(IBlobDownloadable));
            }

            string url = $"{_config.Url}/{blob.BlobId}?t={blob.UnixTimeSeconds}&h={blob.Hmac}";
            var client = new HttpClient();
            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
