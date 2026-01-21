namespace MonProjects.Infrastructure.Middlewares
{
    using Constants;
    using Exceptions.Contracts;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment env;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env
            )
        {
            this.next = next;
            this.env = env;

        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //await this.CreateErrorLogAsync(context, exception);

            if (this.env.IsDevelopment())
            {
                bool isCustomExInterfaceExist = exception
                    .GetType()
                    .GetInterfaces()
                    .Any(i => i.Name == nameof(ICustomExceptionCreator));

                if (isCustomExInterfaceExist)
                {
                    ICustomExceptionCreator customException = exception as ICustomExceptionCreator;

                    await customException.CreateErrorMessage(exception.Message, context.Response);
                }
                else
                {
                    await context.Response.WriteToJsonAsync(StatusCodes.Status400BadRequest, exception.Message);
                }
            }
            else
            {
                await context.Response.WriteToJsonAsync(StatusCodes.Status400BadRequest, AppExceptions.GlobalErrorExceptionMessage);
            }
        }

        //private async Task CreateErrorLogAsync(HttpContext context, Exception exception)
        //{
        //    int schoolId = context.User.GetSchoolId();
        //    int userId = context.User.GetUserId();

        //    if ((schoolId > 0 && userId > 0)
        //        || exception.GetType() == typeof(JwtAuthorizeException))
        //    {
        //        await this.errorLogService.SaveErrorDataAsync(schoolId.ToString(), userId.ToString(), context, exception);
        //    }
        //}
    }
}
