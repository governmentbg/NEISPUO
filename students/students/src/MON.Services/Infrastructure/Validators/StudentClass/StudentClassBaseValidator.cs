namespace MON.Services.Infrastructure.Validators
{
    using DocumentFormat.OpenXml.Bibliography;
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.Enums;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Class;
    using MON.Services.Extensions;
    using MON.Services.Interfaces;
    using MON.Shared;
    using MON.Shared.Enums;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Extensions;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class StudentClassBaseValidator : IStudentClassValidator
    {
        protected readonly MONContext _context;
        protected readonly IUserInfo _userInfo;
        protected readonly IInstitutionService _institutionService;

        public StudentClassEnrollmentValidationResult ValidationResult { get; private set; }

        public StudentClassBaseValidator(MONContext context, IUserInfo userInfo, IInstitutionService institutionService)
        {
            _context = context;
            _userInfo = userInfo;
            _institutionService = institutionService;
            ValidationResult = new StudentClassEnrollmentValidationResult();
        }

        public virtual async Task<(bool showInitialEntollmentButtonCheck, string showInitialEntollmentButtonCheckError)> ShowInitialEntollmentButton(AdmissionDocumentViewModel admissionDocument)
        {
            if (admissionDocument == null) return (false, "Empty or null admissionDocument");

            if (admissionDocument.UsedInClassEnrollment) return (false, "Used in class enrollment");

            StudentClassEnrollmentValidationResult rolePermissionCheck = CheckRolePermission(admissionDocument.InstitutionId);
            if (rolePermissionCheck.HasErrors) return (false, rolePermissionCheck.ToString());

            StudentClassEnrollmentValidationResult institutionTypeToPositionCheck = await CheckInstitutionTypeToPosition(admissionDocument.InstitutionId, admissionDocument.Position);
            if (institutionTypeToPositionCheck.HasErrors)
            {
                return (false, institutionTypeToPositionCheck.ToString());
            }

            //int positionTypeStudent = (int)PositionType.Student;
            //if (admissionDocument.Position == positionTypeStudent)
            //{
            //    // Да не се позволява повече от един запис с позиция „училище/ДГ“ за даден ученик и дадена институция.
            //    if (await _context.EducationalStates.AnyAsync(x => x.PersonId == admissionDocument.PersonId
            //        && x.InstitutionId == admissionDocument.InstitutionId
            //        && x.PositionId == positionTypeStudent))
            //    {
            //        return false;
            //    }
            //}

            return (true, "");
        }

        public virtual async Task GetInitialEnrollmentTargetClassDetails(StudentClassModel model)
        {
            if (model == null) return;

            var targetClass = await _context.ClassGroups
               .Where(x => x.ClassId == model.ClassId)
               .Select(x => new
               {
                   x.ClassId,
                   x.InstitutionId,
                   x.BasicClassId,
                   x.ClassTypeId,
                   x.ClassType.ClassKind,
                   x.ClassEduFormId,
                   x.IsNotPresentForm,
                   x.IsNotNpo109,
                   IsValid = x.IsValid ?? true // В базата има default(1)
               })
               .FirstOrDefaultAsync();

            if (targetClass == null)
            {
                return;
            }

            ValidationResult.TargetClassId = targetClass.ClassId;
            ValidationResult.TargetInstitutionId = targetClass.InstitutionId;
            ValidationResult.TargetBasicClassId = targetClass.BasicClassId;
            ValidationResult.TargetClassTypeId = targetClass.ClassTypeId;
            ValidationResult.TargetClassKindId = targetClass.ClassKind;
            ValidationResult.TargerClassEduFormId = targetClass.ClassEduFormId;
            ValidationResult.TargetClassIsNotPresentForm = targetClass.IsNotPresentForm;
            ValidationResult.TargetClassIsValid = targetClass.IsValid;
            ValidationResult.TargetClassIsNotNpo109 = targetClass.IsNotNpo109;

            var admissionDocument = await _context.AdmissionDocuments
                .Where(x => x.Id == model.AdmissionDocumentId)
                .Select(x => new
                {
                    x.Id,
                    x.PositionId
                })
                .SingleOrDefaultAsync();

            if (admissionDocument == null)
            {
                /// Има нужда за запис в група/паралелка без да е наличин документ за записване.
                /// Тогава позицията ще е вземем от EduState и ще я подадем на модела.

                ValidationResult.TargetPositionId = model.InitialEnrollmentPosition ?? (int)PositionType.Student; // default
            } else
            {
                ValidationResult.TargetPositionId = admissionDocument.PositionId;
            }
        }

        public virtual async Task GetInitialCplrEnrollmentTargetClassDetails(StudentClassModel model)
        {
            if (model == null || model.ClassGroups.IsNullOrEmpty()) return;

            var targetClasses = await _context.ClassGroups
               .Where(x => model.ClassGroups.Contains(x.ClassId))
               .Select(x => new
               {
                   x.ClassId,
                   x.ClassName,
                   x.InstitutionId,
                   x.BasicClassId,
                   x.ClassTypeId,
                   x.ClassType.ClassKind,
                   x.ClassEduFormId,
                   x.IsNotPresentForm,
                   x.ClassSpecialityId,
                   x.IsNotNpo109,
                   IsValid = x.IsValid ?? true // В базата има default(1)
               })
               .ToListAsync();

            ValidationResult.TargetPositionId = (int)PositionType.StudentPersDevelopmentSupport; // default;

            foreach (var classGroup in targetClasses)
            {
                ValidationResult.TargetClasses.Add(new EnrollmentTargetClass
                {
                    TargetPositionId = ValidationResult.TargetPositionId,
                    TargetClassId = classGroup.ClassId,
                    TargetClassName = classGroup.ClassName,
                    TargetBasicClassId = classGroup.BasicClassId,
                    TargetClassTypeId = classGroup.ClassTypeId,
                    TargetInstitutionId = classGroup.InstitutionId,             
                    TargerClassEduFormId = classGroup.ClassEduFormId,
                    TargerClassSpecialityId = classGroup.ClassSpecialityId,
                    TargetClassIsNotPresentForm = classGroup.IsNotPresentForm,
                    TargetClassIsValid = classGroup.IsValid,
                    TargetClassIsNotNpo109 = classGroup.IsNotNpo109,
            });

                ValidationResult.Merge(CheckRolePermission(classGroup.InstitutionId));
            }
        }

        public virtual async Task GetAdditionalEnrollmentTargetClassDetails(StudentClassBaseModel model)
        {
            if (model == null) return;

            var targetClass = await _context.ClassGroups
               .Where(x => x.ClassId == model.ClassId)
               .Select(x => new
               {
                   x.ClassId,
                   x.ClassName,
                   x.InstitutionId,
                   x.BasicClassId,
                   x.ClassTypeId,
                   x.ClassType.ClassKind,
                   x.ClassEduFormId,
                   x.IsNotPresentForm,
                   x.ClassSpecialityId,
                   x.IsNotNpo109,
                   IsValid = x.IsValid ?? true // В базата има default(1)
               })
               .FirstOrDefaultAsync();

            if (targetClass == null)
            {
                return;
            }

            ValidationResult.TargetClassId = targetClass.ClassId;
            ValidationResult.TargetInstitutionId = targetClass.InstitutionId;
            ValidationResult.TargetBasicClassId = targetClass.BasicClassId;
            ValidationResult.TargetClassTypeId = targetClass.ClassTypeId;
            ValidationResult.TargetClassKindId = targetClass.ClassKind;
            ValidationResult.TargerClassEduFormId = targetClass.ClassEduFormId;
            ValidationResult.TargetClassIsNotPresentForm = targetClass.IsNotPresentForm;
            ValidationResult.TargetClassIsValid = targetClass.IsValid;
            ValidationResult.TargerClassSpecialityId = targetClass.ClassSpecialityId;
            ValidationResult.TargetClassIsNotNpo109 = targetClass.IsNotNpo109;

            bool isCdoClassGroup = ValidationResult.TargetClassKindId == (int)ClassKindEnum.Cdo;

            switch (ValidationResult.TargetClassKindId)
            {
                case (int)ClassKindEnum.Cdo:
                    // При записване в ЦДО/общежитие Позицията в StudentClass = 3
                    ValidationResult.TargetPositionId = (int)PositionType.Student;
                    break;
                case (int)ClassKindEnum.Professional:
                    ValidationResult.TargetPositionId = (int)PositionType.ProfessionalEducation;
                    break;
                default:
                    ValidationResult.TargetPositionId = (int)PositionType.StudentPersDevelopmentSupport;
                    break;
            }
        }

        public virtual Task GetCplrAdditionalEnrollmentTargetClassDetail(StudentClassBaseModel model)
        {
            // Логиката съвпада с GetAdditionalEnrollmentTargetClassDetails
            return GetAdditionalEnrollmentTargetClassDetails(model);
        }

        /// <summary>
        /// 1. Проверка за невалиден или липсващ модел.
        /// 2. Проверка за невалидна или липсваща паралелка, в която ще записваме.
        /// 3. Проверка на правата.
        /// 4. Проверка на док. за записване, ако е посочен (дали е за дадения ученик и дали не е използван за запосване в паралелка вече).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<StudentClassEnrollmentValidationResult> ValidateInitialEnrollment(StudentClassModel model)
        {
            if (model == null)
            {
                ValidationResult.Errors.Add(Messages.EmptyModelError, nameof(model), nameof(StudentClassModel));
                return ValidationResult;
            }

            if (ValidationResult.TargetClassId == null)
            {
                ValidationResult.Errors.Add($"Липсва валидна група/паралелка за записване с ИД №: {model.ClassId}.");
                return ValidationResult;
            }

            if (!ValidationResult.TargetClassIsValid)
            {
                ValidationResult.Errors.Add($"Не позволено записването в закрита група/паралелка с ИД №: {model.ClassId}.");
                return ValidationResult;
            }

            ValidationResult.Merge(CheckRolePermission(ValidationResult.TargetInstitutionId));

            var admissionDocument = await _context.AdmissionDocuments
                .Where(x => x.Id == model.AdmissionDocumentId)
                .Select(x => new
                {
                    x.Id,
                    x.InstitutionId,
                    x.PersonId,
                    x.PositionId
                })
                .SingleOrDefaultAsync();

            if (admissionDocument != null && admissionDocument.PersonId != model.PersonId)
            {
                // Документът за записване не е обвързан с дадения ученик.
                ValidationResult.Errors.Add("Документът за записване не е обързан с дадения ученик.");
            }

            if (admissionDocument != null && await _context.StudentClasses.AnyAsync(x => x.AdmissionDocumentId == admissionDocument.Id))
            {
                // Докъментът за записване вече е използван за запис в паралелка.
                ValidationResult.Errors.Add("Докъментът за записване вече е използван за запис в паралелка.");
            }

            if (model.EntryDate.HasValue && model.EntryDate.Value.Date < (model.EnrollmentDate ?? DateTime.Now).Date)
            {
                ValidationResult.Errors.Add("Датата на постъпване не може да е преди датата на записване в група/паралелка.");
            }

            ValidationResult.Merge(await CheckInstitutionTypeToPosition(ValidationResult.TargetInstitutionId ?? default, ValidationResult.TargetPositionId));
            ValidationResult.Merge(await CheckInitialEnrollmentClassKindToInstitutionType(ValidationResult.TargetInstitutionId ?? default, ValidationResult.TargetClassKindId, ValidationResult.TargetClassIsNotPresentForm));

            //// Възможност за записване в неучебна група без запис в учебна паралелка #733
            //// https://github.com/Neispuo/students/issues/733
            //// Трябва да се даде възможност за записване на ученици в ClassKind = 3, без да има запис в ClassKind = 1 в същата институция.
            //// Това е при условие, че в EducationalState съществува запис за дадения ученик и институция с позиция 7(учащ (друга институция))

            var existingEduStates = await _context.EducationalStates
                .Where(x => x.PersonId == model.PersonId && x.InstitutionId.HasValue && x.InstitutionId == _userInfo.InstitutionID)
                .Select(x => new { x.EducationalStateId, x.PositionId })
                .ToListAsync();

            if (ValidationResult.TargetClassKindId == (int)ClassKindEnum.Other
                && !existingEduStates.Any(x => x.PositionId == (int)PositionType.StudentOtherInstitution))
            {
                // Когато записваме в група/паралелка с ClassKind = 3(Други групи) трябва да има запис в EducationalState за текущата институция
                // и дадения ученик с позиция 7(учащ (друга институция))
                ValidationResult.Errors.Add($"Липсва валидна паралелка за записване с ИД №: {ValidationResult.TargetClassId}.");
                return ValidationResult;
            }

            ValidationResult.Merge(await CheckInstitutionTypeToPosition(ValidationResult.TargetInstitutionId ?? default, ValidationResult.TargetPositionId));
            ValidationResult.Merge(await CheckResourceSupportClassGroup(model.PersonId, ValidationResult.TargetClassTypeId, ValidationResult.TargetClassId, ValidationResult.TargetClassIsNotNpo109));

            return ValidationResult;
        }

        /// <summary>
        /// 1. Проверка за невалиден или липсващ модел.
        /// 2. Проверка за невалидна или липсваща паралелка, в която ще записваме.
        /// 3. Проверка на правата.
        /// 4. Проверка на док. за записване, ако е посочен (дали е за дадения ученик и дали не е използван за запосване в паралелка вече).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<StudentClassEnrollmentValidationResult> ValidateInitialCplrEnrollment(StudentClassModel model)
        {
            ValidationResult.Merge(await CheckLoggedUserInstitutionType(InstitutionTypeEnum.PersonalDevelopmentSupportCenter));
            if (ValidationResult.HasErrors)
            {
                return ValidationResult;
            }

            if (model == null)
            {
                ValidationResult.Errors.Add(Messages.EmptyModelError, nameof(model), nameof(StudentClassModel));
                return ValidationResult;
            }

            if (model.ClassGroups.IsNullOrEmpty())
            {
                ValidationResult.Errors.Add("Не са избрани паралеки/гупи.");
                return ValidationResult;
            }

            ValidationResult.Merge(await CheckInstitutionTypeToPosition(_userInfo.InstitutionID ?? default, ValidationResult.TargetPositionId));

            if (!ValidationResult.TargetClasses.IsNullOrEmpty())
            {
                foreach (var classGroup in ValidationResult.TargetClasses)
                {
                    if (!classGroup.TargetClassIsValid)
                    {
                        ValidationResult.Errors.Add($"Не позволено записването в закрита група/паралелка с ИД №: {classGroup.TargetClassId}.");
                    }

                    ValidationResult.Merge(CheckRolePermission(classGroup.TargetInstitutionId));
                }
            }

            if (model.AdmissionDocumentId.HasValue)
            {
                var admissionDocument = await _context.AdmissionDocuments
                    .Where(x => x.Id == model.AdmissionDocumentId)
                    .Select(x => new
                    {
                        x.Id,
                        x.InstitutionId,
                        x.PersonId,
                        x.PositionId
                    })
                    .SingleOrDefaultAsync();

                if (admissionDocument != null && admissionDocument.PersonId != model.PersonId)
                {
                    // Документът за записване не е обвързан с дадения ученик.
                    ValidationResult.Errors.Add("Документът за записване не е обързан с дадения ученик.");
                }

                if (admissionDocument != null && await _context.StudentClasses.AnyAsync(x => x.AdmissionDocumentId == admissionDocument.Id))
                {
                    // Докъментът за записване вече е използван за запис в паралелка.
                    ValidationResult.Errors.Add("Докъментът за записване вече е използван за запис в паралелка.");
                }
            }

            if (model.EntryDate.HasValue && model.EntryDate.Value.Date < (model.EnrollmentDate ?? DateTime.Now).Date)
            {
                ValidationResult.Errors.Add("Датата на постъпване не може да е преди датата на записване в група/паралелка.");
            }

            ValidationResult.Merge(await CheckResourceSupportClassGroup(model.PersonId, ValidationResult.TargetClassTypeId, ValidationResult.TargetClassId, ValidationResult.TargetClassIsNotNpo109));

            return ValidationResult;
        }

        /// <summary>
        /// 1. Проверка за невалиден или липсващ модел.
        /// 2. Проверка за невалидна или липсваща паралелка, в която ще записваме.
        /// 3. Проверка на правата.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<StudentClassEnrollmentValidationResult> ValidateAdditionalClassEnrollment(StudentClassBaseModel model)
        {
            if (model == null)
            {
                ValidationResult.Errors.Add(Messages.EmptyModelError, nameof(model), nameof(StudentClassBaseModel));
                return ValidationResult;
            }

            if (ValidationResult.TargetClassId == null)
            {
                ValidationResult.Errors.Add($"Липсва валидна паралелка за записване с ИД №: {model.ClassId}.");
                return ValidationResult;
            }

            if (!ValidationResult.TargetClassIsValid)
            {
                ValidationResult.Errors.Add($"Не позволено записването в закрита група/паралелка с ИД №: {model.ClassId}.");
                return ValidationResult;
            }

            if (model.EntryDate.HasValue && model.EntryDate.Value.Date < (model.EnrollmentDate ?? DateTime.Now).Date)
            {
                ValidationResult.Errors.Add("Датата на постъпване не може да е преди датата на записване в група/паралелка.");
            }

            ValidationResult.Merge(CheckRolePermission(ValidationResult.TargetInstitutionId));
            ValidationResult.Merge(await CheckForDuplicateCdoClassGroup(model.PersonId, ValidationResult.TargetClassKindId));
            ValidationResult.Merge(await CheckAdditionalEnrollmentClassKindToInstitutionType(ValidationResult.TargetInstitutionId ?? default, ValidationResult.TargetClassKindId, ValidationResult.TargetClassTypeId));
            ValidationResult.Merge(await CheckEnrollUnenrollClassTypeLimitation(ValidationResult.TargetClassTypeId, true));
            ValidationResult.Merge(await CheckResourceSupportClassGroup(model.PersonId, ValidationResult.TargetClassTypeId, ValidationResult.TargetClassId, ValidationResult.TargetClassIsNotNpo109));

            return ValidationResult;
        }

        /// <summary>
        /// 1. Проверка за невалиден или липсващ модел.
        /// 2. Проверка за невалидна или липсваща паралелка, в която ще записваме.
        /// 3. Проверка на правата.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<StudentClassEnrollmentValidationResult> ValidateCplrAdditionalClassEnrollment(StudentClassBaseModel model)
        {
            await ValidateAdditionalClassEnrollment(model);

            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(ValidationResult.TargetInstitutionId ?? int.MinValue);
            if (institution == null)
            {
                ValidationResult.Errors.Add(Messages.InvalidInstitutionCodeError);
                return ValidationResult;
            }

            if (!institution.IsCPLR)
            {
                ValidationResult.Errors.Add(Messages.InvalidInstTypeError);
                return ValidationResult;
            }

            if (model.EntryDate.HasValue && model.EntryDate.Value.Date < (model.EnrollmentDate ?? DateTime.Now).Date)
            {
                ValidationResult.Errors.Add("Датата на постъпване не може да е преди датата на записване в група/паралелка.");
            }

            ValidationResult.Merge(await CheckResourceSupportClassGroup(model.PersonId, ValidationResult.TargetClassTypeId, ValidationResult.TargetClassId, ValidationResult.TargetClassIsNotNpo109));

            return ValidationResult;
        }

        /// <summary>
        /// Валидация на премстването в паралелка в рамките на една институция.
        /// 1. Проверка за невалиден или липсващ модел.
        /// 2. Проверка за невалидна или липсваща текуща паралелка. Текущата паралелка е задължителна.
        /// 3. Проверка на правата.
        /// 4. Проверка дали текущата паралелка е текуща(IsCurrent == true).
        /// 5. Проверка за невалидна или липсваща паралелка, в която ще преместваме.
        /// 6. Проверка дали паралелката, в която ще преместване има родител (ClassGroup.ParentClassID != null). Задължително записваме само в такива.
        /// 7. Може да се мести от паралелка в паралелка, ако те са един и същи вид - ClassType -- тази проверка не е актуална(М.Митова 15.12.2021)
        /// 8. Проверка дали PersonId-то на тукащата паралека съвпата с подаденото в модела.
        /// 9. Проверка дали институцията на текущата и новата паралелка е една и съща.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentStudentClass"></param>
        /// <returns></returns>\
        public virtual async Task<StudentClassEnrollmentValidationResult> ValidateChange(StudentClassModel model, StudentClass currentStudentClass)
        {
            if (model == null)
            {
                ValidationResult.Errors.Add(Messages.EmptyModelError, nameof(model), nameof(StudentClassModel));
                return ValidationResult;
            }

            if (currentStudentClass == null)
            {
                ValidationResult.Errors.Add($"Невалидна или липсваща текуща група/паралелка с ИД №: {model.CurrentStudentClassId}.");
                return ValidationResult;
            }

            ValidationResult.Merge(CheckRolePermission(currentStudentClass.Class?.InstitutionId));

            bool isCurrent = currentStudentClass?.IsCurrent ?? false;
            if (!isCurrent)
            {
                ValidationResult.Errors.Add($"Не е позволено преместването от група/паралелка, в която детето/ученикът не е записан текущо.");
                return ValidationResult;
            }

            if (ValidationResult.HasErrors) return ValidationResult;

            var targetClass = await _context.ClassGroups
                .Where(x => x.ClassId == model.ClassId)
                .Select(x => new
                {
                    x.ClassId,
                    x.ParentClassId,
                    x.InstitutionId,
                    x.BasicClassId,
                    x.ClassTypeId,
                    x.ClassType.ClassKind,
                    x.IsNotPresentForm,
                    IsValid = x.IsValid ?? true // в базата има default(1)
                })
                .FirstOrDefaultAsync();

            if (targetClass == null)
            {
                ValidationResult.Errors.Add($"Липсва валидна група/паралелка за записване с ИД №: {targetClass.ClassId}.");
                return ValidationResult;
            }

            if (!targetClass.IsValid)
            {
                ValidationResult.Errors.Add($"Не позволено записването в закрита група/паралелка с ИД №: {targetClass.ClassId}.");
                return ValidationResult;
            }

            //if (!targetClass.ParentClassId.HasValue)
            //{
            //    ValidationResult.Errors.Add($"Паралелката с ИД №: {targetClass.ClassId} няма родител.");
            //    return ValidationResult;
            //}

            int? currnetClassType = await _context.ClassGroups
               .Where(x => x.ClassId == currentStudentClass.ClassId)
               .Select(x => x.ClassTypeId)
               .SingleOrDefaultAsync();

            // 7. Може да се мести от паралелка в паралелка, ако те са един и същи вид - ClassType -- тази проверка не е актуална(М.Митова 15.12.2021)
            //if (!currnetClassType.HasValue || currnetClassType.Value != targetClass.ClassTypeId)
            //{
            //    ValidationResult.Errors.Add("Може да се мести от паралелка в паралелка, ако те са един и същи вид.");
            //    return ValidationResult;
            //}

            if (currentStudentClass.PersonId != model.PersonId)
            {
                ValidationResult.Errors.Add("Същеструва разминаване в ИД № на ученика.");
                return ValidationResult;
            }

            if (currentStudentClass.Class?.InstitutionId == null
                || currentStudentClass.Class?.InstitutionId != targetClass.InstitutionId)
            {
                ValidationResult.Errors.Add($"Съществува разминаване между институцията на текущата и институцията на избраната паралелка ({currentStudentClass.Class?.InstitutionId}/{targetClass.InstitutionId}).");
                return ValidationResult;
            }

            ValidationResult.TargetBasicClassId = targetClass.BasicClassId;
            ValidationResult.TargetClassTypeId = targetClass.ClassTypeId;
            ValidationResult.TargetClassKindId = targetClass.ClassKind;
            ValidationResult.TargetInstitutionId = targetClass.InstitutionId;
            ValidationResult.CurrentStudentClassId = currentStudentClass.Id;
            ValidationResult.TargetPositionId = currentStudentClass.PositionId;
            ValidationResult.TargetClassIsNotPresentForm = targetClass.IsNotPresentForm;
            ValidationResult.TargetClassIsValid = targetClass.IsValid;

            ValidationResult.Merge(await CheckResourceSupportClassGroup(model.PersonId, ValidationResult.TargetClassTypeId, ValidationResult.TargetClassId, ValidationResult.TargetClassIsNotNpo109));

            return ValidationResult;
        }

        public virtual async Task<StudentClassEnrollmentValidationResult> ValidateChange(StudentAdditionalClassChangeModel model, StudentClass currentStudentClass)
        {
            if (model == null)
            {
                ValidationResult.Errors.Add(Messages.EmptyModelError, nameof(model), nameof(StudentAdditionalClassChangeModel));
                return ValidationResult;
            }

            if (currentStudentClass == null)
            {
                ValidationResult.Errors.Add($"Невалидна или липсваща текуща група/паралелка с ИД №: {model.CurrentStudentClassId}.");
                return ValidationResult;
            }

            ValidationResult.Merge(CheckRolePermission(currentStudentClass.Class?.InstitutionId));

            bool isCurrent = currentStudentClass?.IsCurrent ?? false;
            if (!isCurrent)
            {
                ValidationResult.Errors.Add($"Ученикът не е записан текущо в паралелката/групата, от която желаете да го преместите.");
                return ValidationResult;
            }

            if (ValidationResult.HasErrors)
            {
                return ValidationResult;
            }

            var targetClass = await _context.ClassGroups
                .Where(x => x.ClassId == model.ClassId)
                .Select(x => new
                {
                    x.ClassId,
                    x.InstitutionId,
                    x.ClassTypeId,
                    x.BasicClassId,
                    x.ClassType.ClassKind,
                    x.IsNotPresentForm,
                    IsValid = x.IsValid ?? true // в базата има default(1)
                })
                .FirstOrDefaultAsync();

            if (targetClass == null)
            {
                ValidationResult.Errors.Add($"Липсва валидна група/паралелка за записване с ИД №: {targetClass.ClassId}.");
                return ValidationResult;
            }

            if (!targetClass.IsValid)
            {
                ValidationResult.Errors.Add($"Не позволено записването в закрита група/паралелка с ИД №: {targetClass.ClassId}.");
                return ValidationResult;
            }

            if (currentStudentClass.Class?.InstitutionId == null
                || currentStudentClass.Class?.InstitutionId != targetClass.InstitutionId)
            {
                ValidationResult.Errors.Add($"Съществува разминаване между институцията на текущата и институцията на избраната група/паралелка ({currentStudentClass.Class?.InstitutionId}/{targetClass.InstitutionId}).");
                return ValidationResult;
            }

            ValidationResult.TargetBasicClassId = targetClass.BasicClassId;
            ValidationResult.TargetClassTypeId = targetClass.ClassTypeId;
            ValidationResult.TargetClassKindId = targetClass.ClassKind;
            ValidationResult.TargetInstitutionId = targetClass.InstitutionId;
            ValidationResult.CurrentStudentClassId = currentStudentClass.Id;
            ValidationResult.TargetPositionId = currentStudentClass.PositionId;
            ValidationResult.TargetClassIsNotPresentForm = targetClass.IsNotPresentForm;
            ValidationResult.TargetClassIsValid = targetClass.IsValid;

            ValidationResult.Merge(await CheckResourceSupportClassGroup(model.PersonId, ValidationResult.TargetClassTypeId, ValidationResult.TargetClassId, ValidationResult.TargetClassIsNotNpo109));

            return ValidationResult;
        }

        public virtual async Task<StudentClassEnrollmentValidationResult> ValidateUnenrollment(StudentClass entity, StudentClassUnenrollmentModel model)
        {
            if (entity == null)
            {
                ValidationResult.Errors.Add(Messages.EmptyEntityError, nameof(entity), nameof(StudentClass));
                return ValidationResult;
            }

            var currentClass = await _context.ClassGroups
                .Where(x => x.ClassId == entity.ClassId)
                .Select(x => new
                {
                    x.ClassId,
                    x.InstitutionId,
                    x.ClassType.ClassKind
                })
                .SingleOrDefaultAsync();

            ValidationResult.Merge(CheckRolePermission(currentClass?.InstitutionId));
            if (ValidationResult.HasErrors) return ValidationResult;

            // Трябва да има възможност за отписване от втория клас (ако е от ClassKind <> 1), което не води до отписване от институция – бутон за Отписване от класа от timeline-a
            if (currentClass?.ClassKind == (int)ClassKindEnum.Basic)
            {
                ValidationResult.Errors.Add($"Не може да бъде отписан от неосновната група/парарелка, ако тя е от тип '{ClassKindEnum.Basic.GetEnumDescription()}'!");
                return ValidationResult;
            }

            if (model.DischargeDate < entity.EnrollmentDate)
            {
                ValidationResult.Errors.Add($"Датата на отписване {model.DischargeDate:yyyy-MM-dd} не може да е преди датата на записване {entity.EnrollmentDate:yyyy-MM-dd}.");

                return ValidationResult;
            }


            // Позволено отписване при следните условия:
            // 1. Не е избран външен доставчик на СО
            // 2. Избран е външен доставчик на СО, но паралелката е от ClassType, раличен от описаните в AppSettings.ExternalSoProviderClassTypeEnrollmentLimitation
            ValidationResult.Merge(await CheckEnrollUnenrollClassTypeLimitation(entity.Class?.ClassTypeId, false));
          

            //if (!await _context.StudentClasses.AnyAsync(x => x.PersonId == entity.PersonId && x.IsCurrent
            //    && x.ClassId != currentClass.ClassId))
            //{
            //    ValidationResult.Errors.Add("Не е записан в друга паралелка освен текущата!"));
            //    return ValidationResult;
            //}

            return ValidationResult;
        }

        public virtual async Task<StudentClassEnrollmentValidationResult> ValidateUpdate(StudentClassBaseModel model, StudentClass entity)
        {
            if (model == null)
            {
                ValidationResult.Errors.Add(Messages.EmptyModelError, nameof(entity), nameof(StudentClassBaseModel));
                return ValidationResult;
            }

            if (!entity.AuthorizeInstitution(_userInfo?.InstitutionID))
            {
                ValidationResult.Errors.Add(Messages.UnauthorizedMessageError);
                return ValidationResult;
            }

            if (!entity.IsCurrent)
            {
                ValidationResult.Errors.Add("Ученикът не е записан в избраната паралка/група!");
            }

            if (model.EntryDate.HasValue && model.EntryDate.Value.Date < (model.EnrollmentDate ?? DateTime.Now).Date)
            {
                ValidationResult.Errors.Add("Датата на постъпване не може да е преди датата на записване в група/паралелка.");
            }

            // Позволена е редакцията при следните условия:
            // 1. Паралелката е с ClassKind = 1
            // 2. Паралелката е служебна (липсва и ClassType, съответно ClassKind)
            // 3. Не е избран външен доставчик на СО
            // 4. Избран е външен доставчик на СО, но параалелката е от ClassType раличен от описаните в AppSettings.ExternalSoProviderClassTypeEnrollmentLimitation
            bool isNotPresentForm = entity.IsNotPresentForm ?? false;
            int? classKind = entity.ClassType?.ClassKind;

            if (!isNotPresentForm && classKind.HasValue && classKind.Value != (int)ClassKindEnum.Basic)
            {
                // Паралелката не е служебна и не е основна. Ще правим проверка в AppSettings.ExternalSoProviderClassTypeEnrollmentLimitation
                ValidationResult.Merge(await CheckEnrollUnenrollClassTypeLimitation(entity.ClassTypeId, true));
            }

            return ValidationResult;
        }

        public virtual async Task<StudentClassEnrollmentValidationResult> ValidatePositionChange(StudentPositionChangeModel model, StudentClass entity)
        {
            if (model == null)
            {
                ValidationResult.Errors.Add(Messages.EmptyModelError, nameof(entity), nameof(StudentClassBaseModel));
                return ValidationResult;
            }

            ValidationResult.Merge(CheckRolePermission(entity?.InstitutionId));

            if (!entity.IsCurrent)
            {
                ValidationResult.Errors.Add("Ученикът не е записан в избраната паралка/група!");
            }

            ValidationResult.Merge(await CheckInstitutionTypeToPosition(entity.InstitutionId, model.PositionId));


            if (model.PositionId == (int)PositionType.Student && await _context.StudentClasses.AnyAsync(x => x.PersonId == entity.PersonId
                            && x.IsCurrent && x.InstitutionId != entity.InstitutionId
                            && x.InstitutionSchoolYear.DetailedSchoolType.InstType == (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport
                            && x.ClassType.ClassKind == (int)ClassKindEnum.Basic))
            {
                ValidationResult.Errors.Add($"Не е позволено записване на дете/ученик с позиция: {PositionType.Student.GetEnumDescription()}. Детето/ученикът е записан в учебна група/паралелка в ЦСОП.");
            }

            if (model.PositionId == (int)PositionType.StudentSpecialNeeds && !await _context.StudentClasses.AnyAsync(x => x.PersonId == entity.PersonId
                            && x.IsCurrent && x.InstitutionId != entity.InstitutionId
                            && x.InstitutionSchoolYear.DetailedSchoolType.InstType == (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport
                            && x.ClassType.ClassKind == (int)ClassKindEnum.Basic))
            {
                ValidationResult.Errors.Add($"Не е позволено записване на дете/ученик с позиция: {PositionType.StudentSpecialNeeds.GetEnumDescription()}. Детето/ученикът не е записан в учебна група/паралелка в ЦСОП.");
            }

            return ValidationResult;
        }

        /// <summary>
        /// Проверка за наличето на подходяща роля на потребителя.
        /// Разрешените роли са "Институция"(School) и "Институция (техн. сътрудник)"(InstitutionAssociate).
        /// Освен това се проверява дали подаденото institutionId съвпада с това на логнатия потребител.
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        protected StudentClassEnrollmentValidationResult CheckRolePermission(int? institutionId)
        {
            var checkResult = new StudentClassEnrollmentValidationResult();

            if (!institutionId.HasValue)
            {
                checkResult.Errors.Add(new ValidationError(Messages.UnauthorizedMessageError));
                return checkResult;
            }

            if (!_userInfo.IsSchoolDirector && !_userInfo.IsInstitutionAssociate)
            {
                checkResult.Errors.Add(new ValidationError(Messages.UnauthorizedMessageError, "Unauthorized. !IsSchoolDirector && !IsInstitutionAssociate"));
                return checkResult;
            }

            if (_userInfo.InstitutionID != institutionId.Value)
            {
                checkResult.Errors.Add(new ValidationError(Messages.UnauthorizedMessageError, "Unauthorized. InstitutionId to user institutionId mismatch."));
                return checkResult;
            }

            return checkResult;
        }

        /// <summary>
        /// Проверка за позволена позиция спрямо типа на институцията, в която записваме.
        /// Позволените позиции за описани в student.AppSettings, ключ InstTypeToPositionLimit.
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        protected async Task<StudentClassEnrollmentValidationResult> CheckInstitutionTypeToPosition(int institutionId, int position)
        {
            var checkResult = new StudentClassEnrollmentValidationResult();

            if (false == Enum.IsDefined(typeof(PositionType), position))
            {
                checkResult.Errors.Add(new ValidationError($"Неразпознат позиция с ИД №: \"{position}\" за институция с ИД №: {institutionId}"));
                return checkResult;
            }

            HashSet<int> allowedPositions = await _institutionService.GetAllowedStudentPositions(institutionId);
            if (!allowedPositions.Contains(position))
            {
                checkResult.Errors.Add(new ValidationError($"Неразпозната позиция с ИД №: \"{position}\" за институция с ИД №: {institutionId}"));
            }

            return checkResult;
        }

        /// <summary>
        /// Проверка за позволен вид паралелка(ClassKind) спрямо типа на институцията, в която записваме.
        /// Проверката е при запис в основна група/паралалке, при запис през документ за записване.
        /// Позволените позиции за описани в student.AppSettings, ключ InstTypeToClassKindEnrollmentLimit.
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        protected async Task<StudentClassEnrollmentValidationResult> CheckInitialEnrollmentClassKindToInstitutionType(int institutionId, int? classKindId, bool? isNotPresentForm)
        {
            var checkResult = new StudentClassEnrollmentValidationResult();
            if (!classKindId.HasValue) return checkResult;

            HashSet<int> allowedClassKinds = (await _institutionService.GetAllowedClasssKindsEnrollmentLimit(institutionId))?.InitialEnrollment?.AllowedClassKind;
            if (allowedClassKinds == null)
            {
                checkResult.Errors.Add(new ValidationError("Не са описани ограниченията между тип на институця и тип на група/паралека!"));
                return checkResult;
            }

            if (isNotPresentForm != true && !allowedClassKinds.Contains(classKindId.Value))
            {
                ClassKindEnum classKind = (ClassKindEnum)classKindId.Value;
                checkResult.Errors.Add(new ValidationError($"Непозволен тип паралелка: \"{classKind.GetEnumDescription()}\" за институция с ИД №: {institutionId}"));
            }

            return checkResult;
        }

        /// <summary>
        /// Проверка за позволен вид/тип паралелка(ClassKind/ClassType) спрямо типа на институцията, в която записваме.
        /// Проверката е при запис в допълнителна група/паралалке.
        /// Позволените позиции за описани в student.AppSettings, ключ InstTypeToClassKindEnrollmentLimit.
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        protected async Task<StudentClassEnrollmentValidationResult> CheckAdditionalEnrollmentClassKindToInstitutionType(int institutionId, int? classKindId, int? classTypeId)
        {
            var checkResult = new StudentClassEnrollmentValidationResult();
            if (!classKindId.HasValue) return checkResult;

            InstTypeToClassTypeConfiguration classKindConfig = await _institutionService.GetAllowedClasssKindsEnrollmentLimit(institutionId);
            if (classKindConfig == null || (classKindConfig.SingleEnrollment == null && classKindConfig.MultipleEntrollment == null))
            {
                checkResult.Errors.Add(new ValidationError("Не са описани ограниченията между тип на институция и тип/вид на група/паралелка."));
                return checkResult;
            }

            if (!classKindConfig.IsValid(classKindId, classTypeId))
            {
                InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(institutionId);
                checkResult.Errors.Add(new ValidationError($"Несъответствие между тип на институция ({((InstitutionTypeEnum)institution.InstTypeId).GetEnumDescription()})" +
                    $" и тип({classTypeId})/вид({classKindId}) на група/паралелка."));
            }

            return checkResult;
        }

        /// <summary>
        /// Проверка за ограничени ClassType на група паралелка в зависимост от ExternalSoProviderClassTypeEnrollmentLimitation конфигурацията в student.AppSettings.
        /// При избран външен доставчик на СО за текуюата учебна година институцията следва да е ограничена за записва в групи/паралелки от определен ClassType прес НЕИСПУО
        /// </summary>
        /// <returns></returns>
        protected async Task<StudentClassEnrollmentValidationResult> CheckEnrollUnenrollClassTypeLimitation(int? classTypeId, bool isEnrollment)
        {
            var checkResult = new StudentClassEnrollmentValidationResult();
            if (!classTypeId.HasValue) return checkResult;

            // В AppSettings.ExternalSoProviderLimitationsCheck не е избрано да правим проверка
            // или логнатата институция не е избрала външен доставчик на СО. Нямаме ограничения
            if (!_institutionService.ExternalSoProviderLimitationsCheck
                || !await _institutionService.HasExternalSoProviderForLoggedInstitution())
            {
                return checkResult;
            }

            if (_institutionService.ExternalSoProviderClassTypeEnrollmentLimitation.Contains(classTypeId.Value))
            {
                string classTypeName = await _context.ClassTypes.Where(x => x.ClassTypeId == classTypeId.Value)
                    .Select(x => x.Name)
                    .SingleOrDefaultAsync();

                checkResult.Errors.Add(new ValidationError($"Избран е външен доставчик на СО. {(isEnrollment ? "Записване" : "Отписване")} на деца/ученици в група от вид \"{classTypeName}\" се извърша от външното приложение."));
            }

            return checkResult;
        }

        /// <summary>
        /// Проверка за вече от един записан в група/паралелка от тип ЦДО.
        /// Не може дете да се запише в повече от едно ЦДО.;
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="classKindId"></param>
        /// <returns></returns>
        protected async Task<StudentClassEnrollmentValidationResult> CheckForDuplicateCdoClassGroup(int personId, int? classKindId)
        {
            var checkResult = new StudentClassEnrollmentValidationResult();

            if (classKindId == (int)ClassKindEnum.Cdo)
            {
                if (await _context.StudentClasses.AnyAsync(x => x.PersonId == personId
                    && x.IsCurrent
                    && x.Class.ClassType.ClassKind == (int)ClassKindEnum.Cdo))
                {
                    checkResult.Errors.Add(new ValidationError($"Не е позволено записването в повече от една/един клас/група от тип: \"{ClassKindEnum.Cdo.GetEnumDescription()}\"."));
                }
            }

            // Todo: Да се проверява за единствен запис в общежитие(ClassKind == 3 и ClassType 39 или 49)

            return checkResult;
        }

        /// <summary>
        /// При записване на дете/ученик в група за РП няма да се допуска броят на децата/учениците да надвишава дванадесет.
        /// Ако IsNotNPO109 = 0, не може да се записват повече от 12 деца в група с ClassType = 37
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="clasTypeId"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        protected async Task<StudentClassEnrollmentValidationResult> CheckResourceSupportClassGroup(int personId, int? clasTypeId, int? classId, bool isNotNpo109)
        {
            var checkResult = new StudentClassEnrollmentValidationResult();

            if (clasTypeId != 37 || !classId.HasValue || isNotNpo109)
            {
                return checkResult;
            }

            int studentsCount = await _context.StudentClasses
                .Where(x => x.ClassId == classId.Value && x.PersonId != personId && x.IsCurrent)
                .CountAsync();
            int maxCount = 12;
            if (studentsCount >= maxCount) {
                checkResult.Errors.Add(new ValidationError($"Броят на децата/учениците, записани в избраната група, надвишава {maxCount}."));
            }

            return checkResult;
        }

        public Task<ApiValidationResult> VisibleAddToNewClassBtnCheck(int personId)
        {
            ValidationResult.Merge(CheckRolePermission(_userInfo?.InstitutionID));

            // Todo да се провери дали е записван в основна паралелка и др.

            return Task.FromResult((ApiValidationResult)ValidationResult);
        }

        private async Task<ApiValidationResult> CheckLoggedUserInstitutionType(InstitutionTypeEnum instType)
        {
            var checkResult = new StudentClassEnrollmentValidationResult();

            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(_userInfo?.InstitutionID ?? default);
            if (institution?.InstTypeId != (int)instType)
            {
                checkResult.Errors.Add(new ValidationError(Messages.InvalidInstTypeError));
            }

            return checkResult;
        }
    }
}
