namespace MonProjects.Infrastructure.Middlewares
{
    using Constants;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Http;
    using Services.Logs;
    using Services.CertificateValidate;
    using Services.Models.CertificateValidate;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public class CertificateAuthenticationMiddleware
    {
        private readonly RequestDelegate next;

        public CertificateAuthenticationMiddleware(
            RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            X509Certificate2 cert = await httpContext
                                            .Connection?
                                            .GetClientCertificateAsync();

            if (object.Equals(cert, null))
            {
                string userCertValue = httpContext
                                        .Request
                                        .Headers
                                        .Where(x => x.Key == "X-Client-Cert")
                                        .Select(x => x.Value
                                                      .FirstOrDefault())
                                        .FirstOrDefault();

                ILogService logService = httpContext
                                            .RequestServices?
                                            .GetService(typeof(ILogService)) as ILogService;

                int result = await logService.CreateLogAsync(userCertValue);

                await httpContext.Response.WriteToJsonAsync(StatusCodes.Status401Unauthorized, AppExceptions.NoCertificateExceptionMessage);
                return;
            }

            ICertificateValidateService certificateValidateService = httpContext
                                                                        .RequestServices?
                                                                        .GetService(typeof(ICertificateValidateService)) as ICertificateValidateService;

            if (object.Equals(certificateValidateService, null))
            {
                await httpContext.Response.WriteToJsonAsync(StatusCodes.Status404NotFound, AppExceptions.NoCertificateValidationServiceExceptionMessage);
                return;
            }

            CertificateDataDapperModel certificateData = await certificateValidateService.GetCertificateDataAsync(cert.Thumbprint);

            if (object.Equals(certificateData, null))
            {
                await httpContext.Response.WriteToJsonAsync(StatusCodes.Status403Forbidden, AppExceptions.NoValidCertificateDataExistExceptionMessage);
                return;
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(AppClaims.AppClaimTypeExtSystemId, certificateData.ExtSystemId.ToString()),
                new Claim(AppClaims.AppClaimTypeThumbprint, cert.Thumbprint),
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CertificateAuthentication.Type);

            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);

            httpContext.User = userPrincipal;
            
            await next(httpContext);
        }
    }
}
