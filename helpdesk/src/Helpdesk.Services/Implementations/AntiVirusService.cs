using Helpdesk.DataAccess;
using Helpdesk.Models.Configuration;
using Helpdesk.Services.Interfaces;
using Helpdesk.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using nClam;
using System;
using System.Threading.Tasks;

namespace Helpdesk.Services.Implementations
{

    /// <summary>
    /// Връзка с AntiVirus услугата
    /// </summary>
    public class AntiVirusService : BaseService, IAntiVirusService
    {
        private readonly AntiVirusConfig _config;

        public AntiVirusService(HelpdeskContext context, IUserInfo userInfo,
            ILogger<AntiVirusService> logger, IOptions<AntiVirusConfig> config)
            : base(context, userInfo, logger)
        {
            _config = config.Value;
        }

        public async Task<ClamScanResult> ScanAsync(byte[] contents)
        {
            try
            {
                if (_config.Enabled && (contents.Length <= _config.MaxSize))
                {
                    var client = new ClamClient(_config.Host, _config.Port);
                    var result = await client.SendAndScanFileAsync(contents);
                    return result;
                }
                else
                {
                    return new ClamScanResult(string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("При антивирусна проверка на файл настъпи грешка.", ex);
                return new ClamScanResult(string.Empty);
            }
        }
    }
}
