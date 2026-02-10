using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Security
{
    public interface IPermissionGenerator
    {
        Task<HashSet<string>> GetUserAllowPermissions(int? entityId);

        Task<HashSet<string>> GetUserDenyPermissions(int? entityId);

    }
}
