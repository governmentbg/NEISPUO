using RegiXServiceReference;
using System;
using System.Threading.Tasks;

namespace Kontrax.RegiX.Standard.Client
{
    public static class RegiXUtility
    {
        public static async Task<RegiXResponse> CallAsync(RegiXEntryPointClient client, ServiceRequestData request)
        {
            Guid callId = Guid.NewGuid();
            //string rawResponse;
            ServiceResultData result;

            RawMessage rawRequestMessage = new RawMessage();
            RawMessage rawResponseMessage = new RawMessage();

            try
            {
                callId = RegiXMessageInspector.BeforeCall();

                if (client == null)
                    client = new RegiXEntryPointClient();
                await client.OpenAsync();
                result = await client.ExecuteSynchronousAsync(request);
                await client.CloseAsync();

            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
            finally
            {
                //rawResponse = RegiXMessageInspector.AfterCall(callId);
                RegiXMessageInspector.AfterCallAll(callId, ref rawRequestMessage, ref rawResponseMessage);
            }

            return new RegiXResponse(result, rawRequestMessage, rawResponseMessage);
        }



    }
}
