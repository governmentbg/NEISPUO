using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using MON.Models;
using MON.Models.Absence;
using MON.Services.Interfaces;
using MON.Shared;
using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Infrastructure.Validators
{
    [ExcludeFromCodeCoverage]
    public class AbsencesImportValidator
    {
        private readonly MONContext context;
        private readonly IUserInfo userInfo;
        private readonly IInstitutionService institutionService;
        //private readonly IAppConfigurationService configurationService;

        private bool absenceImportBasicClassIdCheck = false;

        public ApiValidationResult ValidationResult { get; private set; }

        public AbsencesImportValidator(MONContext context, IUserInfo userInfo,
            IInstitutionService institutionService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.userInfo = userInfo ?? throw new ArgumentNullException(nameof(userInfo));
            this.institutionService = institutionService ?? throw new ArgumentNullException(nameof(institutionService));
            //this.configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
            ValidationResult = new ApiValidationResult();
        }

        //private async void LoadConfigAsync()
        //{
        //    string config = await configurationService.GetValueByKey("AbsenceImportBasicClassIdCheck");
        //    if (bool.TryParse(config ?? "", out bool boolValue))
        //    {
        //        this.absenceImportBasicClassIdCheck = boolValue;
        //    }
        //    else
        //    {
        //        this.absenceImportBasicClassIdCheck = false;
        //    }
        //}

        public async Task<ApiValidationResult> ValidateAbsencesImport(List<AbsenceImportModel> absences, AbsenceImportTypeEnum importType, short? schoolYear = null, short? month = null)
        {
            if (absences == null)
            {
                ValidationResult.Errors.Add(Messages.EmptyModelError);
                return ValidationResult;
            }

            if (!userInfo.InstitutionID.HasValue)
            {
                ValidationResult.Errors.Add(Messages.InvalidInstitutionCodeError);
                return ValidationResult;
            }

            int institutionId = userInfo.InstitutionID.Value;

            HashSet<int> institutionIdsHash = absences.Select(x => x.InstitutionCode).ToHashSet();
            HashSet<int> yearsHash = absences.Select(x => x.SchoolYear).ToHashSet();
            HashSet<int> monthHash = absences.Select(x => x.Month).ToHashSet();

            if (institutionIdsHash.Count == 1 && yearsHash.Count == 1 && monthHash.Count == 1)
            {
                if (institutionIdsHash.First() != institutionId)
                {
                    ValidationResult.Errors.Add($"{Messages.UnauthorizedMessageError}{Environment.NewLine}{Messages.InvalidInstitutionCodeError}");
                }

                ValidationResult.Merge(await CheckForExistingImport(institutionIdsHash.First(), yearsHash.First(), monthHash.First(), importType));
            }

            // Има грешки, които не позволяват да се продължи подробната проверка.
            if (ValidationResult.HasErrors) return ValidationResult;


            if (institutionIdsHash.Count > 1)
            {
                ValidationResult.Errors.Add("В записите са посочени повече от една институции.");
            }

            if (yearsHash.Count > 1)
            {
                ValidationResult.Errors.Add("В записите са посочени повече от една учебни години.");
            }

            if (monthHash.Count > 1)
            {
                ValidationResult.Errors.Add("В записите са посочени повече от един месеци.");
            }

            var duplicatedForPerson = absences
                .GroupBy(x => new { x.Identification, x.StudentIdentificationType })
                .Where(x => x.Count() > 1)
                .ToList();
            if (duplicatedForPerson.Any())
            {
                IEnumerable<string> duplicated = duplicatedForPerson.Select(x => $"{x.Key.Identification} - {x.First().FirstName} {x.First().MiddleName} {x.First().LastName}");
                ValidationResult.Errors.Add($"Съществуват дублирани редове за ученик/ученици: {string.Join(" / ", duplicated)}");
            }

            if (schoolYear.HasValue && month.HasValue)
            {
                // Подаване на отсътвия ръчно или от дневник. Учебната година и месеца ги подаваме.
                ValidationResult.Merge(await ChecForActiveCamapign(schoolYear.Value, month.Value));
            } else
            {
                // Подаване на отсътвия от файл. Учебната година и месеца не ги подаваме и трябва да ги вземем от файла (absences)
                // Липсват отсъствия за проверка
                if (!absences.IsNullOrEmpty())
                {
                    schoolYear = (short)absences.First().SchoolYear;
                    month = (short)absences.First().Month;
                    ValidationResult.Merge(await ChecForActiveCamapign(schoolYear.Value, month.Value));
                }
            }

            ValidationResult.Merge(await DetailedValidation(absences, institutionId));

            return ValidationResult;
        }

        public ApiValidationResult ValidateAbsencesExport(List<AbsenceExportModel> absences)
        {
            if (absences == null)
            {
                ValidationResult.Errors.Add(Messages.EmptyModelError);
                return ValidationResult;
            }

            return ValidationResult;
        }

        private async Task<ApiValidationResult> ChecForActiveCamapign(short schoolYear, short month)
        {
            var checkResult = new ApiValidationResult();

            DateTime now = DateTime.UtcNow;
            if (false == (await context.AbsenceCampaigns.AnyAsync(x => x.SchoolYear == schoolYear && x.Month == month
                && (x.IsManuallyActivated || (x.FromDate <= now && now < x.ToDate.AddDays(1)))
                )))
            {
                checkResult.Errors.Add($"За учебна година: {schoolYear} и месец: {month} липсва активна кампания за подаване(импортиране) на отсъствия.");
            }

            return checkResult;
        }

        /// <summary>
        /// Проверява за вече съществуващ и финализиран импорт на отсъствия.
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private async Task<ApiValidationResult> DetailedValidation(List<AbsenceImportModel> absences, int institutionId)
        {
            var checkResult = new ApiValidationResult();

            // Липсват отсъствия за проверка
            if (absences.IsNullOrEmpty()) return checkResult;

            HashSet<int> yearsHash = absences.Select(x => x.SchoolYear).ToHashSet();
            HashSet<int> monthHash = absences.Select(x => x.Month).ToHashSet();

            HashSet<string> personIdentifiersHash = absences
                .Where(x => !x.Identification.IsNullOrEmpty())
                .Select(x => x.Identification)
                .ToHashSet();

            // За PersonalId има unique constraint и ToDictionaryAsync е безопасно и без group
            var personsDict = await context.People
                .AsNoTracking()
                .Where(x => x.PersonalId != null && personIdentifiersHash.Contains(x.PersonalId))
                .ToDictionaryAsync(x => x.PersonalId, x => new
                {
                    x.PersonId,
                    x.PersonalId,
                    x.PersonalIdtype,
                    x.FirstName,
                    x.MiddleName,
                    x.LastName
                });

            var studentClasses = await context.StudentClasses
                .AsNoTracking()
                .Where(x => x.InstitutionId == institutionId
                    && yearsHash.Contains(x.SchoolYear))
                .Select(x => new
                {
                    x.Id,
                    x.PersonId,
                    x.SchoolYear,
                    x.BasicClassId,
                    x.InstitutionId,
                    x.Class.IsNotPresentForm

                })
                .ToListAsync();

            for (int i = 0; i < absences.Count; i++)
            {
                AbsenceImportModel current = absences[i];
                if (current.InstitutionCode != institutionId)
                {
                    checkResult.Errors.Add($"[Ред: {i + 1}] Код на институция по НЕИСПУО: {current.InstitutionCode} -> {Messages.InvalidInstitutionCodeError}");
                }

                if (personsDict.TryGetValue(current.Identification ?? "", out var person))
                {
                    // https://github.com/Neispuo/students/issues/640
                    //// Проверка дали името и фамилията на намерения ученик (Person), по подадения Идентификатор( ЕГН, ЛНЧ или друг), съвпадат с подадените.
                    //if (!person.FirstName.Equals(current.FirstName ?? "", StringComparison.OrdinalIgnoreCase)
                    //    || !person.LastName.Equals(current.LastName ?? "", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    checkResult.Errors.Add($"[Ред: {i + 1}] Име: {current.FirstName}, Фамилия: {current.LastName} -> Разминаване с намерените Име: {person.FirstName}, Фамилия: {person.LastName}");
                    //}

                    // https://github.com/Neispuo/students/issues/640
                    if (person.PersonalIdtype != current.StudentIdentificationType)
                    {
                        checkResult.Errors.Add($"[Ред: {i + 1}] Идентификатор( ЕГН, ЛНЧ или друг): {current.Identification} с Вид на идентификатора: {current.StudentIdentificationType} " +
                            $"-> {Messages.InvalidIdentificationTypeError} Различава се от този в данните на детето/ученика в НЕИСПУО.");
                    }
                }
                else
                {
                    // Не е намерен ученик (Person) по подадения Идентификатор( ЕГН, ЛНЧ или друг)
                    checkResult.Errors.Add($"[Ред: {i + 1}] Идентификатор( ЕГН, ЛНЧ или друг): {current.Identification} -> {Messages.InvalidIdentificatorError}");
                }

                current.PersonId = person?.PersonId;

                //LoadConfigAsync();

                // Проверка за валиден BasicClassId се прави само при включена конфиграция в [student].[AppSettings].AbsenceImportBasicClassIdCheck

                if (this.absenceImportBasicClassIdCheck)
                {
                    InstitutionCacheModel institution = await this.institutionService.GetInstitutionCache(current.InstitutionCode);

                    // Проверка за валиден BasicClassId не се прави за институция от тип ЦСОП
                    if (institution != null && !institution.IsCSOP)
                    {
                        if (!studentClasses.Any(x => x.InstitutionId == current.InstitutionCode
                            && x.PersonId == current.PersonId
                            && (x.BasicClassId == current.Class || x.IsNotPresentForm == true)))
                        {
                            checkResult.Errors.Add($"[Ред: {i + 1}] {current.FirstName} {current.MiddleName} {current.LastName} {current.Identification} -> Невалидна група/паралелка. За учебната година {current.SchoolYear} не е намерен запис на паралека с клас {current.Class}.");
                        }
                    }
                }
            }

            return checkResult;
        }

        /// <summary>
        /// Проверява за вече съществуващ и финализиран импорт на отсъствия.
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private async Task<ApiValidationResult> CheckForExistingImport(int institutionId, int schoolYear, int month, AbsenceImportTypeEnum importType)
        {
            var checkResult = new ApiValidationResult();

            var existingAbsenceImports = await context.AbsenceImports
                .Where(x => x.InstitutionId == institutionId && x.SchoolYear == schoolYear && x.Month == month)
                .Select(x => new { x.Id, x.IsFinalized, x.ImportType })
                .ToListAsync();

            if (existingAbsenceImports.Any(x => x.IsFinalized))
            {
                checkResult.Errors.Add($"За посочените Код на институция: {institutionId}, Учебна година: {schoolYear} и Месец: {month} съществува импортиране на отсъствия, което е завършено/подписано.");
            }

            var existingWithDiffrerentImportType = existingAbsenceImports.FirstOrDefault(x => x.ImportType != null && x.ImportType != (byte)importType);
            if (existingWithDiffrerentImportType != null)
            {
                checkResult.Errors.Add($"За посочените Код на институция: {institutionId}, Учебна година: {schoolYear} и Месец: {month}" +
                    $" е направен {((AbsenceImportTypeEnum)existingWithDiffrerentImportType.ImportType.Value).GetEnumDescriptionAttrValue()}. Не е позволен {importType.GetEnumDescriptionAttrValue()}");
            }

            return checkResult;
        }
    }
}
