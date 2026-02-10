using Kontrax.Regix.Core;
using Kontrax.Regix.Core.RegixModels.MoI;
using Kontrax.RegiX.Core.TestStandard;
using Kontrax.RegiX.Standard.Client;
using RegiXServiceReference;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kontrax.RegiX.Core.Services
{
    public class MoIService : BaseService
    {
        public MoIService(RegixConfig config) : base(config) { }
        public async Task<ForeignIdentityInfoResponse> GetForeignIdentityInfoAsync(string lnch)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetForeignIdentityInfoRequest(lnch);
            RegiXResponse response = await CallAsync(context, request);

            if (string.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<ForeignIdentityInfoResponse>(null);
            }

            XElement node = GetXMLNode<ForeignIdentityInfoResponse>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<ForeignIdentityInfoResponse>(node));
        }


        /// <summary>
        /// Справка за физическо лице с ЛНЧ(МВР)
        /// </summary>
        /// <returns></returns>
        private ServiceRequestData GetForeignIdentityInfoRequest(string lnch)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<ForeignIdentityInfoRequest xsi:schemaLocation=""http://egov.bg/RegiX/MVR/RCH/ForeignIdentityInfoRequest ForeignIdentityInfoRequestV2.xsd"" 
                        xmlns=""http://egov.bg/RegiX/MVR/RCH/ForeignIdentityInfoRequest"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                        <IdentifierType>LNCh</IdentifierType>
                        <Identifier>{lnch}</Identifier>
                </ForeignIdentityInfoRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.MVRERChAdapter.APIService.IMVRERChAPI.GetForeignIdentityV2";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }
    }
}
