using Diplomas.Public.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Diplomas.Public.Services.Interfaces
{
    public interface IUIErrorService
    {
        Task<int> Add(ErrorModel model);
    }
}
