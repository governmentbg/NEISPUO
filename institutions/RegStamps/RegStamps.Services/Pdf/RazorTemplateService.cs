namespace RegStamps.Services.Pdf
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;

    public class RazorTemplateService : IRazorTemplateService
    {
        private readonly IRazorViewEngine razorViewEngine;
        private readonly IServiceProvider serviceProvider;
        private readonly ITempDataProvider tempDataProvider;

        public RazorTemplateService(
            IRazorViewEngine engine,
            IServiceProvider serviceProvider,
            ITempDataProvider tempDataProvider)
        {
            this.razorViewEngine = engine;
            this.serviceProvider = serviceProvider;
            this.tempDataProvider = tempDataProvider;
        }

        public async Task<string> GetTemplateHtmlAsStringAsync<T>(string viewName, T model) where T : class
        {
            DefaultHttpContext httpContext = new DefaultHttpContext()
            {
                RequestServices = serviceProvider
            };

            ActionContext actionContext = new ActionContext(
                    httpContext, new RouteData(), new ActionDescriptor());

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = razorViewEngine.FindView(
                        actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    return string.Empty;
                }

                ViewDataDictionary viewDataDictionary = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary()
                )
                {
                    Model = model
                };

                ViewContext viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDataDictionary,
                    new TempDataDictionary(
                            actionContext.HttpContext, tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return sw.ToString();
            }
        }
    }
}
