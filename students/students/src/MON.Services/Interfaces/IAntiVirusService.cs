namespace MON.Services.Interfaces
{
    using nClam;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAntiVirusService
    {
        Task<ClamScanResult> ScanAsync(byte[] contents, CancellationToken cancellationToken);
    }
}
