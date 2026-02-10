using MON.DataAccess;
using MON.Models;
using MON.Services.Interfaces;
using MON.Shared.Interfaces;
using System.Threading.Tasks;
namespace MON.Services.Infrastructure.Validators
{
    public class GenericClassValidator : StudentClassBaseValidator
    {
        public GenericClassValidator(MONContext context, IUserInfo userInfo, IInstitutionService institutionService)
            : base(context, userInfo, institutionService)
        {
        }

        public override async Task<StudentClassEnrollmentValidationResult> ValidateInitialEnrollment(StudentClassModel model)
        {
            await base.ValidateInitialEnrollment(model);

            return ValidationResult;
        }

        public override async Task<StudentClassEnrollmentValidationResult> ValidateAdditionalClassEnrollment(StudentClassBaseModel model)
        {
            await base.ValidateAdditionalClassEnrollment(model);

            return ValidationResult;
        }
    }
}
