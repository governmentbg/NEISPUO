namespace Helpdesk.Services.Interfaces
{
    using nClam;
    using System.Threading.Tasks;

    public interface IAntiVirusService
    {
        Task<ClamScanResult> ScanAsync(byte[] contents);
    }
}