namespace MON.Services.Infrastructure.Validators
{
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Enums;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;

    public class PersonalDevelopmentSupportCenterClassValidator : StudentClassBaseValidator
    {
        public PersonalDevelopmentSupportCenterClassValidator(MONContext context, IUserInfo userInfo, IInstitutionService institutionService)
            : base(context, userInfo, institutionService)
        {
        }

        public override async Task GetAdditionalEnrollmentTargetClassDetails(StudentClassBaseModel model)
        {
            await base.GetAdditionalEnrollmentTargetClassDetails(model);

            ValidationResult.TargetPositionId = (int)PositionType.StudentPersDevelopmentSupport;
        }
    }
}
