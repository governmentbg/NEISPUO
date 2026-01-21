namespace MonProjects.Infrastructure.Middlewares
{
    using Constants;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Http;
    using Services.ExtSystem;
    using Services.Models.ExtSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class ProjectQueryStringValidationMiddleware
    {
        private readonly RequestDelegate next;

        public ProjectQueryStringValidationMiddleware(
            RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {

            bool queryExist = httpContext.Request.QueryString.HasValue;

            if (!queryExist)
            {
                await httpContext.Response.WriteToJsonAsync(StatusCodes.Status400BadRequest, AppExceptions.NoQueryStringExistExceptionMessage);
                return;
            }

            string queryString = httpContext.Request.QueryString.Value.Substring(1);

            IEnumerable<string> queryParts = queryString.Split(new char[] { AppConstants.SlashChar }, StringSplitOptions.RemoveEmptyEntries);

            IExtSystemServices extSystemService = httpContext
                                                    .RequestServices?
                                                    .GetService(typeof(IExtSystemServices)) as IExtSystemServices;

            if (object.Equals(extSystemService, null))
            {
                await httpContext.Response.WriteToJsonAsync(StatusCodes.Status404NotFound, AppExceptions.NoExtSystemServiceExceptionMessage);
                return;
            }

            int extSystemId = httpContext
                                .User
                                .GetCertificateExtSystemId();

            IEnumerable<ServiceDataDapperModel> allowedServices = await extSystemService
                                                                        .GetServicesAsync(extSystemId);

            if (!allowedServices.Any())
            {
                await httpContext.Response.WriteToJsonAsync(StatusCodes.Status400BadRequest, AppExceptions.NoValidServicesExistExceptionMessage);
                return;
            }

            bool isServiceNameExist = allowedServices.Any(x => x.ServiceName.Equals(queryParts.FirstOrDefault()));

            if (!isServiceNameExist)
            {
                await httpContext.Response.WriteToJsonAsync(StatusCodes.Status400BadRequest, AppExceptions.NoValidServiceNameExistExceptionMessage);
                return;
            }

            ServiceDataDapperModel serviceData = allowedServices
                                                    .Where(x => x.ServiceName.ToLower().Equals(queryParts.FirstOrDefault().ToLower()))
                                                    .FirstOrDefault();

            if (object.Equals(serviceData, null))
            {
                await httpContext.Response.WriteToJsonAsync(StatusCodes.Status404NotFound, AppExceptions.NoServiceDataExistExceptionMessage);
                return;
            }

            string parameters = string.Empty;

            if (!string.IsNullOrWhiteSpace(serviceData.ServiceParameters)
                && queryParts.Skip(1).Count() > 0)
            {
                List<string> parametersParts = new List<string>();
                List<string> queryParametersParts = queryParts.Skip(1).ToList();
                List<string> serviceParametersParts = serviceData
                                                    .ServiceParameters
                                                    .Split(new char[] { AppConstants.PipeChar }, StringSplitOptions.RemoveEmptyEntries)
                                                    .ToList();

                if (queryParametersParts.Count != serviceParametersParts.Count)
                {
                    await httpContext.Response.WriteToJsonAsync(StatusCodes.Status400BadRequest, AppExceptions.NoValidCountOfQueryParametersExceptionMessage);
                    return;
                }

                for (int i = 0; i < serviceParametersParts.Count; i++)
                {
                    parametersParts.Add($"{serviceParametersParts[i]}:{queryParametersParts[i]}");
                }

                if (queryParts.FirstOrDefault().ToLower().Equals("getcbdata"))
                {
                    parametersParts.Add($"extSystemId:{extSystemId}");
                }

                parameters = string.Join(AppConstants.PipeString, parametersParts);                    
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(AppClaims.AppClaimTypeProcedureName, serviceData.ProcedureName),
                new Claim(AppClaims.AppClaimTypeProcedureParameters, parameters),
                new Claim(AppClaims.AppClaimTypeIsProcedureReturnArray, serviceData.IsReturnArray.ToString()),
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CertificateAuthentication.Type);

            httpContext.User.AddIdentity(identity);

            await next(httpContext);
        }
    }
}
