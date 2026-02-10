namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using MON.DataAccess;
    using MON.DataAccess.Dto;
    using MON.Models;
    using MON.Models.Administration;
    using MON.Models.Grid;
    using MON.Models.StudentModels.Lod;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Interfaces;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class AdministrationService : BaseService<AdministrationService>, IAdministrationService
    {
        private const string ContextualInfoTag = "ContextualInformation";
        private const string PermissionsDocumentationAppSettingsKey = "PermissionsDocumentation";
        private readonly IAppConfigurationService _configurationService;
        private const string ModuleName = "students";
        private readonly ISignalRNotificationService _signalRNotificationService;
        private readonly ICacheService _cacheService;
        private readonly IStudentLODService _studentLODService;
        private readonly ILodFinalizationService _lodFinalizationService;

        public AdministrationService(DbServiceDependencies<AdministrationService> dependencies,
            IAppConfigurationService configurationService,
            ISignalRNotificationService signalRNotificationService,
            ICacheService cacheService,
            IStudentLODService studentLODService,
            ILodFinalizationService lodFinalizationService)
            : base(dependencies)
        {
            _configurationService = configurationService;
            _signalRNotificationService = signalRNotificationService;
            _cacheService = cacheService;
            _studentLODService = studentLODService;
            _lodFinalizationService= lodFinalizationService;
        }

        public async Task<IPagedList<VAuditLog>> GetAuditLogs(LogsListInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input), Messages.EmptyModelError);

            IQueryable<VAuditLog> query = _context.VAuditLogs
               .AsNoTracking();

            if (!_userInfo.IsMon)
            {
                query = query.Where(x => x.CreatedBySysUserId != null && x.CreatedBySysUserId == _userInfo.SysUserID);
            }

            query = query
                 .Where(!input.Filter.IsNullOrWhiteSpace(),
                    predicate => predicate.Username.Contains(input.Filter)
                    || predicate.EntitySetName.Contains(input.Filter)
                    || predicate.EntityTypeName.Contains(input.Filter)
                    || predicate.State.Contains(input.Filter)
                    || predicate.Ip.Contains(input.Filter)
                    || predicate.UserAgent.Contains(input.Filter)
                    || predicate.Username.Contains(input.Filter))
                 .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "TimestampUtc desc" : input.SortBy);

            int totalCount = await query.CountAsync();
            IList<VAuditLog> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<IPagedList<ContextualInformation>> ContextualInformationList(ContextualInfoListInput input)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForContextualInformationManage))
            {
                throw new UnauthorizedAccessException();
            }

            if (input == null)
                throw new ArgumentNullException(nameof(input), Messages.EmptyModelError);

            IQueryable<ContextualInformation> query = _context.ContextualInformations
                .AsNoTracking()
                .Where(x => input.ModuleName == null || x.ModuleName == input.ModuleName)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.Key.Contains(input.Filter)
                   || predicate.Description.Contains(input.Filter)
                   || predicate.ModuleName.Contains(input.Filter)
                   || predicate.Value.Contains(input.Filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Key asc" : input.SortBy);

            int totalCount = await query.CountAsync();
            IList<ContextualInformation> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<Dictionary<string, string>> ContextualInformation(string moduleName)
        {
            var query = _context.ContextualInformations
                .Where(x => x.ModuleName == moduleName)
                .Select(x => new { x.Key, x.Value })
                .FromCacheAsync(new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(15) }, ContextualInfoTag);

            return (await query)
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.First().Value);
        }

        public async Task UpdateContextualInformation(ContextualInformation model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForContextualInformationManage))
            {
                throw new UnauthorizedAccessException();
            }

            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            ContextualInformation entity = await _context.ContextualInformations
                .SingleOrDefaultAsync(x => x.ModuleName == model.ModuleName && x.Key == model.Key);

            if (entity != null)
            {
                entity.Value = model.Value;
                entity.Description = model.Description;
                await SaveAsync();
                QueryCacheManager.ExpireTag(ContextualInfoTag);

                await _signalRNotificationService.ContextualInformationReloaded(await ContextualInformation("students"));
            }
        }

        public Task<string> GetDBDgml()
        {
            return Task.FromResult(_context.AsDgml());
        }

        public async Task<IPagedList<PermissionDocumentationModel>> GetPermissionDocumentations(PagedListInput input)
        {
            PagedList<PermissionDocumentationModel> result = new PagedList<PermissionDocumentationModel>();
            List<PermissionDocumentationModel> models = await ReadPermissionDocumentationModels();

            if (models == null) return result;

            Dictionary<string, string> permissions = (await _authorizationService.GetAllPermissons())
                .ToDictionary(x => x.Key, x => x.Name);

            foreach (var model in models)
            {
                if (permissions.TryGetValue(model.PermissionName, out string permission))
                {
                    model.Permission = permission ?? "";
                }
            }

            IQueryable<PermissionDocumentationModel> query = models.AsQueryable()
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.Permission.Contains(input.Filter, StringComparison.OrdinalIgnoreCase)
                   || predicate.PermissionName.Contains(input.Filter, StringComparison.OrdinalIgnoreCase)
                   || predicate.Description.Contains(input.Filter, StringComparison.OrdinalIgnoreCase)
                   || predicate.Usage.Contains(input.Filter, StringComparison.OrdinalIgnoreCase))
                .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "PermissionName desc" : input.SortBy);

            int totalCount = models.Count;
            IList<PermissionDocumentationModel> items = query.PagedBy(input.PageIndex, input.PageSize).ToList();

            return items.ToPagedList(totalCount);
        }

        public async Task CreatePermissionDocumentation(PermissionDocumentationModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            List<PermissionDocumentationModel> source = await ReadPermissionDocumentationModels() ?? new List<PermissionDocumentationModel>();
            if (source.Any(x => x.PermissionName.Equals(model.PermissionName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"PermissionName: {model.PermissionName} aleready exists.");
            }

            source.Add(new PermissionDocumentationModel
            {
                PermissionName = model.PermissionName,
                Description = model.Description,
                Usage = model.Usage
            });

            string content = JsonSerializer.Serialize(source);
            await _context.AppSettings
                .Where(x => x.Key == PermissionsDocumentationAppSettingsKey)
                .UpdateAsync(x => new AppSetting { Value = content });
        }

        public async Task UpdatePermissionDocumentation(PermissionDocumentationModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            List<PermissionDocumentationModel> source = await ReadPermissionDocumentationModels();
            PermissionDocumentationModel entity = source?.FirstOrDefault(x => x.PermissionName == model.PermissionName) ?? throw new ArgumentNullException(nameof(source), "Source cant be null!");

            entity.Description = model.Description;
            entity.Usage = model.Usage;

            string content = JsonSerializer.Serialize(source);
            await _context.AppSettings
                .Where(x => x.Key == PermissionsDocumentationAppSettingsKey)
                .UpdateAsync(x => new AppSetting { Value = content });
        }

        private async Task<List<PermissionDocumentationModel>> ReadPermissionDocumentationModels()
        {
            string contents = await _configurationService.GetValueByKey(PermissionsDocumentationAppSettingsKey);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return string.IsNullOrWhiteSpace(contents)
                ? null
                : JsonSerializer.Deserialize<List<PermissionDocumentationModel>>(contents, options);
        }

        public Task<ContextualInformation> GetContextualInformationByKey(string key)
        {
            return _context.ContextualInformations
                .SingleOrDefaultAsync(x => x.ModuleName == ModuleName && x.Key == key);
        }

        public Task<string> GetTenantAppSetting(string key)
        {
            if (key.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(key));
            }

            int institutionId = _userInfo.InstitutionID ?? throw new ApiException(Messages.InvalidInstitutionCodeError);

            return _context.AppSettingsForTenants
                .Where(x => x.InstitutionId == institutionId && x.Key == key)
                .Select(x => x.Value)
                .SingleOrDefaultAsync();
        }

        public async Task SetTenantAppSetting(AppSettingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Key.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(model.Key));
            }

            int institutionId = _userInfo.InstitutionID ?? throw new ArgumentNullException(nameof(_userInfo.InstitutionID));
            AppSettingsForTenant appSetting = await _context.AppSettingsForTenants
                .Where(x => x.InstitutionId == institutionId && x.Key == model.Key)
                .SingleOrDefaultAsync();

            if (appSetting == null)
            {
                appSetting = new AppSettingsForTenant
                {
                    InstitutionId = institutionId,
                    Key = model.Key
                };
                _context.AppSettingsForTenants.Add(appSetting);
            }

            appSetting.Value = model.Value;

            await SaveAsync();
        }

        public async Task<List<string>> GetCacheKeys()
        {
            return await _cacheService.GetKeys();
        }

        public async Task<object> GetCacheKeyValue(string cacheKey)
        {
            return await _cacheService.GetAsync<object>(cacheKey);
        }

        public async Task<object> GetCacheKeyFull(string cacheKey)
        {
            return await _cacheService.GetFullAsync(cacheKey);
        }

        public async Task<object> GetCacheServerInfo()
        {
            return await _cacheService.GetCacheServerInfo();
        }

        //public async Task GetCacheKeyInfo(string cacheKey)
        //{

        //}

        public async Task ClearCache()
        {
            await _cacheService.ClearCache();
        }

        public async Task ClearCacheKey(string cacheKey)
        {
            await _cacheService.RemoveAsync(cacheKey);
        }

        public async Task<DataReferencesViewModel> GetDataReferences(string schemaName, string tableName,
            string entityId, int? top, bool? onlyWithDependecies)
        {
            FormattableString queryString = $"SELECT student.fn_admin_ReferencedDataForTableAndEntitySqlGenertor({schemaName}, {tableName}, {entityId}, {top}) AS Str";
            IQueryable<FunctionStringResult> query = _context.Set<FunctionStringResult>()
                .FromSqlInterpolated(queryString);

            string sqlStr = (await query.FirstOrDefaultAsync())?.Str;

            if (sqlStr.IsNullOrWhiteSpace()) return null;

            //FormattableString dataReferencesQueryString = $"{sqlStr}";
            IQueryable<DataReferencesResult> dataReferencesQuery = _context.Set<DataReferencesResult>()
                .FromSqlRaw(sqlStr);

            List<DataReferencesResult> dataReferences = await dataReferencesQuery.ToListAsync();
            if (!dataReferences.Any()) return null;

            if (!top.HasValue) top = 100;
            if (!onlyWithDependecies.HasValue) onlyWithDependecies = false;
            bool showWithDependecies = await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameDataReferencesReadManage);

            DataReferencesViewModel result = new DataReferencesViewModel
            {
                SchemaName = schemaName,
                TableName = tableName,
                EntityId = entityId,
                TakeTop = top.Value,
                OnlyWithDependecies = onlyWithDependecies.Value,
                Result = dataReferences.Select(x => new ReferencesViewModel
                {
                    ReferencingTableSchema = x.ReferencingTableSchema,
                    ReferencingTableName = x.ReferencingTableName,
                    ReferencingColumnName = x.ReferencingColumnName,
                    ReferencedTableSchema = x.ReferencedTableSchema,
                    ReferencedTableName = x.ReferencedTableName,
                    TargetEntityId = x.TargetEntityId,
                    TakeTop = top.Value,
                    OnlyWithDependecies = onlyWithDependecies.Value,
                    HasDependencies = !x.JsonStr.IsNullOrWhiteSpace(),
                    Dependencies = !showWithDependecies || x.JsonStr.IsNullOrWhiteSpace()
                            ? null
                            : JArray.Parse(x.JsonStr),
                    Count = x.Count
                })
                .Where(x => onlyWithDependecies.Value == false || x.HasDependencies == true)
                .OrderByDescending(x => x.HasDependencies)
                .ToList()
            };

            return result;
        }

        public async Task<IPagedList<UnsignedStudentLodListDto>> GetUnsignedStudentLodList(UnsignedStudentLodListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input), nameof(UnsignedStudentLodListInput)));
            }

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodFinalizationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DateTime dischargeDateLimit = new DateTime(input.SchoolYear + 1, 6, 1); // примерно 2023-06-01
            FormattableString queryString = $"select * from student.fn_UnsignedStudentLodList({input.SchoolYear}, {dischargeDateLimit:yyyy-MM-dd})";

            IQueryable<UnsignedStudentLodListDto> query = _context.Set<UnsignedStudentLodListDto>()
                .FromSqlInterpolated(queryString);

            if (_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.RegionId != null && x.RegionId == _userInfo.RegionID.Value);
            }

            if (_userInfo.InstitutionID.HasValue)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
            }

            query = query.OrderBy(string.IsNullOrEmpty(input.SortBy) ? "InstitutionId asc, FullName asc" : input.SortBy);

            string filter = input.Filter ?? "";
            query = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.FullName.Contains(filter)
                   || predicate.Identifier.Contains(filter)
                   || predicate.InstitutionId.ToString().Contains(filter)
                   || predicate.InstitutionName.Contains(filter)
                   || predicate.SchoolYearName.Contains(filter)
                   || predicate.RegionName.Contains(filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "InstitutionId asc, FullName asc" : input.SortBy); ;

            int totalCount = await query.CountAsync();
            List<UnsignedStudentLodListDto> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task FinalizeLods(LodFinalizationAdministrationModel model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFinalizationAdministrationModel)));
            }

            foreach (int personId in model.PersonIds)
            {
                byte[] lodFileContent = await _studentLODService.GeneratePersonalFileInternal(new LodGeneratorModel { PersonId = personId, SchoolYears = new List<int> { model.SchoolYear } }, cancellationToken);

                await _lodFinalizationService.SignLodAsync(new LodSignatureModel
                {
                    PersonId = personId,
                    SchoolYear = model.SchoolYear,
                    Signature = lodFileContent.IsNullOrEmpty()
                        ? ""
                        : Convert.ToBase64String(lodFileContent)
                });
            }
        }
    }
}
