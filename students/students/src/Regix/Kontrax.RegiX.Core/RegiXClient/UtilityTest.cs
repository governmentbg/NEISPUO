using Kontrax.RegiX.Standard.Client;
using RegiXServiceReference;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace Kontrax.RegiX.Core.TestStandard
{
    public static class UtilityTest
    {
        #region Test Calls

        public static async Task<RegiXResponse> TestDummyAsync()
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetDummyServiceRequest();
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestPersonDataSearchAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetPersonDataSearchRequest(egn);
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestValidPersonSearchAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetValidPersonSearchRequest(egn);
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestRelationsRequestAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetRelationsRequest(egn);
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestEmploymentContractsAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetEmploymentContractsRequest(egn);
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestJobSeekerStatusAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetJobSeekerStatusRequest(egn);
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestPensionRightInfoReportAsync(string egn)
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetPensionRightInfoReport(egn);
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestValidUICInfoAsync()
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetValidUICInfoRequest();
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestActualStateAsync()
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetActualStateRequest();
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestStateOfPlayAsync()
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetStateOfPlayRequest();
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestMotorVehicleRegistrationInfoV3Async()
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetMotorVehicleRegistrationInfoV3Request();
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestAircraftsByOwnerAsync()
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetAircraftsByOwnerRequest();
            return await TestAsync(context, request);
        }

        public static async Task<RegiXResponse> TestAircraftsByMSNAsync()
        {
            CustomContext context = new CustomContext();
            ServiceRequestData request = ServiceRequests.GetAircraftsByMSNRequest();
            return await TestAsync(context, request);
        }
        #endregion


        private static async Task<RegiXResponse> TestAsync(CustomContext context, ServiceRequestData request)
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



        private static RegiXEntryPointClient CreateRegiXEntryPointClient()
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.Transport)
            {
                Name = "WSHttpBinding_IRegiXEntryPoint",
                MaxReceivedMessageSize = 2097152 // 2 MB
            };
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            EndpointAddress address = new EndpointAddress("https://regix-service-test.egov.bg/RegiX/RegiXEntryPoint.svc");

            //ContractDescription desc = new ContractDescription("RegiXServiceReference.IRegiXEntryPoint");
            //ServiceEndpoint endpoint = new ServiceEndpoint(desc, binding, address);
            //endpoint.Name = "WSHttpBinding_IRegiXEntryPoint";


            RegiXEntryPointClient client = new RegiXEntryPointClient(binding, address);
            client.ClientCredentials.ClientCertificate.SetCertificate(
                StoreLocation.LocalMachine,
                StoreName.My,
                X509FindType.FindByThumbprint,
                "62ae64af7a550d426d0ba03ac9893717211d4266");

            RegiXEndpointBehavior behavior = new RegiXEndpointBehavior();
            client.Endpoint.EndpointBehaviors.Add(behavior);

            return client;

        }



    }
}
