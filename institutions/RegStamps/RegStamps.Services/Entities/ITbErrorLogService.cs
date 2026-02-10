using HandlebarsDotNet;
using System;

namespace RegStamps.Services.Entities
{
    public interface ITbErrorLogService
    {
        Task<int> CreateLogAsync(int schoolId, string url, string actionName, string controllerName, Exception exception);
    }
}
