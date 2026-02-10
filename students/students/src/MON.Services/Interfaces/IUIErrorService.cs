namespace MON.Services.Interfaces
{
    using MON.Models;
    using System.Threading.Tasks;

    public interface IUIErrorService
    {
        Task<int> Add(ErrorModel model);
    }
}
