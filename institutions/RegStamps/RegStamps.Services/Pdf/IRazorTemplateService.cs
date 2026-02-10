namespace RegStamps.Services.Pdf
{
    public interface IRazorTemplateService
    {
        Task<string> GetTemplateHtmlAsStringAsync<T>(string viewName, T model) where T : class;
    }
}
