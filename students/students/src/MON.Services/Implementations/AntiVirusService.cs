using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MON.Models.Configuration;
using MON.Services.Interfaces;
using nClam;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Implementations
{
    /// <summary>
    /// Връзка с AntiVirus услугата
    /// </summary>
    public class AntiVirusService : BaseService<AntiVirusService>, IAntiVirusService
    {
        private readonly AntiVirusConfig _config;

        public AntiVirusService(DbServiceDependencies<AntiVirusService> dependencies,
            IOptions<AntiVirusConfig> config)
            : base(dependencies)
        {
            _config = config.Value;
        }

        public async Task<ClamScanResult> ScanAsync(byte[] contents, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_config.Enabled && (contents.Length <= _config.MaxSize))
                {
                    var client = new ClamClient(_config.Host, _config.Port);
                    var result = await client.SendAndScanFileAsync(contents, cancellationToken);
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
