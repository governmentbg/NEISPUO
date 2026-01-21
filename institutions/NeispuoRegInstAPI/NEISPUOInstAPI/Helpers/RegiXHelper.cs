using RegiXService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;

namespace NEISPUORegInstAPI.Helpers
{
    public class RegiXHelper
    {
        public class RegixBasicRequest
        {
            public string Operation { get; set; }
            public string Argument { get; set; }
        }
        public class RegixResponse
        {
            public int Status { get; set; }
            public string Message { get; set; }
            public XmlDocument Data { get; set; }
        }
        public RegixResponse AskRegix(RegixBasicRequest req, string certPath, string certPass)
        {
            RegixResponse resp = new RegixResponse();
            try
            {
                //RegiXEntryPointClient client = new RegiXEntryPointClient("WSHttpBinding_IRegiXEntryPoint");
                //RegiXServiceReference.ServiceRequestData request = new RegiXServiceReference.ServiceRequestData();
                RegiXEntryPointV2Client.EndpointConfiguration endpointConfiguration = new RegiXEntryPointV2Client.EndpointConfiguration();
                System.ServiceModel.BasicHttpsBinding binding = new System.ServiceModel.BasicHttpsBinding(BasicHttpsSecurityMode.Transport)
                {
                    Name = "WSHttpBinding_IRegiXEntryPointV2",
                    MaxReceivedMessageSize = 2097152 // 2 MB,
                    
                };
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

                EndpointAddress address = new EndpointAddress("https://service-regix.egov.bg/RegiXEntryPointV2.svc");
                RegiXEntryPointV2Client client = new RegiXEntryPointV2Client(binding,address);
               
                ServiceRequestData request = new ServiceRequestData();
                // Име на операцията, която искаме да изпълним
                request.Operation = req.Operation;
                // Контекст (описание), в който изпълняваме заявката
                request.CallContext =
                    new CallContext()
                    {
                        // Име на администрацията създала заявката
                        AdministrationName = "МОН",
                        // OID на администрацията създала заявката
                        AdministrationOId = "2.16.100.1.1.28",
                        // Идентификатор на служителя създал заявката
                        EmployeeIdentifier = "ИС на МОН",
                        // Допълнителене идентификатор на служителя създал заявката
                        //EmployeeAditionalIdentifier = "<employee_additional_identifier>",
                        // Имена на служителя създал заявката
                        EmployeeNames = "ИС на МОН",
                        // Позиция на служителя създал заявката
                        //EmployeePosition = "<employee_position>",
                        // Правно основание
                        LawReason = "За съпоставка на децата/учениците между ГРАО/МВР и ИС на МОН",
                        // Пояснения
                        Remark = "Справката е генерирана автоматично от ИС на МОН",
                        // Идентификатор на отговорният служител
                        // ResponsiblePersonIdentifier = "<responsible_person_identifier>",
                        // Тип на услугата
                        ServiceType = "Административна услуга",
                        // URI на услугата
                        ServiceURI = "neispuo.mon.bg"
                    };
                // Дали резултата да бъде подписван
                request.SignResult = false;
                // Дали да бъде връщана матрицата за достъп
                request.ReturnAccessMatrix = true; //| false;
                                                   // EID Token в случай, че се използа такъв
                                                   //request.EIDToken = "<eid_token>";
                                                   // XmlElement съдържащ аргумента на заявката
                XmlDocument doc = new XmlDocument();

                doc.LoadXml(req.Argument);

                request.Argument = doc.DocumentElement;

                //client.ClientCredentials.ClientCertificate.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(certPath, certPass);
                X509Store store = new X509Store("My", StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
                foreach (X509Certificate2 x509 in collection)
                {
                    if (x509.Thumbprint.ToLower() == "0c555f4d3ff7a3300778335a62b8254d756a8721") // Thumbprint на серртификата от Windows KeyStore-a
                    {
                        client.ClientCredentials.ClientCertificate.SetCertificate(
                         x509.SubjectName.Name, store.Location, StoreName.My);
                    }
                }

                RequestWrapper requestWrapper = new RequestWrapper();
                requestWrapper.ServiceRequestData = request;
                ResultWrapper result = client.Execute(requestWrapper);
                // Изпълнение на услугата. Резултатът се съдържа в променливата result

                // Отогворът съдържа следните property-та:
                // result.Data;  - Данните на резултата
                // result.HasError; - Дали е възникнала грешка
                // result.Error; - Съобщението на грешката
                // result.Signature; - Подпис на резултата

                if (result.ServiceResultData.Error != "")
                {
                    resp.Status = 0;
                    resp.Message = result.ServiceResultData.Error;
                }
                else
                {
                    resp.Status = 1;
                    XmlDocument docres = new XmlDocument();
                    docres.LoadXml(result.ServiceResultData.Data.Response.Any.OuterXml.ToString().Replace("#", String.Empty));
                    resp.Message = "OK";
                    resp.Data = docres;
                }

                return resp;
            }
            catch (Exception ex)
            {
                resp.Status = -1;
                resp.Message = ex.ToString();
                return resp;

            }
        }
    }
}
