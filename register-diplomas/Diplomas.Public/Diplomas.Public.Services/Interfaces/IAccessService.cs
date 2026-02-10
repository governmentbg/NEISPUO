using Diplomas.Public.Models.Access;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Diplomas.Public.Services.Interfaces
{
    public interface IAccessService
    {
        Task<int> Add(ExtAccessModel model);
    }
}
