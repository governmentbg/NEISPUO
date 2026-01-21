namespace MonProjects.Services.Logs
{
    using System.Threading.Tasks;

    public interface ILogService
    {
        Task<int> CreateLogAsync(string certValue);
    }
}
