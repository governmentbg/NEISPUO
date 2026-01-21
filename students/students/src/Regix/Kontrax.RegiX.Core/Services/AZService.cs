using Kontrax.Regix.Core;
using Kontrax.Regix.Core.RegixModels.AZ.JobSeekerContracts;
using Kontrax.Regix.Core.RegixModels.AZ.JobSeekerStatus;
using Kontrax.RegiX.Core.TestStandard;
using Kontrax.RegiX.Standard.Client;
using RegiXServiceReference;
using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kontrax.RegiX.Core.Services
{
    public class AZService : BaseService
    {
        public AZService(RegixConfig config) : base(config) { }

        public async Task<JobSeekerStatusResponse> GetJobSeekerStatusAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetJobSeekerStatusRequest(egn);
            RegiXResponse response = await CallAsync(context, request);

            if (string.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<JobSeekerStatusResponse>(null);
            }

            XElement node = GetXMLNode<JobSeekerStatusResponse>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<JobSeekerStatusResponse>(node));
        }


        /// <summary>
        /// Справка за статус на търсещото работа лице
        /// </summary>
        /// <returns></returns>
        private ServiceRequestData GetJobSeekerStatusRequest(string egn)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<JobSeekerStatusRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/AZ/JobSeekerStatusRequest"">
                        <PersonalID>{egn}</PersonalID>
                        </JobSeekerStatusRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.AZJobsAdapter.APIService.IAZJobsAPI.GetJobSeekerStatus";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }


        public async Task<JobSeekerContractsResponse> GetJobSeekerContractsAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetJobSeekerContractsRequest(egn);
            RegiXResponse response = await CallAsync(context, request);

            if (String.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<JobSeekerContractsResponse>(null);
            }

            XElement node = GetXMLNode<JobSeekerContractsResponse>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<JobSeekerContractsResponse>(node));
        }


        /// <summary>
        /// Справка за статус на търсещото работа лице
        /// </summary>
        /// <returns></returns>
        private ServiceRequestData GetJobSeekerContractsRequest(string egn)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<JobSeekerContractsRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/AZ/JobSeekerContractsRequest"">
                        <PersonalID>{egn}</PersonalID>
                        </JobSeekerContractsRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.AZJobsAdapter.APIService.IAZJobsAPI.GetJobSeekerContracts";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }
    }
}
