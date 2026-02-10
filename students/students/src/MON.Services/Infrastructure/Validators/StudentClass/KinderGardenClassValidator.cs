namespace MON.Services.Infrastructure.Validators
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Enums;
    using MON.Services.Interfaces;
    using MON.Shared.Enums;
    using MON.Shared.Extensions;
    using MON.Shared.Interfaces;
    using System.Linq;
    using System.Threading.Tasks;

    public class KinderGardenClassValidator : StudentClassBaseValidator
    {
        public KinderGardenClassValidator(MONContext context, IUserInfo userInfo, IInstitutionService institutionService)
            : base(context, userInfo, institutionService)
        {
        }

        public override async Task<StudentClassEnrollmentValidationResult> ValidateInitialEnrollment(StudentClassModel model)
        {
            await base.ValidateInitialEnrollment(model);

            // Когато имаме Позиция = 3 или 7 ClassKind трябва да е 1
            if (ValidationResult.TargetPositionId == (int)PositionType.Student
                || ValidationResult.TargetPositionId == (int)PositionType.StudentOtherInstitution)
            {
                int? classKind = await _context.ClassGroupAlls
                    .Where(x => x.ClassId == model.ClassId)
                    .Select(x => x.ClassKind)
                    .SingleOrDefaultAsync();

                if (!classKind.HasValue || classKind.Value != 1)
                {
                    ValidationResult.Errors.Add("Когато имаме Позиция = 3 или 7 (но в InstType in 1, 2) ClassKind трябва да е 1");
                }
            }

            return ValidationResult;
        }

        public override async Task<StudentClassEnrollmentValidationResult> ValidateInitialCplrEnrollment(StudentClassModel model)
        {
            await base.ValidateInitialCplrEnrollment(model);

            if (ValidationResult.TargetPositionId != (int)PositionType.StudentPersDevelopmentSupport)
            {
                ValidationResult.Errors.Add($"Не е позволена позиция различна от {PositionType.StudentPersDevelopmentSupport.GetEnumDescription()}.");
            }

            return ValidationResult;
        }

        public override async Task<StudentClassEnrollmentValidationResult> ValidateAdditionalClassEnrollment(StudentClassBaseModel model)
        {
            await base.ValidateAdditionalClassEnrollment(model);

            bool isCdoClassGroup = ValidationResult.TargetClassKindId == (int)ClassKindEnum.Cdo;
            if (isCdoClassGroup)
            {
                // Трябва да има StudentClass с позиция 3 и IsCurrent = 1 и ClassKind = 1.
                bool hasStudentClassInPositionStudent = await _context.StudentClasses.AnyAsync(x => x.PersonId == model.PersonId
                    && x.IsCurrent && x.PositionId == (int)PositionType.Student && x.ClassType.ClassKind == (int)ClassKindEnum.Basic);

                if (!hasStudentClassInPositionStudent)
                {
                    ValidationResult.Errors.Add($"Необходимо е да има запис в текуща група/паралелка с позиция: \"{PositionType.Student.GetEnumDescription()}\" от тип: \"{ClassKindEnum.Basic.GetEnumDescription()}\".");
                    return ValidationResult;
                }
            }

            return ValidationResult;
        }
    }
}
