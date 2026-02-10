using Kontrax.Regix.Core;
using Kontrax.Regix.Core.RegixModels.NRA;
using Kontrax.RegiX.Core.TestStandard;
using Kontrax.RegiX.Standard.Client;
using RegiXServiceReference;
using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kontrax.RegiX.Core.Services
{
    public class AVService : BaseService
    {
        public AVService(RegixConfig config) : base(config) { }

        public async Task<Regix.Core.RegixModels.AV.StateOfPlay.StateOfPlay> GetStateOfPlayAsync(string eik)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetStateOfPlayRequest(eik);
            RegiXResponse response = await CallAsync(context, request);

            if (String.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<Regix.Core.RegixModels.AV.StateOfPlay.StateOfPlay>(null);
            }

            XElement node = GetXMLNode<Regix.Core.RegixModels.AV.StateOfPlay.StateOfPlay>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<Regix.Core.RegixModels.AV.StateOfPlay.StateOfPlay>(node));
        }

        private ServiceRequestData GetStateOfPlayRequest(string eik)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = $@"<GetStateOfPlayRequest xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://www.bulstat.bg/GetStateOfPlayRequest"">
                          <UIC>{eik}</UIC>
                        </GetStateOfPlayRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API.GetStateOfPlay";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }

        public async Task<Regix.Core.RegixModels.AV.ActualResponseType.ActualStateResponse> GetActualStateAsync(string eik)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetActualStateRequest(eik);
            RegiXResponse response = await CallAsync(context, request);

            if (String.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<Regix.Core.RegixModels.AV.ActualResponseType.ActualStateResponse>(null);
            }

            XElement node = GetXMLNode<Regix.Core.RegixModels.AV.ActualResponseType.ActualStateResponse>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<Regix.Core.RegixModels.AV.ActualResponseType.ActualStateResponse>(node));
        }


        private ServiceRequestData GetActualStateRequest(string eik)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = $@"<ActualStateRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://egov.bg/RegiX/AV/TR/ActualStateRequest""><UIC>{eik}</UIC></ActualStateRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetActualState";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }
    }
}
