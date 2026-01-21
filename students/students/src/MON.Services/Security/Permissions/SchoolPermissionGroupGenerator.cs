using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MON.Models;
using MON.Models.Cache;
using MON.Models.EduState;
using MON.Models.Enums;
using MON.Services.Implementations;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared;
using MON.Shared.Enums;
using MON.Shared.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Security
{
    public class SchoolPermissionGroupGenerator : BasePermissionGroupGenerator
    {
        private readonly EduStateCacheService _eduStateCacheService;
        private readonly IAppConfigurationService _configurationService;
        private HashSet<int> _massEnrollmentAllowedClassTypes = null;
        private readonly ICacheService _cache;

        public SchoolPermissionGroupGenerator(IServiceProvider serviceProvider,
            IUserInfo userInfo, PermissionsContextEnum permissionsContext)
            : base(serviceProvider, userInfo, permissionsContext)
        {
            _eduStateCacheService = serviceProvider?.GetRequiredService<EduStateCacheService>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _configurationService = serviceProvider?.GetRequiredService<IAppConfigurationService>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _cache = serviceProvider?.GetRequiredService<ICacheService>() ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        private async Task<bool> HasSchoolTypeLodAccess(int detailedSchoolTypeId)
        {
            if (_cache == null)
            {
                bool hasSchoolTypeLoadAccess = await Context.SchoolTypeLodAccesses
                    .AnyAsync(x => x.DetailedSchoolTypeId == detailedSchoolTypeId && x.IsLodAccessAllowed);
                return hasSchoolTypeLoadAccess;
            }


            string key = CacheKeys.SchoolTypeLodAccess;

            List<SchoolTypeLodAccessCacheModel> list = await _cache.GetAsync<List<SchoolTypeLodAccessCacheModel>>(key);
            if (list == null)
            {
                list = await Context.SchoolTypeLodAccesses
                    .Select(x => new SchoolTypeLodAccessCacheModel
                    {
                        DetailedSchoolTypeId = x.DetailedSchoolTypeId,
                        IsLodAccessAllowed = x.IsLodAccessAllowed,
                    })
                    .ToListAsync();

                await _cache.SetAsync(key, list);
            }

            return list.Any(x => x.DetailedSchoolTypeId == detailedSchoolTypeId && x.IsLodAccessAllowed);
        }

        public HashSet<int> MassEnrollmentAllowedClassTypes
        {
            get
            {
                if (_massEnrollmentAllowedClassTypes == null)
                {
                    string config = _configurationService.GetValueByKey("MassEnrollmentAllowedClassTypes").Result;
                    if (config.IsNullOrWhiteSpace())
                    {
                        _massEnrollmentAllowedClassTypes = new HashSet<int>();
                    }
                    else
                    {
                        _massEnrollmentAllowedClassTypes = JsonConvert.DeserializeObject<HashSet<int>>(config ?? "");
                    }
                }

                return _massEnrollmentAllowedClassTypes;
            }
        }

        protected override async Task<HashSet<PermissionGroupEnum>> GetClassGroupPermissionGroupsForLoggedUser(int classGroupId)
        {
            ClassGroupCacheModel classGroup = await ClassGroupService.GetClassGroupCache(classGroupId);

            if (UserInfo.InstitutionID.HasValue && UserInfo.InstitutionID.Value == classGroup?.InstitutionId)
            {
                if (!PermissionGroups.Contains(PermissionGroupEnum.Owner)) PermissionGroups.Add(PermissionGroupEnum.Owner);

                InstitutionCacheModel institution = await InstitutionService.GetInstitutionCache(UserInfo.InstitutionID.Value);

                if (institution != null && institution.InstTypeId.HasValue
                    && (institution.InstTypeId == (int)InstitutionTypeEnum.PersonalDevelopmentSupportCenter || institution.InstTypeId == (int)InstitutionTypeEnum.SpecializedServiceUnit)
                    && UserInfo.UserRole == UserRoleEnum.School)
                {
                    if (!PermissionGroups.Contains(PermissionGroupEnum.MassEntrollmentManager)) PermissionGroups.Add(PermissionGroupEnum.MassEntrollmentManager);
                }

                // Да се даде възможност за безконтролно записване на деца в несамостоятелни общежития #1158.
                // Ако една институция е InstType = 1, да може да записва ученици в неучебни групи от тип 39 или 49 по подобие на ЦПЛР,
                // без да иска разрешение от основното училище, ако детето не е записано при тях?
                // Идеята е гимназии с общежитие да си записват деца да спят при тях. 
                if (institution != null && institution.InstTypeId.HasValue && (institution.InstTypeId == (int)InstitutionTypeEnum.School || institution.InstTypeId == (int)InstitutionTypeEnum.KinderGarden) 
                    && MassEnrollmentAllowedClassTypes.Contains(classGroup?.ClassTypeId ?? 0))
                {
                    if (!PermissionGroups.Contains(PermissionGroupEnum.MassEntrollmentManager)) PermissionGroups.Add(PermissionGroupEnum.MassEntrollmentManager);
                }

            }

            return PermissionGroups;
        }

        protected override async Task<HashSet<PermissionGroupEnum>> GetInstitutionPermissionGroupsForLoggedUser(int institutionId)
        {
            if (UserInfo.InstitutionID.HasValue && UserInfo.InstitutionID == institutionId)
            {             
                PermissionGroups.Add(PermissionGroupEnum.Owner);
                InstitutionCacheModel institution = await InstitutionService.GetInstitutionCache(institutionId);

                bool hasSchoolTypeLoadAccess = institution != null
                     ? await HasSchoolTypeLodAccess(institution.DetailedSchoolTypeId)
                     : false;

                if (!hasSchoolTypeLoadAccess)
                {
                    // Бутонът за вдигане на ръчичка да се показва спрямо настройките в SchoolTypeLodAccess #1050
                    PermissionGroups.Add(PermissionGroupEnum.AdmissionPermissionRequestManager);
                }
            }

            return PermissionGroups;
        }

        protected override async Task<HashSet<PermissionGroupEnum>> GetStudentPermissionGroupsForLoggedUser(int studentId)
        {
            // Липсва InstitutionID На логнатия потребител
            if (!UserInfo.InstitutionID.HasValue) return PermissionGroups;

            int instId = UserInfo.InstitutionID.Value;
            List<EduStateModel> eduStates = _eduStateCacheService != null
                ? await _eduStateCacheService.GetEduStatesForStudent(studentId)
                : await Context.EducationalStates
                    .AsNoTracking()
                    .Where(x => x.PersonId == studentId && x.PositionId != (int)PositionType.Staff)
                    .Select(x => new EduStateModel
                    {
                        PersonId = x.PersonId,
                        PositionId = x.PositionId,
                        InstitutionId = x.InstitutionId
                    })
                    .ToListAsync();

            if (eduStates.Any(x => x.InstitutionId == instId && x.PositionId != (int)PositionType.Discharged))
            {
                // В EducationalStates има запис с институцията на логнатия потребител и позиция различна от PositionType.Discharge
                // Значи сме собственик
                PermissionGroups.Add(PermissionGroupEnum.Owner);

                return PermissionGroups;
            }

            InstitutionCacheModel institution = await InstitutionService.GetInstitutionCache(instId);

            #region Проверка в SchoolTypeLodAccesses
            if (institution != null)
            {
                bool hasSchoolTypeLoadAccess = await HasSchoolTypeLodAccess(institution.DetailedSchoolTypeId);

                // В SchoolTypeLodAccesses има запис показващ, че за типа на институцията на логнатия потребител има дадено право до ЛОД-а

                if (hasSchoolTypeLoadAccess)
                {
                    PermissionGroups.Add(PermissionGroupEnum.Owner);

                    return PermissionGroups;
                }
            }
            #endregion


            if (!PermissionGroups.Contains(PermissionGroupEnum.AdmissionDocCreator)
                && !eduStates.Any(x => x.PositionId != (int)PositionType.Discharged))
            {
                // Липсва запис за ученика в EducationalStates с позиция различна от PositionType.Discharged
                PermissionGroups.Add(PermissionGroupEnum.AdmissionDocCreator);
            }

            if (!PermissionGroups.Contains(PermissionGroupEnum.Reader)
                && eduStates.Any(x => x.InstitutionId == instId && x.PositionId == (int)PositionType.Discharged))
            {
                PermissionGroups.Add(PermissionGroupEnum.Reader);
            }

            await CheckAdmissionPermissionRequests(studentId, instId);
            await CheckDiplomaCreateRequests(studentId, instId);
            await CheckOtherInstitutions(studentId, instId);

            // Ако е записвано с позиция 7 или 8 от институция с InstType = 3,
            // то следва да може да се запише и в позиция 3 от онституциия с InstType 1
            if (institution.IsSchool || institution.IsKinderGarden)
            {
                // Ако има такъв permission няма смисъл да го добавяме
                if (!PermissionGroups.Contains(PermissionGroupEnum.AdmissionDocCreator))
                {
                    // Ако е записан с позиция 3 в друга институция, нямаме право да го записваме втори път
                    if (!eduStates.Any(x => x.InstitutionId != instId && x.PositionId == (int)PositionType.Student))
                    {
                        // Можем да го запишем само ако е записан с позиция 7 или 8 в друга институция
                        if (eduStates.Any(x => x.InstitutionId != instId
                                && (x.PositionId == (int)PositionType.StudentOtherInstitution || x.PositionId == (int)PositionType.StudentPersDevelopmentSupport)))
                        {
                            PermissionGroups.Add(PermissionGroupEnum.AdmissionDocCreator);
                        }
                    }
                }
            }

            // За следните типове институции даваме поне PartialPersonalDataReader роля, която показва част от личните данни 
            switch (institution.InstTypeId)
            {
                case (int)InstitutionTypeEnum.PersonalDevelopmentSupportCenter:
                    if (!PermissionGroups.Contains(PermissionGroupEnum.PartialPersonalDataReader)) {
                        PermissionGroups.Add(PermissionGroupEnum.PartialPersonalDataReader);
                    }

                    break;
                case (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport:
                    // Ако институцията е с InstType 4(ЦСОП) и не е получил група с права Owner
                    // при проверка за наличен запис в SchoolTypeLodAccesses 
                    // да има права поне за създаване на документ за записване.
                    if (!PermissionGroups.Contains(PermissionGroupEnum.AdmissionDocCreator)) {
                        PermissionGroups.Add(PermissionGroupEnum.AdmissionDocCreator);
                    }

                    if (!PermissionGroups.Contains(PermissionGroupEnum.PartialPersonalDataReader))
                    {
                        PermissionGroups.Add(PermissionGroupEnum.PartialPersonalDataReader);
                    }

                    break;
                case (int)InstitutionTypeEnum.SpecializedServiceUnit:
                    if (!PermissionGroups.Contains(PermissionGroupEnum.PartialPersonalDataReader)) {
                        PermissionGroups.Add(PermissionGroupEnum.PartialPersonalDataReader);
                    }

                    break;
                default:
                    break;
            }

            // Премахване на права
            // 1. Ако даден PersonId съществува в EduState за дадената институция


            return PermissionGroups;
        }

        /// <summary>
        /// Проверка за наличен запис в AdmissionPermissionRequests. Поискано право за записване на ученик.
        /// При налично право ще дадем правата на група PermissionGroupEnum.AdmissionDocCreator
        /// </summary>
        /// <returns></returns>
        private async Task CheckAdmissionPermissionRequests(int studentId, int instId)
        {
            // Вече има търсената група с права
            if (PermissionGroups.Contains(PermissionGroupEnum.Owner)
                || PermissionGroups.Contains(PermissionGroupEnum.AdmissionDocCreator)) return;

            if (await Context.AdmissionPermissionRequests.AnyAsync(x => x.PersonId == studentId
                && x.RequestingInstitutionId == instId
                && x.IsPermissionGranted
                && x.AdmissionDocumentId.HasValue == false))
            {
                // Поискано и дадено право за създаване не документ за записване,
                // който обаче не е използван т.е. липсва обвързан документ за записване
                PermissionGroups.Add(PermissionGroupEnum.AdmissionDocCreator);
            }
        }

        /// <summary>
        /// Проверка за наличен запис в DiplomaCreateRequests, който не е използван.
        /// Създадена заявление за издаване на документ/диплома.
        /// При налично заявление ще дадем правата на група PermissionGroupEnum.DiplomaCreator
        /// </summary>
        /// <returns></returns>
        private async Task CheckDiplomaCreateRequests(int studentId, int instId)
        {
            // Вече съществуват необходимите права
            if (PermissionGroups.Contains(PermissionGroupEnum.Owner)
                || PermissionGroups.Contains(PermissionGroupEnum.DiplomaCreator))
            {
                return;
            }

            if (await Context.DiplomaCreateRequests.AnyAsync(x => x.PersonId == studentId
                && x.RequestingInstitutionId == instId
                && x.Deleted == false
                && x.IsGranted == true
                && x.DiplomaId == null))
            {
                // Поискано и дадено право за създаване на диплома
                PermissionGroups.Add(PermissionGroupEnum.DiplomaCreator);
            }
        }

        private async Task CheckOtherInstitutions(int studentId, int instId)
        {
            // Вече има търсената група с права
            if (PermissionGroups.Contains(PermissionGroupEnum.Owner)
                || PermissionGroups.Contains(PermissionGroupEnum.AdmissionDocCreator)) return;

            if (await Context.OtherInstitutions.AnyAsync(x => x.InstitutionId == instId && x.PersonId == studentId))
            {
                PermissionGroups.Add(PermissionGroupEnum.AdmissionDocCreator);
            }
        }

        protected override async Task<HashSet<string>> GetStudentDenyPermissionsForLoggedUser(int studentId)
        {
            HashSet<string> denyPermissions = new HashSet<string>();
            // Липсва InstitutionID На логнатия потребител
            if (!UserInfo.InstitutionID.HasValue) return denyPermissions;

            List<EduStateModel> eduStates = _eduStateCacheService != null
                ? await _eduStateCacheService.GetEduStatesForStudent(studentId)
                : await Context.EducationalStates
                    .AsNoTracking()
                    .Where(x => x.PersonId == studentId && x.PositionId != (int)PositionType.Staff)
                    .Select(x => new EduStateModel
                    {
                        PersonId = x.PersonId,
                        PositionId = x.PositionId,
                        InstitutionId = x.InstitutionId
                    })
                    .ToListAsync();

            int instId = UserInfo.InstitutionID.Value;
            if (eduStates.Any(x => x.InstitutionId == instId && x.PositionId != (int)PositionType.Discharged))
            {
                // Е EduState има запис за даден PersonId и InstitutionId с позиция различна от 9.
                // Следователно е записан в институцията и не следва да има право да създава
                // документи за записване.
                denyPermissions.Add(DefaultPermissions.PermissionNameForStudentAdmissionDocumentCreate);
            }

            if (!eduStates.Any(x => x.InstitutionId == instId && x.PositionId != (int)PositionType.Discharged))
            {
                // Е EduState липса запис за даден PersonId и InstitutionId с позиция различна от 9.
                // Следователно е отписан от институцията и не следва да има право да създава
                // документи за отписване и преместване.
                denyPermissions.Add(DefaultPermissions.PermissionNameForStudentDischargeDocumentCreate);
                denyPermissions.Add(DefaultPermissions.PermissionNameForStudentRelocationDocumentCreate);
            }

            return denyPermissions;
        }
        protected override Task<HashSet<string>> GetInstitutionDenyPermissionsForLoggedUser(int institutionId)
        {
            return Task.FromResult(new HashSet<string>());
        }
        protected override Task<HashSet<string>> GetClassDenyPermissionsForLoggedUser(int classGroupId)
        {
            return Task.FromResult(new HashSet<string>());
        }
    }
}
