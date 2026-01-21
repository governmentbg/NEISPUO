using Kontrax.Regix.Core;
using Kontrax.Regix.Core.RegixModels.GRAO;
using Kontrax.RegiX.Core.TestStandard;
using Kontrax.RegiX.Standard.Client;
using RegiXServiceReference;
using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kontrax.RegiX.Core.Services
{
    public class GRAOService : BaseService
    {
        public GRAOService(RegixConfig config) : base(config) { }

        public async Task<ValidPersonResponse> GetValidPersonAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetValidPersonSearchRequest(egn);
            RegiXResponse response = await CallAsync(context, request);

            if (response.Errors.Count > 0)
            {
                throw new InvalidOperationException(string.Join(" | ", response.Errors));
            }

            if (string.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<ValidPersonResponse>(null);
            }

            XElement node = GetXMLNode<ValidPersonResponse>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<ValidPersonResponse>(node));
        }


        /// <summary>
        /// Справка за валидност на физическо лице (НБД „Население“/МРРБ)
        /// </summary>
        /// <returns></returns>
        private ServiceRequestData GetValidPersonSearchRequest(string egn)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<ValidPersonRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/GRAO/NBD/ValidPersonRequest"">
                        <EGN>{egn}</EGN>
                        </ValidPersonRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.ValidPersonSearch";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }

        public async Task<RelationsResponse> GetRelationsAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetRelationsRequest(egn);
            RegiXResponse response = await CallAsync(context, request);

            if (response.Errors.Count > 0)
            {
                throw new InvalidOperationException(string.Join(" | ", response.Errors));
            }

            if (string.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<RelationsResponse>(null);
            }

            XElement node = GetXMLNode<RelationsResponse>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<RelationsResponse>(node));
        }

        /// <summary>
        /// Справка за роднински връзки на физическо лице (НБД „Население“/МРРБ)
        /// </summary>
        /// <returns></returns>
        private ServiceRequestData GetRelationsRequest(string egn)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<RelationsRequestType xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/GRAO/NBD/RelationsRequest"">
                        <EGN>{egn}</EGN>
                        </RelationsRequestType>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.RelationsSearch";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }

        /// <summary>
        /// Справка за временен адрес на физическо лице
        /// </summary>
        /// <param name="egn">ЕГН</param>
        /// <param name="searchDate">Дата към която да се направи справката</param>
        /// <returns></returns>
        public async Task<TemporaryAddressResponse> GetTemporaryAddressAsync(string egn, DateTime searchDate)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetTemporaryAddressRequest(egn, searchDate);
            RegiXResponse response = await CallAsync(context, request);

            if (response.Errors.Count > 0)
            {
                throw new InvalidOperationException(string.Join(" | ", response.Errors));
            }

            if (string.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<TemporaryAddressResponse>(null);
            }

            XElement node = GetXMLNode<TemporaryAddressResponse>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<TemporaryAddressResponse>(node));
        }

        /// <summary>
        /// Справка за временен адрес на физическо лице (НБД „Население“/МРРБ)
        /// </summary>
        /// <returns></returns>
        private ServiceRequestData GetTemporaryAddressRequest(string egn, DateTime searchDate)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<TemporaryAddressRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/GRAO/PNA/TemporaryAddressRequest"">
                        <EGN>{egn}</EGN>
                        <SearchDate>{searchDate.ToString("yyyy-MM-dd")}</SearchDate>
                        </TemporaryAddressRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.GraoPNAAdapter.APIService.IPNAAPI.TemporaryAddressSearch";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }


        /// <summary>
        /// Справка за постоянен адрес на физическо лице
        /// </summary>
        /// <param name="egn">ЕГН</param>
        /// <param name="searchDate">Дата към която да се направи справката</param>
        /// <returns></returns>
        public async Task<PermanentAddressResponse> GetPermanentAddressAsync(string egn, DateTime searchDate)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetPermanentAddressRequest(egn, searchDate);
            RegiXResponse response = await CallAsync(context, request);

            if (response.Errors.Count > 0)
            {
                throw new InvalidOperationException(string.Join(" | ", response.Errors));
            }

            if (string.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<PermanentAddressResponse>(null);
            }

            XElement node = GetXMLNode<PermanentAddressResponse>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<PermanentAddressResponse>(node));
        }

        /// <summary>
        /// Справка за постоянен адрес на физическо лице (НБД „Население“/МРРБ)
        /// </summary>
        /// <returns></returns>
        private ServiceRequestData GetPermanentAddressRequest(string egn, DateTime searchDate)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<PermanentAddressRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/GRAO/PNA/PermanentAddressRequest"">
                        <EGN>{egn}</EGN>
                        <SearchDate>{searchDate.ToString("yyyy-MM-dd")}</SearchDate>
                        </PermanentAddressRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.GraoPNAAdapter.APIService.IPNAAPI.PermanentAddressSearch";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }


        public async Task<PersonDataResponse> GetPersonSearchAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = GetPersonSearchRequest(egn);
            RegiXResponse response = await CallAsync(context, request);

            if (response.Errors.Count > 0)
            {
                throw new InvalidOperationException(string.Join(" | ", response.Errors));
            }

            if (string.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                return await Task.FromResult<PersonDataResponse>(null);
            }

            XElement node = GetXMLNode<PersonDataResponse>(response.RawResponse.MessageContent);
            return await Task.FromResult(ParseResponseToObject<PersonDataResponse>(node));
        }


        /// <summary>
        /// Справка за физическо лице (НБД „Население“/МРРБ)
        /// </summary>
        /// <returns></returns>
        private ServiceRequestData GetPersonSearchRequest(string egn)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<PersonDataRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/GRAO/NBD/PersonDataRequest"">
                        <EGN>{egn}</EGN>
                        </PersonDataRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.PersonDataSearch";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }
    }
}
