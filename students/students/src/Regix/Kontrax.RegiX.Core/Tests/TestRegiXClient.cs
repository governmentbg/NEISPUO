using Kontrax.RegiX.Core.TestStandard.Shared;
using Kontrax.RegiX.Standard.Client;
using System;
using System.Threading.Tasks;

namespace Kontrax.RegiX.Core.TestStandard.Tests
{
    public static class TestRegiXClient
    {
        public static async Task TestReports()
        {
            //await TestRegiX(ReportType.Dummy);
            await TestRegiX(ReportType.PersonDataSearch);
            await TestRegiX(ReportType.EmploymentContracts);
            await TestRegiX(ReportType.PensionRightInfoReport);
            await TestRegiX(ReportType.Relations);
            await TestRegiX(ReportType.ValidPersonSearch);
            await TestRegiX(ReportType.ValidUICInfo);
            await TestRegiX(ReportType.ActualState);
            await TestRegiX(ReportType.StateOfPlay);
            await TestRegiX(ReportType.MotorVehicleRegistrationInfoV3);
            await TestRegiX(ReportType.AircraftsByOwner);
            await TestRegiX(ReportType.AircraftsByMSN);
        }

        private static async Task TestRegiX(ReportType report)
        {
            try
            {
                Console.WriteLine("-- Started testing " + report.ToString() + " --");
                Console.WriteLine();

                RegiXResponse result = null;
                switch (report)
                {
                    case ReportType.Dummy:
                        result = await UtilityTest.TestDummyAsync();
                        break;
                    case ReportType.PersonDataSearch:
                        result = await UtilityTest.TestPersonDataSearchAsync("8506258485");
                        break;
                    case ReportType.ValidPersonSearch:
                        result = await UtilityTest.TestValidPersonSearchAsync("8506258485");
                        break;
                    case ReportType.ValidUICInfo:
                        result = await UtilityTest.TestValidUICInfoAsync();
                        break;
                    case ReportType.ActualState:
                        result = await UtilityTest.TestActualStateAsync();
                        break;
                    case ReportType.StateOfPlay:
                        result = await UtilityTest.TestStateOfPlayAsync();
                        break;
                    case ReportType.MotorVehicleRegistrationInfoV3:
                        result = await UtilityTest.TestMotorVehicleRegistrationInfoV3Async();
                        break;
                    case ReportType.AircraftsByOwner:
                        result = await UtilityTest.TestAircraftsByOwnerAsync();
                        break;
                    case ReportType.AircraftsByMSN:
                        result = await UtilityTest.TestAircraftsByMSNAsync();
                        break;
                    case ReportType.Relations:
                        result = await UtilityTest.TestRelationsRequestAsync("8506258485");
                        break;
                    case ReportType.EmploymentContracts:
                        result = await UtilityTest.TestEmploymentContractsAsync("8506258485");
                        break;
                    case ReportType.PensionRightInfoReport:
                        result = await UtilityTest.TestPensionRightInfoReportAsync("8506258485");
                        break;
                    default:
                        break;
                }

                if (result != null)
                {
                    Console.WriteLine("Errors: " + String.Join("; ", result.Result.Error));
                    Console.WriteLine();
                    Console.WriteLine("Request: " + result.RawRequest.MessageId?.ToString());
                    Console.WriteLine(result.RawRequest.MessageContent);
                    Console.WriteLine();
                    Console.WriteLine("Response: " + result.RawResponse.MessageId?.ToString());
                    Console.WriteLine("Relates to: " + result.RawResponse.RelatesToMessageId?.ToString());
                    Console.WriteLine(result.RawResponse.MessageContent);
                    Console.WriteLine();
                    //Console.WriteLine("RawResponse: " + result.RawResponse);
                    //Console.WriteLine();
                    //Console.WriteLine("InnerXml: " + result.Result.Data?.Response?.Any?.InnerXml);
                    //Console.WriteLine();
                }

                Console.WriteLine("-- Ended testing " + report.ToString() + " --");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("*********************************************************************");
                Console.WriteLine();
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (e.InnerException != null)
                    Console.WriteLine(e.InnerException.Message);
            }
        }
    }
}
