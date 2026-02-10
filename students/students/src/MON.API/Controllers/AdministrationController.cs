namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.DataAccess;
    using MON.DataAccess.Dto;
    using MON.Models;
    using MON.Models.Administration;
    using MON.Models.Grid;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    [Authorize]
    public class AdministrationController : BaseApiController
    {
        private readonly IAdministrationService _service;

        public AdministrationController(IAdministrationService service, ILogger<AdministrationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForAuditLogsShow)]
        public async Task<ActionResult<IPagedList<VAuditLog>>> GetAuditLogs([FromQuery] LogsListInput input)
        {
            return Ok(await _service.GetAuditLogs(input));
        }

        [HttpGet]
        public async Task<Dictionary<string, string>> ContextualInformation()
        {
            return await _service.ContextualInformation("students");
        }

        /// <summary>
        /// Creates a DGML class diagram of most of the entities in the project wher you go to localhost/dgml
        /// See https://github.com/ErikEJ/SqlCeToolbox/wiki/EF-Core-Power-Tools
        /// </summary>
        /// <returns>a DGML class diagram</returns>
        [HttpGet]
        public async Task<IActionResult> DBDgml()
        {

            var response = File(Encoding.UTF8.GetBytes(await _service.GetDBDgml()), "application/octet-stream", "DB.dgml");
            return response;
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForContextualInformationManage)]
        public async Task<ActionResult<IPagedList<ContextualInformation>>> ContextualInformationList([FromQuery] ContextualInfoListInput input)
        {
            return Ok(await _service.ContextualInformationList(input));
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForContextualInformationManage)]
        public async Task<ContextualInformation> GetContextualInformationByKey(string key)
        {
            return await _service.GetContextualInformationByKey(key);
        }

        [HttpPut]
        [Authorize(Policy = DefaultPermissions.PermissionNameForContextualInformationManage)]
        public async Task UpdateContextualInformation(ContextualInformation model)
        {
            await _service.UpdateContextualInformation(model);
        }

        [HttpGet]
        public async Task<IPagedList<PermissionDocumentationModel>> GetPermissionDocumentations([FromQuery] PagedListInput input)
        {
            return await _service.GetPermissionDocumentations(input);
        }

        [HttpPut]
        [Authorize(Policy = DefaultPermissions.PermissionNameForContextualInformationManage)]
        public async Task UpdatePermissionDocumentation(PermissionDocumentationModel model)
        {
            await _service.UpdatePermissionDocumentation(model);
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForTenantAppSettingsManage)]
        public async Task<IActionResult> GetTenantAppSetting(string key)
        {
            return Ok(await _service.GetTenantAppSetting(key));
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForTenantAppSettingsManage)]
        public async Task<IActionResult> SetTenantAppSetting(AppSettingModel model)
        {
            await _service.SetTenantAppSetting(model);

            return Ok();
        }

        [HttpGet]
        public async Task<List<string>> GetCacheKeys()
        {
            return await _service.GetCacheKeys();
        }

        [HttpGet]
        public async Task<IActionResult> GetCacheKeyValue(string cacheKey)
        {
            return Ok(await _service.GetCacheKeyValue(cacheKey));
        }

        [HttpGet]
        public async Task<IActionResult> GetCacheServerInfo()
        {
            return Ok(await _service.GetCacheServerInfo());
        }

        [HttpGet]
        public async Task<IActionResult> GetCacheKeyFull(string cacheKey)
        {
            return Ok(await _service.GetCacheKeyFull(cacheKey));
        }

        [HttpPost]
        public async Task<IActionResult> ClearCache()
        {
            await _service.ClearCache();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ClearCacheKey([FromQuery] string cacheKey)
        {
            await _service.ClearCacheKey(cacheKey);

            return Ok();
        }

        [HttpGet]
        public async Task<DataReferencesViewModel> GetDataReferences(string schemaName, string tableName, string entityId, int? top, bool? onlyWithDependecies)
        {
            return await _service.GetDataReferences(schemaName, tableName, entityId, top, onlyWithDependecies);
        }


        [HttpGet]
        public async Task<IPagedList<UnsignedStudentLodListDto>> GetUnsignedStudentLodList([FromQuery] UnsignedStudentLodListInput input)
        {
            return await _service.GetUnsignedStudentLodList(input);
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForLodFinalizationAdministration)]
        public async Task<IActionResult> FinalizeLods(LodFinalizationAdministrationModel model, CancellationToken cancellationToken)
        {
            await _service.FinalizeLods(model, cancellationToken);

            return Ok();
        }
    }
}
