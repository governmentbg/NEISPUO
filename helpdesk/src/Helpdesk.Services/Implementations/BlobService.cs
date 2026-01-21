namespace Helpdesk.Services.Implementations
{
    using Helpdesk.DataAccess;
    using Helpdesk.Models;
    using Helpdesk.Models.Configuration;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared.Interfaces;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using nClam;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class BlobService : BaseService, IBlobService
    {
        private readonly BlobServiceConfig _config;
        private readonly IAntiVirusService _antiVirusService;

        public BlobService(HelpdeskContext context,
            IUserInfo userInfo,
            ILogger<BlobService> logger,
            IAntiVirusService antiVirusService,
            IOptions<BlobServiceConfig> config)
            : base(context, userInfo, logger)
        {
            _config = config.Value;
            _antiVirusService = antiVirusService;
        }
        public async Task<ResultModel<BlobDO>> UploadFileAsync(byte[] Contents, string fileName, string fileType)
        {
            var antiVirusResult = await _antiVirusService.ScanAsync(Contents);
            if (antiVirusResult.Result == ClamScanResults.VirusDetected)
            {
                _logger.LogError($"Открит вирус: {antiVirusResult.RawResult} във файл [{fileName}] [{fileType}] изпратен от потребител [{_userInfo?.SysUserID}]");
                return new ErrorResultModel<BlobDO>(null, $"Във файл {fileName} е открит вирус {antiVirusResult.RawResult}");
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

            var result = await client.PostAsync(_config.Url, content);
            if (result.IsSuccessStatusCode)
            {
                return new OKResultModel<BlobDO>(JsonConvert.DeserializeObject<BlobDO>(await result.Content.ReadAsStringAsync()));
            }
            else
            {
                return null;
            }
        }
    }
}
