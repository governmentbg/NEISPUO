namespace MonProjects.Infrastructure.Extensions
{
    using Constants;
    using System;
    using System.Security.Claims;

    public static class ClaimsPrincipalExtensions
    {
        public static int GetCertificateExtSystemId(this ClaimsPrincipal claims)
            => Convert.ToInt32(GetClaimValue(AppClaims.AppClaimTypeExtSystemId, claims)); 
        public static string GetCertificateTrumbprint(this ClaimsPrincipal claims)
            => GetClaimValue(AppClaims.AppClaimTypeThumbprint, claims); 
        public static string GetProcedureName(this ClaimsPrincipal claims)
            => GetClaimValue(AppClaims.AppClaimTypeProcedureName, claims);
        public static string GetProcedureParameters(this ClaimsPrincipal claims)
            => GetClaimValue(AppClaims.AppClaimTypeProcedureParameters, claims);
        public static bool IsProcedureReturnArray(this ClaimsPrincipal claims)
            => Convert.ToBoolean(GetClaimValue(AppClaims.AppClaimTypeIsProcedureReturnArray, claims));
        private static string GetClaimValue(string claimType, ClaimsPrincipal claims)
             => claims.FindFirst(claimType)?.Value;
    }
}
