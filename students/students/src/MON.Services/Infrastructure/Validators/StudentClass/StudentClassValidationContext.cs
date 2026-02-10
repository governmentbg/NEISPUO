namespace MON.Services.Infrastructure.Validators
{
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Class;
    using MON.Services.Interfaces;
    using MON.Shared.Enums;
    using MON.Shared.Interfaces;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    public class StudentClassValidationContext
    {
        private readonly MONContext _context;
        private readonly IUserInfo _userInfo;
        private readonly IInstitutionService _institutionService;
        public IStudentClassValidator Validator { get; private set; }

        public StudentClassValidationContext(MONContext context, IUserInfo userInfo, IInstitutionService institutionService)
        {
            _context = context;
            _userInfo = userInfo;
            _institutionService = institutionService;
            if (_userInfo?.InstitutionID != null)
            {
                Validator = GetValidator(_userInfo?.InstitutionID ?? throw new ArgumentNullException("InstitutionID")).Result
                    ?? throw new ArgumentNullException(nameof(Validator), nameof(IStudentClassValidator));
            }
            else
            {
                Validator = new GenericClassValidator(_context, _userInfo, _institutionService);
            }
        }

        private async Task<IStudentClassValidator> GetValidator(int institutionId)
        {
            var institution = await _institutionService.GetInstitutionCache(institutionId);
            int? institutionTypeId = institution.InstTypeId;

            if (!institutionTypeId.HasValue) return null;

            return (InstitutionTypeEnum)institutionTypeId.Value switch
            {
                InstitutionTypeEnum.School => new SchoolClassValidator(_context, _userInfo, _institutionService),
                InstitutionTypeEnum.KinderGarden => new KinderGardenClassValidator(_context, _userInfo, _institutionService),
                InstitutionTypeEnum.PersonalDevelopmentSupportCenter => new PersonalDevelopmentSupportCenterClassValidator(_context, _userInfo, _institutionService),
                InstitutionTypeEnum.CenterForSpecialEducationalSupport => new CenterForSpecialEducationalSupportValidator(_context, _userInfo, _institutionService),
                InstitutionTypeEnum.SpecializedServiceUnit => new SpecializedServiceUnitValidator(_context, _userInfo, _institutionService),
                _ => null,
            };
        }

        /// <summary>
        /// Взима детайли за паралелката/групата в която ще записваме и определя позицията е StudentClass
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task GetInitialEnrollmentTargetClassDetails(StudentClassModel model)
        {
            return Validator.GetInitialEnrollmentTargetClassDetails(model);
        }

        /// <summary>
        /// Взима детайли за паралелката/групата в която ще записваме и определя позицията е StudentClass
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task GetInitialCplrEnrollmentTargetClassDetails(StudentClassModel model)
        {
            return Validator.GetInitialCplrEnrollmentTargetClassDetails(model);
        }

        /// <summary>
        /// Взима детайли за паралелката/групата в която ще записваме и определя позицията е StudentClass
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task GetAdditionalEnrollmentTargetClassDetail(StudentClassBaseModel model)
        {
            return Validator.GetAdditionalEnrollmentTargetClassDetails(model);
        }

        /// <summary>
        /// Взима детайли за паралелката/групата в която ще записваме и определя позицията е StudentClass
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task GetCplrAdditionalEnrollmentTargetClassDetail(StudentClassBaseModel model)
        {
            return Validator.GetCplrAdditionalEnrollmentTargetClassDetail(model);
        }

        public Task<StudentClassEnrollmentValidationResult> ValidateEnrollment(StudentClassModel model)
        {
            return Validator.ValidateInitialEnrollment(model);
        }

        public Task<StudentClassEnrollmentValidationResult> ValidateCprlEnrollment(StudentClassModel model)
        {
            return Validator.ValidateInitialCplrEnrollment(model);
        }

        public Task<StudentClassEnrollmentValidationResult> ValidateAdditionalClassEnrollment(StudentClassBaseModel model)
        {
            return Validator.ValidateAdditionalClassEnrollment(model);
        }

        public Task<StudentClassEnrollmentValidationResult> ValidateCplrAdditionalClassEnrollment(StudentClassBaseModel model)
        {
            return Validator.ValidateCplrAdditionalClassEnrollment(model);
        }

        public Task<StudentClassEnrollmentValidationResult> ValidateUpdate(StudentClassBaseModel model, StudentClass entity)
        {
            return Validator.ValidateUpdate(model, entity);
        }

        public Task<StudentClassEnrollmentValidationResult> ValidateUnenrollment(StudentClass entity, StudentClassUnenrollmentModel model)
        {
            return Validator.ValidateUnenrollment(entity, model);
        }

        public Task<StudentClassEnrollmentValidationResult> ValidateChange(StudentClassModel model, StudentClass currentStudentClass)
        {
            return Validator.ValidateChange(model, currentStudentClass);
        }

        public Task<StudentClassEnrollmentValidationResult> ValidateChange(StudentAdditionalClassChangeModel model, StudentClass currentStudentClass)
        {
            return Validator.ValidateChange(model, currentStudentClass);
        }

        public Task<(bool showInitialEntollmentButtonCheck, string showInitialEntollmentButtonCheckError)> ShowInitialEntollmentButton(AdmissionDocumentViewModel admissionDocument)
        {
            return Validator.ShowInitialEntollmentButton(admissionDocument);
        }

        public Task<StudentClassEnrollmentValidationResult> ValidatePositionChange(StudentPositionChangeModel model, StudentClass entity)
        {
            return Validator.ValidatePositionChange(model, entity);
        }

        internal Task<ApiValidationResult> AddToNewClassBtnVisibilityCheck(int personId)
        {
            return Validator.VisibleAddToNewClassBtnCheck(personId);
        }

        //private async Task<bool> InitialClassEnrollmentAllowed(int personId, int institutionId, int positionId)
        //{
        //    if (institutionId != _userInfo.InstitutionID) return false;

        //    PositionType positionType = (PositionType)positionId;
        //    switch (positionType)
        //    {
        //        case PositionType.Student:
        //            var eduState = await _context.EducationalStates.Where(x => x.PersonId == personId && x.PositionId == (int)PositionType.Student)
        //                .Select(x => new
        //                {
        //                    x.InstitutionId,
        //                    x.PositionId
        //                })
        //                .ToListAsync();

        //            if (eduState.Count == 0) return true; // Няма нищо в EducationalStates с позиция 3 - ЗАПИСВАЙ.

        //            // Todo: да се провери
        //            int currentUserInstId = _userInfo.InstitutionID ?? int.MinValue;
        //            if (!eduState.Any(x => x.InstitutionId == currentUserInstId)) return false; // Няма нищо в EducationalStates с InstitutionId различна от тази на логнатия потребител - НЕ ЗАПИСВАЙ.

        //            var sc = await _context.StudentClasses.Where(x => x.PersonId == personId && x.IsCurrent)
        //                .Select(x => new
        //                {
        //                    x.ClassType.ClassKind,
        //                    x.PositionId
        //                })
        //                .ToListAsync();
        //            if (sc.Count == 0) return true;  // Няма нищо в StudentClasses с IsCurrent == true - ЗАПИСВАЙ.

        //            if (!sc.Any(x => x.PositionId == (int)PositionType.Student)) return true; // Няма StudentClasses с позиция 3 - ЗАПИСВАЙ.

        //            /// ClassKind:
        //            /// 1	Групи/Класове       
        //            /// 2	Групи в ЦДО         
        //            /// 3	Други групи    
        //            if (!sc.Any(x => x.ClassKind == 1)) return true; // Има StudentClasses с позиция 3, но няма с ClassKind = 1 (Групи/Класове) - ЗАПИСВАЙ.

        //            bool hasRelocDocument = await _context.RelocationDocuments.AnyAsync(x => x.PersonId == personId && !x.AdmissionDocuments.Any());
        //            if (hasRelocDocument) return true; // Има StudentClasses с позиция 3 i с ClassKind = 1 (Групи/Класове), но и има RelocationDocument, който не е използван - ЗАПИСВАЙ.

        //            return false;
        //        case PositionType.StudentSpecialNeeds:
        //        case PositionType.Discharged:
        //        case PositionType.StudentOtherInstitution:
        //        case PositionType.StudentPersDevelopmentSupport:
        //        case PositionType.Staff:
        //        default:
        //            return true; // ЗАПИСВАЙ. Проверка се прави само за позиция 3 PositionType.Student.
        //    }
        //}
    }
}
