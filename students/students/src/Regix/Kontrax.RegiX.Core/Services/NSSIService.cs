using Kontrax.Regix.Core;
using Kontrax.Regix.Core.RegixModels.GRAO;
using Kontrax.Regix.Core.RegixModels.NSSI;
using Kontrax.RegiX.Core.TestStandard;
using Kontrax.RegiX.Standard.Client;
using RegiXServiceReference;
using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kontrax.RegiX.Core.Services
{
    public class NSSIService : BaseService
    {
        public NSSIService(RegixConfig config) : base(config) { }
        public async Task<PensionRightResponse> GetPensionRightAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetPensionRightRequest(egn);
            RegiXResponse response = await CallAsync(context, request);

            if (String.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<PensionRightResponse>(null);
            }

            XElement node = GetXMLNode<ValidPersonResponse>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<PensionRightResponse>(node));
        }


        /// <summary>
        /// Справка за пенсионни права
        /// </summary>
        /// <returns></returns>
        private ServiceRequestData GetPensionRightRequest(string egn)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<PensionRightRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/NOI/RP/PensionRightRequest"">
                            <Identifier>{egn}</Identifier>
                            <IdentifierType>ЕГН</IdentifierType>
                            <Month>
                             <Month xmlns=""http://egov.bg/RegiX/NOI/RP"">--{DateTime.Now.Month.ToString().PadLeft(2, '0')}</Month>
                             <Year xmlns=""http://egov.bg/RegiX/NOI/RP"">{DateTime.Now.Year}</Year>
                           </Month>
                         </PensionRightRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.NoiRPAdapter.APIService.IRPAPI.GetPensionRightInfoReport";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }
    }
}
