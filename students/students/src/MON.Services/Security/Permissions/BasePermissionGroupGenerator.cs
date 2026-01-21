using Microsoft.Extensions.DependencyInjection;
using MON.DataAccess;
using MON.Models;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Security
{
    public abstract class BasePermissionGroupGenerator : IPermissionGenerator
    {
        private MONContext _context;
        private IClassGroupService _classGroupService;
        private IInstitutionService _institutionService;

        protected IServiceProvider ServiceProvider { get; private set; }
        protected IUserInfo UserInfo { get; private set; }
        protected PermissionsContextEnum PermissionContext { get; private set; }
        protected HashSet<PermissionGroupEnum> PermissionGroups { get; private set; }

        protected BasePermissionGroupGenerator(IServiceProvider serviceProvider, IUserInfo userInfo, PermissionsContextEnum permissionsContext)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider), nameof(IServiceProvider));
            UserInfo = userInfo ?? throw new ArgumentNullException(nameof(userInfo), nameof(IUserInfo));
            PermissionContext = permissionsContext;
            PermissionGroups = new HashSet<PermissionGroupEnum>();
        }

        protected MONContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = ServiceProvider.GetRequiredService<MONContext>();
                }

                return _context;
            }
        }

        protected IClassGroupService ClassGroupService
        {
            get
            {
                if (_classGroupService == null)
                {
                    _classGroupService = ServiceProvider.GetRequiredService<IClassGroupService>();
                }

                return _classGroupService;
            }
        }

        protected IInstitutionService InstitutionService
        {
            get
            {
                if (_institutionService == null)
                {
                    _institutionService = ServiceProvider.GetRequiredService<IInstitutionService>();
                }

                return _institutionService;
            }
        }

        public async Task<HashSet<string>> GetUserAllowPermissions(int? entityId)
        {
            HashSet<string> permissions = new HashSet<string>();

            if (!entityId.HasValue || UserInfo == null) return permissions;

            HashSet<PermissionGroupEnum> permissionGroupsForLoggedUser = await GetPermissionGroupsForLoggedUser(entityId.Value);

            if (permissionGroupsForLoggedUser == null || !permissionGroupsForLoggedUser.Any()) return permissions;

            foreach (PermissionGroupEnum group in permissionGroupsForLoggedUser)
            {
                switch (PermissionContext)
                {
                    case PermissionsContextEnum.Student:
                        if (StudentPermissionLevelGroups.GetInstance.All.TryGetValue((int)group, out HashSet<string> studentPermissonsHash))
                        {
                            permissions.UnionWith(studentPermissonsHash);
                        }
                        break;
                    case PermissionsContextEnum.Institution:
                        if (InstitutionPermissionLevelGroups.GetInstance.All.TryGetValue((int)group, out HashSet<string> institutionPermissonsHash))
                        {
                            permissions.UnionWith(institutionPermissonsHash);
                        }
                        break;
                    case PermissionsContextEnum.ClassGroup:
                        if (ClassPermissionLevelGroup.GetInstance.All.TryGetValue((int)group, out HashSet<string> classPermissonsHash))
                        {
                            permissions.UnionWith(classPermissonsHash);
                        }
                        break;
                    default:
                        break;
                }

            }

            return permissions;
        }

        public virtual async Task<HashSet<string>> GetUserDenyPermissions(int? entityId)
        {
            // Права, които трябва да се премахнат независимо от контекста.
            HashSet<string> denyPermissons = new HashSet<string>();
            if (UserInfo.InstitutionID.HasValue)
            {
                InstitutionCacheModel institution = await InstitutionService.GetInstitutionCache(UserInfo.InstitutionID.Value);
                if (UserInfo.UserRole == UserRoleEnum.School)
                {
                    switch (institution.InstTypeId)
                    {
                        // Не се достъпват менюта в ЛОД, които не са приложими в ДГ
                        case (int)InstitutionTypeEnum.KinderGarden:
                            // Бележки
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentNoteRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentNoteManage);
                            // подменю Оценки в ЛОД
                            //denyPermissons.Add(DefaultPermissions.PermissionNameForStudentEvaluationRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentEvaluationManage);
                            // подменю НВО/ДЗИ
                            //denyPermissons.Add(DefaultPermissions.PermissionNameForStudentExternalEvaluationRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentExternalEvaluationManage);
                            // подменюта Приравняване, Признаване, Валидиране, Изпити за промяна на оценка
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentRecognitionRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentRecognitionManage);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentEqualizationRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentEqualizationManage);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentReassessmentRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentReassessmentManage);
                            // подменю Стипендии
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentScholarshipRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentScholarshipManage);
                            // подменю Санкции
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentSanctionRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentSanctionManage);
                            // подменю Ученическо самоуправление
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentSelfGovernmentRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentSelfGovernmentManage);
                            // подменю Международна мобилност
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentInternationalMobilityRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentInternationalMobilityManage);
                            break;

                        // Не се достъпват менюта в ЛОД, които не са приложими в ЦПЛР и СОЗ
                        case (int)InstitutionTypeEnum.SpecializedServiceUnit:
                        case (int)InstitutionTypeEnum.PersonalDevelopmentSupportCenter:
                            // Документи за преместване
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentRelocationDocumentRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentRelocationDocumentCreate);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentRelocationDocumentUpdate);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentRelocationDocumentDelete);
                            // Други институции
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentOtherInstitutionRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentOtherInstitutionManage);
                            // Бележки
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentNoteRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentNoteManage);
                            // само чете Характеристика на средата
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicManage);
                            // само чете Предучилищна подготовка
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage);
                            // само чете Оценки
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentEvaluationManage);
                            // само чете НВО/ДЗИ
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentExternalEvaluationManage);
                            // няма достъп до Други документи
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentOtherDocumentRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentOtherDocumentManage);
                            // само чете ПЛР - Ранно оценяване и ПЛР - ОПЛР/ДПЛР
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage);
                            // само чете СОП
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentSopManage);
                            // само чете РП
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentResourceSupportManage);
      
                            // няма достъп до Приравняване, Признаване, Валидиране, Изпити за промяна на оценка
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentRecognitionRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentRecognitionManage);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentEqualizationRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentEqualizationManage);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentReassessmentRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentReassessmentManage);
                            // само чете Стипендии
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentScholarshipManage);
                            // само чете Санкции
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentSanctionManage);
                            // само чете Ученическо самоуправление
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentSelfGovernmentManage);
                            // само чете Международна мобилност
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentInternationalMobilityManage);
                            // Приключване на ЛОД
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentLodFinalizationRead);
                            
                            // ОРЕС
                            denyPermissons.Add(DefaultPermissions.PermissionNameForOresRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForOresManage);

                            // Бутоните за приключване на ЛОД да не се виждат
                            denyPermissons.Add(DefaultPermissions.PermissionNameForLodStateManage);

                            // Бутоните Отсъствия в списък с паралки/групи да не се виждат
                            denyPermissons.Add(DefaultPermissions.PermissionNameForClassAbsenceRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForClassAbsenceManage);

                            // Меню Отсъствия да не се вижда
                            denyPermissons.Add(DefaultPermissions.PermissionNameForAbsencesExportRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForAbsencesExportManage);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentAbsenceRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentAbsenceManage);

                            // Меню Дипломи да не се вижда
                            denyPermissons.Add(DefaultPermissions.PermissionNameForAdminDiplomaRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForAdminDiplomaManage);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentDiplomaRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentDiplomaManage);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentDiplomaByCreateRequestRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentDiplomaByCreateRequestManage);

                            // Меню Импорт на оценки да не се вижда
                            denyPermissons.Add(DefaultPermissions.PermissionNameForLodAssessmentImport);

                            // Подписване и прикачване на личен картон
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentDischargeDocumentSignLOD);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentRelocationDocumentSignLOD);
                            break;
                        case (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport:
                            // Други институции
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentOtherInstitutionRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentOtherInstitutionManage);
                            
                            // Бележки
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentNoteRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentNoteManage);

                            // само чете СОП
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentSopManage);
                            // само чете ПЛР - Ранно оценяване и ПЛР - ОПЛР/ДПЛР
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage);
                            // само чете РП
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentResourceSupportManage);

                            // Меню Дипломи да не се вижда
                            denyPermissons.Add(DefaultPermissions.PermissionNameForAdminDiplomaManage);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentDiplomaManage);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentDiplomaByCreateRequestManage);

                            // Меню Импорт на оценки да не се вижда
                            denyPermissons.Add(DefaultPermissions.PermissionNameForLodAssessmentImport);
                            
                            // Подписване и прикачване на личен картон
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentDischargeDocumentSignLOD);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentRelocationDocumentSignLOD);
                            break;
                        case (int)InstitutionTypeEnum.School:
                            // Бележки
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentNoteRead);
                            denyPermissons.Add(DefaultPermissions.PermissionNameForStudentNoteManage);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (entityId.HasValue)
            {
                // Права, които трябва да се премахнат в зависимост от контекста.
                HashSet<string> specificDenyPermissions = PermissionContext switch
                {
                    PermissionsContextEnum.Student => await GetStudentDenyPermissionsForLoggedUser(entityId.Value),
                    PermissionsContextEnum.Institution => await GetInstitutionDenyPermissionsForLoggedUser(entityId.Value),
                    PermissionsContextEnum.ClassGroup => await GetClassDenyPermissionsForLoggedUser(entityId.Value),
                    _ => null,
                };

                if (specificDenyPermissions != null && specificDenyPermissions.Count > 0)
                {
                    denyPermissons.UnionWith(specificDenyPermissions);
                }
            }

            // Проверка за избран външен доставчик на СО 
            if (UserInfo.InstitutionID.HasValue)
            {
                int institutionCurrentSchoolYear = await _institutionService.GetCurrentYear(UserInfo.InstitutionID.Value);
                bool hasExtSoProviderIntegration = await _institutionService.HasExternalSoProvider(UserInfo.InstitutionID.Value, institutionCurrentSchoolYear);
                if (hasExtSoProviderIntegration)
                {
                    // Ако е избран външен доставчик на СО следва да забраним възможностите за управление на:
                    // Стипендии, СОП, Харакетеристика на средата.
                    denyPermissons.Add(DefaultPermissions.PermissionNameForStudentSopManage);
                    denyPermissons.Add(DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicManage);
                    denyPermissons.Add(DefaultPermissions.PermissionNameForStudentScholarshipManage);
                }
            }

            return denyPermissons;
        }

        private Task<HashSet<PermissionGroupEnum>> GetPermissionGroupsForLoggedUser(int entityId)
        {
            return PermissionContext switch
            {
                PermissionsContextEnum.Student => GetStudentPermissionGroupsForLoggedUser(entityId),
                PermissionsContextEnum.Institution => GetInstitutionPermissionGroupsForLoggedUser(entityId),
                PermissionsContextEnum.ClassGroup => GetClassGroupPermissionGroupsForLoggedUser(entityId),
                _ => null,
            };
        }

        protected abstract Task<HashSet<PermissionGroupEnum>> GetStudentPermissionGroupsForLoggedUser(int studentId);
        protected abstract Task<HashSet<PermissionGroupEnum>> GetInstitutionPermissionGroupsForLoggedUser(int institutionId);
        protected abstract Task<HashSet<PermissionGroupEnum>> GetClassGroupPermissionGroupsForLoggedUser(int classGroupId);

        protected abstract Task<HashSet<string>> GetStudentDenyPermissionsForLoggedUser(int studentId);
        protected abstract Task<HashSet<string>> GetInstitutionDenyPermissionsForLoggedUser(int institutionId);
        protected abstract Task<HashSet<string>> GetClassDenyPermissionsForLoggedUser(int classGroupId);

    }
}
