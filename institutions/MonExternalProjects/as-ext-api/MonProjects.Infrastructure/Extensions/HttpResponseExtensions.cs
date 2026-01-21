namespace MonProjects.Infrastructure.Extensions
{
    using Exceptions.Models;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public static class HttpResponseExtensions
    {
        public static async Task WriteToJsonAsync(this HttpResponse response, int statusCode, string message)
        {
            response.StatusCode = statusCode;
            await response.WriteAsync(new ErrorMessageDetailsModel
            {
                StatusCode = statusCode,
                Message = message
            }.ToString());
        }

    }
}
