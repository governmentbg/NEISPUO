namespace SB.Domain;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
public class ViewRender : IViewRender
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;
    public ViewRender(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        this._viewEngine = viewEngine;
        this._tempDataProvider = tempDataProvider;
        this._serviceProvider = serviceProvider;
    }
    public async Task<string> RenderAsync<TModel>(string name, TModel model)
    {
        var httpContext = new DefaultHttpContext { RequestServices = this._serviceProvider };
        return await this.RenderAsync(httpContext, name, model);
    }
    public async Task<string> RenderAsync<TModel>(HttpContext httpContext, string name, TModel model)
    {
        var lastSlashIndex = name.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
        string dir = name.Substring(0, lastSlashIndex + 1);
        string file = name.Substring(lastSlashIndex + 1);
        var viewEngineResult = this._viewEngine.GetView(dir, file, true);
        if (!viewEngineResult.Success)
        {
            throw new InvalidOperationException($"Couldn't find view '{name}'");
        }
        var view = viewEngineResult.View;
        using var output = new StringWriter();
        var viewContext = new ViewContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            view,
            new ViewDataDictionary<TModel>(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new ModelStateDictionary())
            {
                Model = model
            },
            new TempDataDictionary(
                httpContext,
                this._tempDataProvider),
            output,
            new HtmlHelperOptions());
        await view.RenderAsync(viewContext);
        return output.ToString();
    }
}
