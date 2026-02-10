namespace MonProjects.Services.ExtSystem
{
    using Services.Models.ExtSystem;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IExtSystemServices
    {
        Task<IEnumerable<ServiceDataDapperModel>> GetServicesAsync(int extSystemId);
    }
}
