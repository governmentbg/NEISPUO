using RegiXServiceReference;
using System.Xml;

namespace Kontrax.RegiX.Core.TestStandard
{
    public static class ServiceRequests
    {
        private static ServiceRequestData GetServiceRequest(string operation, XmlElement argument, CallContext callContext)
        {
            ServiceRequestData request = new ServiceRequestData
            {
                Operation = operation,
                Argument = argument,
                CallContext = callContext,
                CitizenEGN = null,
                EmployeeEGN = null,
                ReturnAccessMatrix = false,
                SignResult = true,
                CallbackURL = null,
                EIDToken = null
            };

            return request;
        }

        public static ServiceRequestData GetDummyServiceRequest()
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @"<ValidPersonRequest
                            xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                            xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                           xmlns=""http://egov.bg/RegiX/GRAO/NBD/ValidPersonRequest"">
                            <EGN>8506258485</EGN>
                          </ValidPersonRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.ValidPersonSearch";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }



        /// <summary>
        /// Справка за физическо лице (НБД „Население“/МРРБ)
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetPersonDataSearchRequest(string egn)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<PersonDataRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                    xmlns=""http://egov.bg/RegiX/GRAO/NBD/PersonDataRequest"">
                    <EGN>{egn}</EGN>
                    </PersonDataRequest> ";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.PersonDataSearch";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }

        /// <summary>
        /// Справка за валидност на физическо лице (НБД „Население“/МРРБ)
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetValidPersonSearchRequest(string egn)
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

        /// <summary>
        /// Справка за роднински връзки на физическо лице (НБД „Население“/МРРБ)
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetRelationsRequest(string egn)
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
        /// Справка за трудови договори на физическо лице (НБД „Население“/МРРБ)
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetEmploymentContractsRequest(string egn)
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

        /// <summary>
        /// Справка за статус на търсещото работа лице
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetJobSeekerStatusRequest(string egn)
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

        /// <summary>
        /// Справка за упражнено право за пенсиониране
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetPensionRightInfoReport(string egn)
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @$"<PensionRightRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/NOI/RP/PensionRightRequest"">
                            <Identifier>{egn}</Identifier>
                            <IdentifierType>ЕГН</IdentifierType>
                            <Month>
                             <Month xmlns=""http://egov.bg/RegiX/NOI/RP"">--02</Month>
                             <Year xmlns=""http://egov.bg/RegiX/NOI/RP"">2013</Year>
                           </Month>
                         </PensionRightRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.NoiRPAdapter.APIService.IRPAPI.GetPensionRightInfoReport";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }



        /// <summary>
        /// Справка за валидност на ЕИК номер (Търговски регистър/АВ)
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetValidUICInfoRequest()
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @"<ValidUICRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                            xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                            xmlns=""http://egov.bg/RegiX/AV/TR/ValidUICRequest"">
                            <UIC>201593301</UIC>
                            </ValidUICRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetValidUICInfo";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }

        /// <summary>
        /// Справка за актуално състояние (Търговски регистър/АВ)
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetActualStateRequest()
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @"<ActualStateRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/AV/TR/ActualStateRequest"">
                        <UIC>201593301</UIC>
                        </ActualStateRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetActualState";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }

        /// <summary>
        /// Справка по код на БУЛСТАТ или по фирмено дело за актуално състояние на субект на БУЛСТАТ (Регистър БУЛСТАТ/АВ)
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetStateOfPlayRequest()
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @"<GetStateOfPlayRequest xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                            xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                            xmlns=""http://www.bulstat.bg/GetStateOfPlayRequest"">
                            <UIC>1212120908</UIC>
                            <Case>
                              <Court />
                            </Case>
                          </GetStateOfPlayRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API.GetStateOfPlay";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }


        /// <summary>
        /// Разширена справка за МПС по регистрационен номер - V3 (Регистър на моторните превозни средства/МВР)
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetMotorVehicleRegistrationInfoV3Request()
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @"<GetMotorVehicleRegistrationInfoV3Request 
                        xsi:schemaLocation=""http://egov.bg/RegiX/MVR/MPS/GetMotorVehicleRegistrationInfoV3Request GetMotorVehicleRegistrationInfoV3Request.xsd"" 
                        xmlns=""http://egov.bg/RegiX/MVR/MPS/GetMotorVehicleRegistrationInfoV3Request"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	                        <Identifier>CA1234CA</Identifier>
                        </GetMotorVehicleRegistrationInfoV3Request>";
            xml = @"<GetMotorVehicleRegistrationInfoV2Request 
                    xsi:schemaLocation=""http://egov.bg/RegiX/MVR/MPS/GetMotorVehicleRegistrationInfoV2Request GetMotorVehicleRegistrationInfoV2Request.xsd"" 
                    xmlns=""http://egov.bg/RegiX/MVR/MPS/GetMotorVehicleRegistrationInfoV2Request"" 
                    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	                    <Identifier>CA0000CA</Identifier>
                    </GetMotorVehicleRegistrationInfoV2Request>";
            xml = @"<MotorVehicleRegistrationRequest 
                    xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                    xmlns=""http://egov.bg/RegiX/MVR/MPS/MotorVehicleRegistrationRequest"">
                    <Identifier>А7801ХО</Identifier>
                  </MotorVehicleRegistrationRequest>
";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.MVRMPSAdapter.APIService.IMVRMPSAPI.GetMotorVehicleRegistrationInfoV3";
            operation = "TechnoLogica.RegiX.MVRMPSAdapter.APIService.IMVRMPSAPI.GetMotorVehicleRegistrationInfoV2";
            operation = "TechnoLogica.RegiX.MVRMPSAdapter.APIService.IMVRMPSAPI.GetMotorVehicleRegistrationInfo";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }

        /// <summary>
        /// Справка по ЕИК/БУЛСТАТ/ЕГН/ЛНЧ за вписани на името на лицето въздухоплавателни средства (Регистър на гражданските въздухоплавателни средства на Република България/ГВА)
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetAircraftsByOwnerRequest()
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @"<AircraftsByOwnerRequest 
                        xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/GVA/AircraftsByOwnerRequest"">
                        <OwnerID>1234567890</OwnerID>
                        <DateFrom>2000-01-01</DateFrom>
                        <DateTo>2016-01-01</DateTo>
                      </AircraftsByOwnerRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.GvaAircraftsAdapter.APIService.IGvaAircraftsAPI.GetAircraftsByOwner";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }

        /// <summary>
        /// Справка по сериен номер на въздухоплавателно средство за вписани в регистъра обстоятелства (Регистър на гражданските въздухоплавателни средства на Република България/ГВА)
        /// </summary>
        /// <returns></returns>
        public static ServiceRequestData GetAircraftsByMSNRequest()
        {
            CallContext callContext = CustomCallContext.GetCallContext();

            XmlDocument doc = new XmlDocument();
            string xml = @"<AircraftsByMSNRequest 
                        xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                        xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                        xmlns=""http://egov.bg/RegiX/GVA/AircraftsByMSNRequest"">
                        <MSN>123</MSN>
                      </AircraftsByMSNRequest>";
            doc.LoadXml(xml);
            XmlElement argument = doc.DocumentElement;

            string operation = "TechnoLogica.RegiX.GvaAircraftsAdapter.APIService.IGvaAircraftsAPI.GetAircraftsByMSN";
            ServiceRequestData request = GetServiceRequest(operation, argument, callContext);

            return request;
        }


    }
}
