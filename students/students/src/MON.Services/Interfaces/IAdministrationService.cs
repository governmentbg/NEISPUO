namespace MON.Services.Interfaces
{
    using MON.DataAccess;
    using MON.DataAccess.Dto;
    using MON.Models;
    using MON.Models.Administration;
    using MON.Models.Grid;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAdministrationService
    {
        Task<IPagedList<VAuditLog>> GetAuditLogs(LogsListInput input);
        public Task<string> GetDBDgml();
        Task<Dictionary<string, string>> ContextualInformation(string moduleName);
        Task<IPagedList<ContextualInformation>> ContextualInformationList(ContextualInfoListInput input);
        Task UpdateContextualInformation(ContextualInformation model);
        Task<IPagedList<PermissionDocumentationModel>> GetPermissionDocumentations(PagedListInput input);
        Task CreatePermissionDocumentation(PermissionDocumentationModel model);
        Task UpdatePermissionDocumentation(PermissionDocumentationModel model);
        Task<ContextualInformation> GetContextualInformationByKey(string key);
        Task<string> GetTenantAppSetting(string key);
        Task SetTenantAppSetting(AppSettingModel model);
        Task<List<string>> GetCacheKeys();
        Task<object> GetCacheKeyValue(string cacheKey);
        Task<object> GetCacheKeyFull(string cacheKey);
        Task<object> GetCacheServerInfo();
        Task ClearCache();
        Task ClearCacheKey(string cacheKey);

        Task<DataReferencesViewModel> GetDataReferences(string schemaName, string tableName, string entityId, int? top, bool? onlyWithDependecies);
        Task<IPagedList<UnsignedStudentLodListDto>> GetUnsignedStudentLodList(UnsignedStudentLodListInput input);
        Task FinalizeLods(LodFinalizationAdministrationModel model, CancellationToken cancellationToken);
    }
}
