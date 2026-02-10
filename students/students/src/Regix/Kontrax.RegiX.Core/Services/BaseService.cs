using Kontrax.Regix.Core;
using Kontrax.RegiX.Core.TestStandard;
using Kontrax.RegiX.Standard.Client;
using RegiXServiceReference;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Kontrax.RegiX.Core.Services
{
    public class BaseService : IDisposable
    {
        private bool _disposed = false;
        private readonly RegixConfig _config;

        public BaseService(RegixConfig config)
        {
            _config = config;
        }

        protected ServiceRequestData GetServiceRequest(string operation, XmlElement argument, CallContext callContext)
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

        protected T ParseResponseToObject<T>(XElement node) where T : class, new()
        {
            try
            {
                if (node == null)
                {
                    Console.WriteLine("Node element is null");
                    return default;
                }


                T data = new T();

                // Добавя xsd namespace
                XNamespace xsi = @"http://www.w3.org/2001/XMLSchema";
                XAttribute attribute = new XAttribute(XNamespace.Xmlns + "xsd", xsi.NamespaceName);
                node.Add(attribute);


                using StringReader reader = new StringReader(node.ToString());
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                data = (T)xmlSerializer.Deserialize(reader);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing response to object: " + ex.Message);
                return default;
            }
        }

        protected XElement GetXMLNode<ResponseType>(string rawResponse)
        {
            XElement node = GetResponseNodeFromRawResponse<ResponseType>(rawResponse);
            return node;
        }

        private XElement GetResponseNodeFromRawResponse<T>(string rawResponse)
        {
            if (rawResponse != null)
            {
                XDocument soap = XDocument.Parse(rawResponse);  // Текстът не трябва да започва с BOM.
                XmlTypeAttribute xmlAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(
                                   typeof(T),
                                   typeof(XmlTypeAttribute)
                                 );
                XNamespace ns = xmlAttribute.Namespace;
                string typeName = typeof(T).Name;
                return soap.Descendants(ns + typeName).FirstOrDefault();
            }
            return null;
        }

        protected async Task<RegiXResponse> CallAsync(CustomContext context, ServiceRequestData request)
        {
            try
            {
                RegiXEntryPointClient client = CreateRegiXEntryPointClient();

                // Ако не е подаден сертификат, се използва този от конфигурационния файл.            
                X509CertificateInitiatorClientCredential clientCert = client.ClientCredentials.ClientCertificate;
                if (context.Certificate != null)
                {
                    clientCert.Certificate = context.Certificate;
                }
                else if (clientCert.Certificate == null)
                {
                    throw new Exception("Не е указан сертификат за достъп до RegiX.");
                }

                RegiXResponse response = await RegiXUtility.CallAsync(client, request);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private RegiXEntryPointClient CreateRegiXEntryPointClient()
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.Transport)
            {
                Name = "WSHttpBinding_IRegiXEntryPoint",
                MaxReceivedMessageSize = 2097152 // 2 MB
            };
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            EndpointAddress address = new EndpointAddress(_config.Url);

            //ContractDescription desc = new ContractDescription("RegiXServiceReference.IRegiXEntryPoint");
            //ServiceEndpoint endpoint = new ServiceEndpoint(desc, binding, address);
            //endpoint.Name = "WSHttpBinding_IRegiXEntryPoint";


            RegiXEntryPointClient client = new RegiXEntryPointClient(binding, address);
            switch (_config.SearchLocation)
            {
                case SearchLocationType.Computer:
                    client.ClientCredentials.ClientCertificate.SetCertificate(
                        StoreLocation.LocalMachine,
                        StoreName.My,
                        X509FindType.FindByThumbprint,
                        _config.Thumbprint
                        //"62ae64af7a550d426d0ba03ac9893717211d4266"
                        );
                    break;
                case SearchLocationType.File:
                    X509Certificate2 certificate = new X509Certificate2(_config.FileLocation, _config.Password);
                    client.ClientCredentials.ClientCertificate.Certificate = certificate;
                    break;
            }

            RegiXEndpointBehavior behavior = new RegiXEndpointBehavior();
            client.Endpoint.EndpointBehaviors.Add(behavior);

            return client;

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            _disposed = true;
        }
    }
}
