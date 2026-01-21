using MON.Shared.Interfaces;

namespace MON.Services.Extensions
{
    public static class SecurityExtensions
    {
        public static bool AuthorizeInstitution(this IInstitution entity, int? intitutionId)
        {
            return entity != null && entity.InstitutionId.HasValue && intitutionId.HasValue
                && entity.InstitutionId == entity.InstitutionId;
        }

        public static bool AuthorizeInstitution(this IInstitutionNotNullable entity, int? intitutionId)
        {
            return entity != null && intitutionId.HasValue
                && entity.InstitutionId == entity.InstitutionId;
        }
    }
}
