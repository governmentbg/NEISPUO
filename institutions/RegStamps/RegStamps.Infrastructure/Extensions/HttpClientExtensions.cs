namespace RegStamps.Infrastructure.Extensions
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    public static class HttpClientExtensions
    {
        public const string RequestFailed = "Resources at: [{0}] {1} are not available. HttpStatus: {2}. Message: {3}.";

        public static async Task<TModel> DeserializeToObjectAsync<TModel>(this HttpResponseMessage httpResponse)
        {
            return JsonConvert.DeserializeObject<TModel>(await httpResponse.Content.ReadAsStringAsync());
        }

        public static async Task<IEnumerable<TModel>> DeserializeToCollectionAsync<TModel>(this HttpResponseMessage httpResponse)
        {
            return JsonConvert.DeserializeObject<IEnumerable<TModel>>(await httpResponse.Content.ReadAsStringAsync());
        }

        public static async Task<string> GetErrorMessageAsync(this HttpResponseMessage httpResponse)
        {
            HttpMethod httpMethod = httpResponse.RequestMessage.Method;
            string absoluteUri = httpResponse.RequestMessage.RequestUri.AbsoluteUri;
            HttpStatusCode statusCode = httpResponse.StatusCode;
            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            return String.Format(RequestFailed, httpMethod, absoluteUri, statusCode, responseContent);
        }
    }
}
