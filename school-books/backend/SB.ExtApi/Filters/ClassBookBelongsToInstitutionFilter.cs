namespace SB.ExtApi;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SB.ApiAbstractions;
using SB.Domain;

public class ClassBookBelongsToInstitutionFilter : IAsyncResourceFilter
{
    private IApiAuthCachedQueryStore apiAuthCachedQueryStore;

    public ClassBookBelongsToInstitutionFilter(IApiAuthCachedQueryStore apiAuthCachedQueryStore)
    {
        this.apiAuthCachedQueryStore = apiAuthCachedQueryStore;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        int? schoolYear = context.HttpContext.GetIntFromRoute("schoolYear");
        int? instId = context.HttpContext.GetIntFromRoute("institutionId");
        int? classBookId = context.HttpContext.GetIntFromRoute("classBookId");

        var classBookBelongsToInstitution =
            schoolYear.HasValue &&
            instId.HasValue &&
            classBookId.HasValue &&
            (await this.apiAuthCachedQueryStore.GetClassBookBelongsToInstitutionAsync(
                schoolYear.Value,
                instId.Value,
                classBookId.Value,
                context.HttpContext.RequestAborted));

        if (!classBookBelongsToInstitution)
        {
            context.Result = new ContentResult()
            {
                Content = "ClassBookId does not belong to institution",
                ContentType = "text/plain",
                StatusCode = StatusCodes.Status404NotFound,
            };
        }
        else
        {
            await next();
        }
    }
}
