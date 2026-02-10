namespace MON.Services.Implementations
{
    using MON.Models.Diploma.Import;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.ErrorHandling;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class DiplomaImportValidationExclusionService : BaseService<DiplomaImportValidationExclusionService>, IDiplomaImportValidationExclusionService
    {
        private const string DiplomaImportValidationExclusionKey = "DiplomaImportValidationExclusions";

        private readonly IAppConfigurationService _appConfigurationService;

        public DiplomaImportValidationExclusionService(DbServiceDependencies<DiplomaImportValidationExclusionService> dependencies,
            IAppConfigurationService appConfigurationService)
            : base(dependencies)
        {
            _appConfigurationService = appConfigurationService;
        }

        public async Task<List<DiplomaImportValidationExclusionModel>> List()
        {
            string contents = await _appConfigurationService.GetValueByKey(DiplomaImportValidationExclusionKey);

            if (contents.IsNullOrWhiteSpace())
            {
                return new List<DiplomaImportValidationExclusionModel>();
            }


            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return string.IsNullOrWhiteSpace(contents)
                ? null
                : JsonSerializer.Deserialize<List<DiplomaImportValidationExclusionModel>>(contents, options);
        }

        public async Task AddOrUpdate(DiplomaImportValidationExclusionModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentDiplomaImportValidationExclusionsManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            List<DiplomaImportValidationExclusionModel> settings = await List();
            DiplomaImportValidationExclusionModel setting = settings.SingleOrDefault(x => x.Id == model.Id);
            if (setting == null)
            {
                // Нов
                setting = new DiplomaImportValidationExclusionModel()
                {
                    Id = Guid.NewGuid().ToString(),
                };
                settings.Add(setting);
            }

            setting.PersonalIdTypeId = model.PersonalIdTypeId;
            setting.PersonalId = model.PersonalId;
            setting.Series = model.Series;
            setting.FactoryNumber = model.FactoryNumber;
            setting.InstitutonId = model.InstitutonId;
            setting.ValidTo = model.ValidTo;

            await _appConfigurationService.AddOrUpdate(DiplomaImportValidationExclusionKey, JsonSerializer.Serialize(settings));
        }

        public async Task Delete(string id)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentDiplomaImportValidationExclusionsManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            List<DiplomaImportValidationExclusionModel> settings = await List();
            if (settings == null || !settings.Any(x => x.Id == id))
            {
                return;
            }

            string contents = JsonSerializer.Serialize(settings.Where(x => x.Id != id));
            await _appConfigurationService.AddOrUpdate(DiplomaImportValidationExclusionKey, contents);
        }
    }
}
