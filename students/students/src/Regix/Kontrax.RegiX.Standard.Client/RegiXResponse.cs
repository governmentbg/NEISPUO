using RegiXServiceReference;
using System.Collections.Generic;

namespace Kontrax.RegiX.Standard.Client
{
    public partial class RegiXResponse
    {
        public ServiceResultData Result { get; }
        public RawMessage RawRequest { get; }
        public RawMessage RawResponse { get; }


        public RegiXResponse(ServiceResultData result, RawMessage request, RawMessage response)
        {
            Result = result;
            RawRequest = request;
            RawResponse = response;
        }

        public string InnerXml
        {
            get
            {
                if (Result == null) return null;
                return Result.Data?.Response?.Any?.InnerXml;
            }
        }

        public IReadOnlyCollection<string> Errors
        {
            get
            {
                if (Result == null) return null;
                List<string> errors = new List<string>();
                if (!string.IsNullOrEmpty(Result.Error) || Result.HasError || !Result.IsReady)
                {
                    string error = !string.IsNullOrEmpty(Result.Error) ? Result.Error
                        : "Неуточнена грешка:" + (Result.HasError ? " HasError" : null) + (!Result.IsReady ? " !IsReady" : null);
                    errors.Add("Отговор от RegiX: " + error);
                }
                return errors;
            }
        }

    }
}
