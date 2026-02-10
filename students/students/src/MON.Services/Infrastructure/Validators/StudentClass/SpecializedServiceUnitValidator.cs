namespace MON.Services.Infrastructure.Validators
{
    using MON.DataAccess;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;

    public class SpecializedServiceUnitValidator : StudentClassBaseValidator
    {
        public SpecializedServiceUnitValidator(MONContext context, IUserInfo userInfo, IInstitutionService institutionService)
         : base(context, userInfo, institutionService)
        {
        }
    }
}

