using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MON.DataAccess;
using MON.Models;
using MON.Models.Enums;
using MON.Services.Implementations;
using MON.Services.Interfaces;
using MON.Shared.Enums;
using MON.Shared.Extensions;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MON.Services.Infrastructure.Validators
{
    [ExcludeFromCodeCoverage]
    public class AdmissionDocumentValidator : MovementDocumentBaseValidator
    {
        /*
         *  InstType in 1, 2 – PositionId – 3, 7 или 10 (EduState). Не може да се запише в позиция 7, ако няма 3. Не може да се запише два пъти с позиция 3.
            InstType = 3 – PositionId = 8 (ЦПЛР)
            InstType = 4 – PositionId = 7 (ЦСОП)
            -- Валидира се от ValidateInstitutionTypeToPosition().


            Дете може да се записва в ЦПРЛ/СОЗ (позиция 8), без да е записано в основна институция (позиция 3).
            Вида институция е в InstType, BaseSchoolType и DatailedSchoolType.
            -- Не се прави никаква проверка освен за налични два записа с позиция 3 за дадена институция(CheckForExistingPositionForInstitution())
            -- т.е. горното е удовлетворено.


            Позиция 10 – записани в училище и детска градина с флаг, че детето ще се обучава в ЦСОП.
            Флагът може да се вдига от избор в „Позиция“ при записването. ЦСОП ще записва тези деца с позиция 7.
            -- Валидира се чрез първата проверка (ValidateInstitutionTypeToPosition()).
         */

        public AdmissionDocumentValidator(MONContext context,
            IUserInfo userInfo,
            IInstitutionService institutionService,
            EduStateCacheService eduStateCacheService,
            ILogger<AdmissionDocumentValidator> logger)
            : base(context, userInfo, institutionService, eduStateCacheService, logger)
        {

        }

        public async Task<ApiValidationResult> ValidateCreation(AdmissionDocumentModel model)
        {
            ValidationResult.IsValid = true;

            if (model == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Model {nameof(AdmissionDocumentModel)} cant be null!");
                return ValidationResult;
            }

            ValidationResult.Merge(CheckRolePermission(model.InstitutionId));
            //ValidationResult.Merge(await CheckForMissingEnrollmentInPosition(model));  // Това отпада

            ValidationResult.Merge(await CheckForExistingPositionForInstitution(model.PersonId, model.InstitutionId, model.Position));
            ValidationResult.Merge(await ValidateInstitutionTypeToPosition(model.PersonId, model.InstitutionId, model.Position));

            return ValidationResult;
        }

        public ApiValidationResult ValidateDeletion(AdmissionDocument entity)
        {
            ValidationResult.IsValid = true;

            if (entity == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Entity {nameof(AdmissionDocument)} cant be null!");
                return ValidationResult;
            }

            ValidationResult.Merge(CheckDocumentStatusPermisson(entity.Status));
            ValidationResult.Merge(CheckRolePermission(entity.InstitutionId));

            return ValidationResult;
        }

        public async Task<ApiValidationResult> ValidateUpdate(AdmissionDocument entity, AdmissionDocumentModel model, bool fromDraftToConfirmedStatusChange)
        {
            ValidationResult.IsValid = true;

            if (entity == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Entity {nameof(AdmissionDocument)} cant be null!");
                return ValidationResult;
            }

            if (model == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Model {nameof(AdmissionDocumentModel)} cant be null!");
                return ValidationResult;
            }

            // Премахване на ограничението за редакция само на чернови.
            // mmitova - 12.08.2022
            //ValidationResult.Merge(CheckDocumentStatusPermisson(entity.Status));
            ValidationResult.Merge(CheckRolePermission(entity.InstitutionId));

            if (fromDraftToConfirmedStatusChange)
            {
                ValidationResult.Merge(await CheckForExistingPositionForInstitution(entity.PersonId, entity.InstitutionId, model.Position));
            }

            ValidationResult.Merge(await ValidateInstitutionTypeToPosition(entity.PersonId, model.InstitutionId, model.Position));

            return ValidationResult;
        }

        public async Task<ApiValidationResult> ValidateConfirmation(AdmissionDocument entity)
        {
            ValidationResult.IsValid = true;

            if (entity == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Entity {nameof(AdmissionDocument)} cant be null!");
                return ValidationResult;
            }

            ValidationResult.Merge(CheckDocumentStatusPermisson(entity.Status));
            ValidationResult.Merge(CheckRolePermission(entity.InstitutionId));

            ValidationResult.Merge(await CheckForExistingPositionForInstitution(entity.PersonId, entity.InstitutionId, entity.PositionId));
            ValidationResult.Merge(await ValidateInstitutionTypeToPosition(entity.PersonId, entity.InstitutionId, entity.PositionId));

            return ValidationResult;
        }

        /// <summary>
        /// Проверява дали документ за записване може да се редактира/трие/потвърждава.
        /// Използва се от AdmissionDocumentService при зареждане на данните за грида с документи за записване
        /// за даден ученик. Определя видимостта на бутоните за редакция, триене и потвърждаване.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        public (bool checkResult, string error) CanBeModified(int status, int institutionId)
        {
            bool checkResult = true;
            string errorMessages = "";

            var docStatusCheckResult = CheckDocumentStatusPermisson(status);
            var rolePermissionCheckResult = CheckRolePermission(institutionId);

            errorMessages += docStatusCheckResult.ToString();
            errorMessages += rolePermissionCheckResult.ToString();


            checkResult = checkResult && docStatusCheckResult.IsValid;
            checkResult = checkResult && rolePermissionCheckResult.IsValid;

            return (checkResult, errorMessages);
        }


        /// <summary>
        /// Проверка за съществуващи записи в EducationalStates за ученик и институция.
        /// Не е позволено съществуването на повече от един запис за ученик в дадена институция с позиция „училище/ДГ“.
        /// </summary>
        /// <param name="personId">Ученик, който записваме</param>
        /// <param name="institutionId">Институция, в която записваме</param>
        /// <param name="position">Позиция, с която записваме</param>
        /// <returns></returns>
        private async Task<ApiValidationResult> CheckForExistingPositionForInstitution(int personId, int institutionId, int position)
        {
            ApiValidationResult checkResult = new ApiValidationResult
            {
                IsValid = true
            };

            List<EducationalState> educationalStates = await GetStudentEduStates(personId);
            switch (position)
            {
                case (int)PositionType.Student:
                    if (educationalStates.Any(x => x.InstitutionId == institutionId && x.PositionId == (int)PositionType.Student))
                    {
                        checkResult.IsValid = false;
                        checkResult.Messages.Add("Не е позволено да се създаде нов документ за записване с позиция „училище/ДГ“ от потребител на институцията, в която детето е записано.");
                    }
                    break;
                case (int)PositionType.StudentPersDevelopmentSupport:
                case (int)PositionType.StudentOtherInstitution:
                    if (educationalStates.Any(x => x.InstitutionId == institutionId && 
                        (x.PositionId == (int)PositionType.StudentPersDevelopmentSupport || x.PositionId == (int)PositionType.StudentOtherInstitution)))
                    {
                        checkResult.IsValid = false;
                        checkResult.Messages.Add("Ученикът/детето вече е записан/о в институцията. Не може да създадете втори документ за записване.");
                    }
                    break;
                default:
                    break;
            }

            return checkResult;
        }

        /// <summary>
        /// Ако детето не е записано в нито една институция, при записване да не може да се избира позиция, различна от „училище/ДГ“.
        /// Това важи само за институция от тип различен от ЦПЛР. ЦПЛР може да записва деца, дори и да не са записани в училище.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<ApiValidationResult> CheckForMissingEnrollmentInPosition(AdmissionDocumentModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            }

            ApiValidationResult checkResult = new ApiValidationResult
            {
                IsValid = true
            };

            InstitutionCacheModel institutionBaseMode = await _institutionService.GetInstitutionCache(model.InstitutionId);
            if (institutionBaseMode == null)
            {
                throw new ArgumentNullException(nameof(institutionBaseMode));
            }

            int positionTypeStudent = (int)PositionType.Student;
            if (institutionBaseMode.InstTypeId.HasValue 
                && !institutionBaseMode.IsCPLR
                && model.Position != positionTypeStudent)
            {

                List<EducationalState> educationalStates = await GetStudentEduStates(model.PersonId);
                if (!educationalStates.Any(x => x.PositionId == positionTypeStudent))
                {
                    checkResult.IsValid = false;
                    checkResult.Messages.Add($"Ученикът не е записан в нито една институция с позиция 'училище/ДГ'. Това е задължително условия за да бъде записан с друга позиция.");
                }
            }

            return checkResult;
        }

        /// <summary>
        /// Проверка за позволена позиция спрямо типа на институцията, в която записваме.
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private async Task<ApiValidationResult> ValidateInstitutionTypeToPosition(int personId, int institutionId, int position)
        {
            ApiValidationResult checkResult = new ApiValidationResult
            {
                IsValid = true
            };

            InstitutionCacheModel institutionBaseMode = await _institutionService.GetInstitutionCache(institutionId);
            if (institutionBaseMode == null)
            {
                throw new ArgumentNullException(nameof(institutionBaseMode));
            }

            PositionType positionType = (PositionType)position;

            // Проверка за позволена позиция спрямо типа на институцията.
            // Позволените позиции за даден тип институция са описани в student.AppSettings, ключ InstTypeToPositionLimit
            HashSet<int> allowedPositions = await _institutionService.GetAllowedStudentPositions(institutionId);
            if (!allowedPositions.Contains(position))
            {
                checkResult.IsValid = false;
                checkResult.Messages.Add($"За избраната институция не е позволено да създаде документ за записване с позиция: {positionType.GetEnumDescription()}.");
            }

            // InstType in 1, 2 – Не може да се запише в позиция 7, ако няма 3.
            if (positionType == PositionType.StudentOtherInstitution
                && (institutionBaseMode.IsSchool || institutionBaseMode.IsKinderGarden))
            {
                List<EducationalState> educationalStates = await GetStudentEduStates(personId);
                if (!educationalStates.Any(x => x.PositionId == (int)PositionType.Student))
                {
                    checkResult.IsValid = false;
                    checkResult.Messages.Add($"Не е позволено записване на дете/ученик с позиция: {PositionType.StudentOtherInstitution.GetEnumDescription()} без да е записан с позиция: {PositionType.Student.GetEnumDescription()} в друга институция.");
                }
            }

            if (positionType == PositionType.Student  && await _context.StudentClasses.AnyAsync(x => x.PersonId == personId
                            && x.IsCurrent && x.InstitutionId != institutionId
                            && x.InstitutionSchoolYear.DetailedSchoolType.InstType == (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport
                            && x.ClassType.ClassKind == (int)ClassKindEnum.Basic))
            {
                checkResult.IsValid = false;
                checkResult.Messages.Add($"Не е позволено записване на дете/ученик с позиция: {PositionType.Student.GetEnumDescription()}. Детето/ученикът е записан в учебна група/паралелка в ЦСОП.");
            }

            return checkResult;
        }

        private Task<List<EducationalState>> GetStudentEduStates(int personId)
        {
            return _context.EducationalStates
                .Where(x => x.PersonId == personId)
                .ToListAsync();
        }
    }
}
