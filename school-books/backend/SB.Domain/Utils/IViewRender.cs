namespace SB.Domain;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
public interface IViewRender
{
    Task<string> RenderAsync<TModel>(string name, TModel model);
    Task<string> RenderAsync<TModel>(HttpContext httpContext, string name, TModel model);
}
