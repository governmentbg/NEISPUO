using MON.Models.Identity;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface INeispuoAuthenticationService
    {
        Task<UserInfoViewModel> GetUserInfo();
    }
}
