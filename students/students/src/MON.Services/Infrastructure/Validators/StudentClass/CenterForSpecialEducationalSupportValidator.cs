namespace MON.Services.Infrastructure.Validators
{
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Enums;
    using MON.Services.Interfaces;
    using MON.Shared.Enums;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;

    public class CenterForSpecialEducationalSupportValidator : StudentClassBaseValidator
    {
        public CenterForSpecialEducationalSupportValidator(MONContext context, IUserInfo userInfo, IInstitutionService institutionService)
            : base(context, userInfo, institutionService)
        {

        }

        public override async Task GetAdditionalEnrollmentTargetClassDetails(StudentClassBaseModel model)
        {
            await base.GetAdditionalEnrollmentTargetClassDetails(model);

            bool isCdoClassGroup = ValidationResult.TargetClassKindId == (int)ClassKindEnum.Cdo;

            // При записване в ЦДО Позицията в StudentClass = 7
            ValidationResult.TargetPositionId = isCdoClassGroup
                ? (int)PositionType.StudentOtherInstitution
                : (int)PositionType.StudentPersDevelopmentSupport;
        }
    }
}
