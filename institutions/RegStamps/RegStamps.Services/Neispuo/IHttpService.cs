namespace RegStamps.Services.Neispuo
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> GetAsync(string path);
    }
}
