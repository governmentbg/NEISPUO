using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IAppConfigurationService
    {
        Task<string> GetValueByKey(string key);
        Task DeleteValueByKey(string key);
        Task AddOrUpdate(string key, string contents);
    }
}
