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
    public class NRAService : BaseService
    {
        public NRAService(RegixConfig config) : base(config) { }

        public async Task<EmploymentContractsResponse> GetEmploymentContractsAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetEmploymentContractsRequest(egn);
            RegiXResponse response = await CallAsync(context, request);

            if (response.Errors.Count > 0)
            {
                throw new InvalidOperationException(string.Join(" | ", response.Errors));
            }

            if (String.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<EmploymentContractsResponse>(null);
            }

            XElement node = GetXMLNode<EmploymentContractsResponse>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<EmploymentContractsResponse>(node));
        }


        private ServiceRequestData GetEmploymentContractsRequest(string egn)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<EmploymentContractsRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/NRA/EmploymentContracts/Request"">
                        <Identity>
                            <ID>{egn}</ID>
                            <TYPE>EGN</TYPE>
                        </Identity>
                        </EmploymentContractsRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.NRAEmploymentContractsAdapter.APIService.INRAEmploymentContractsAPI.GetEmploymentContracts";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }
    }
}
