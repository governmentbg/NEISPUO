namespace SB.Api;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SB.ApiAbstractions;
using SB.Domain;
using System;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class DisallowWhenInstHasCBExtProviderAttribute : Attribute, IAsyncResourceFilter
{
    public DisallowWhenInstHasCBExtProviderAttribute()
    {
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        HttpContext httpContext = context.HttpContext;
        int? schoolYear = httpContext.GetIntFromRoute("schoolYear");
        int? instId = httpContext.GetIntFromRoute("instId");
        if (schoolYear == null || instId == null)
        {
            throw new Exception($"{nameof(schoolYear)} and {nameof(instId)} should have a value");
        }

        var commonCachedQueryStore = httpContext.RequestServices.GetRequiredService<ICommonCachedQueryStore>();
        if (httpContext.GetAccessType() == AccessType.Write &&
            await commonCachedQueryStore.GetInstHasCBExtProviderAsync(schoolYear.Value, instId.Value, httpContext.RequestAborted))
        {
            context.Result =
                new ContentResult()
                {
                    Content = "School year is managed by an external provider",
                    StatusCode = StatusCodes.Status409Conflict,
                };
            return;
        }

        await next();
    }
}
