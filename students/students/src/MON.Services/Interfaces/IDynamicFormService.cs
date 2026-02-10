using MON.Models;
using MON.Models.Dynamic;
using MON.Models.Grid;
using MON.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IDynamicFormService
    {
        Task<DynamicEntities> GetEntitiesJsonDescription();
        Task<List<DropdownViewModel>> GetEntityTypesDropdowns();
        Task<IPagedList<string>> List(DynamicEntitiesListInput input);
        Task<List<DynamicGridHeader>> GetGridHeaders(string entityTypeName);
        Task<string> GetEntityModel(string entityTypeName, string entityId);
        string GetJsonSchema();
        Task Create(dynamic model);
        Task Update(dynamic model);
        Task Delete(dynamic model);
    }
}
