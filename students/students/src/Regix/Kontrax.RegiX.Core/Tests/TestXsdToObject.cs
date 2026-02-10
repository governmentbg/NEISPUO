using Kontrax.Regix.Core.RegixModels.AZ.JobSeekerStatus;
using Kontrax.Regix.Core.RegixModels.GRAO;
using Kontrax.Regix.Core.RegixModels.NRA;
using Kontrax.Regix.Core.RegixModels.NSSI;
using Kontrax.RegiX.Core.TestStandard.RegiXModels;
using Kontrax.RegiX.Core.TestStandard.Shared;
using Kontrax.RegiX.Standard.Client;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Kontrax.RegiX.Core.TestStandard.Tests
{
    public static class TestXsdToObject
    {
        public static async Task TestParseXsd(ReportType reportType)
        {
            RegiXResponse response = await GetRegiXResponse(reportType);
            if (String.IsNullOrWhiteSpace(response.RawResponse.MessageContent))
            {
                Console.WriteLine("Missing RegiX raw response");
                return;
            }

            XElement node = GetXMLNode(reportType, response.RawResponse.MessageContent);

            ParseResponseToObject(node, reportType);
        }

        private static XElement GetXMLNode(ReportType reportType, string rawResponse)
        {
            XElement node = null;
            switch (reportType)
            {
                case ReportType.AircraftsByMSN:
                case ReportType.AircraftsByOwner:
                    node = GetResponseNodeFromRawResponse<AircraftsResponse>(rawResponse);
                    break;
                case ReportType.Relations:
                    node = GetResponseNodeFromRawResponse<RelationsResponse>(rawResponse);
                    break;
                case ReportType.ValidPersonSearch:
                    node = GetResponseNodeFromRawResponse<ValidPersonResponse>(rawResponse);
                    break;
                case ReportType.PensionRightInfoReport:
                    node = GetResponseNodeFromRawResponse<PensionRightResponse>(rawResponse);
                    break;
                case ReportType.EmploymentContracts:
                    node = GetResponseNodeFromRawResponse<EmploymentContractsResponse>(rawResponse);
                    break;
                case ReportType.JobSeekerStatus:
                    node = GetResponseNodeFromRawResponse<JobSeekerStatusResponse>(rawResponse);
                    break;
                default:
                    break;
            }

            return node;
        }

        private static void ParseResponseToObject(XElement node, ReportType reportType)
        {
            try
            {
                if (node == null)
                {
                    Console.WriteLine("Node element is null");
                    return;
                }

                switch (reportType)
                {
                    case ReportType.AircraftsByMSN:
                    case ReportType.AircraftsByOwner:
                        AircraftsResponse aircraft = new AircraftsResponse();
                        using (StringReader reader = new StringReader(node.ToString()))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(AircraftsResponse));
                            aircraft = (AircraftsResponse)xmlSerializer.Deserialize(reader);
                            if (aircraft != null)
                            {
                                Aircraft air = aircraft.Aircraft.FirstOrDefault();
                                if (air != null)
                                {
                                    Console.WriteLine(AircraftToString(air));
                                }
                            }
                        }
                        break;
                    case ReportType.Relations:
                        RelationsResponse relations = new RelationsResponse();
                        using (StringReader reader = new StringReader(node.ToString()))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RelationsResponse));
                            relations = (RelationsResponse)xmlSerializer.Deserialize(reader);
                            if (relations != null)
                            {
                                foreach (var personRelation in relations.PersonRelations.Where(i => i != null))
                                {
                                    Console.WriteLine(PersonRelationToString(personRelation));
                                }
                            }
                        }
                        break;
                    case ReportType.ValidPersonSearch:
                        ValidPersonResponse person = new ValidPersonResponse();
                        using (StringReader reader = new StringReader(node.ToString()))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ValidPersonResponse));
                            person = (ValidPersonResponse)xmlSerializer.Deserialize(reader);
                            if (person != null)
                            {

                                Console.WriteLine(PersonToString(person));

                            }
                        }
                        break;
                    case ReportType.PensionRightInfoReport:
                        PensionRightResponse pensionRight = new PensionRightResponse();
                        using (StringReader reader = new StringReader(node.ToString()))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PensionRightResponse));
                            pensionRight = (PensionRightResponse)xmlSerializer.Deserialize(reader);
                            if (pensionRight != null)
                            {
                                Console.WriteLine(PensionRightToString(pensionRight));
                            }
                        }
                        break;
                    case ReportType.EmploymentContracts:
                        EmploymentContractsResponse employmentContracts = new EmploymentContractsResponse();
                        using (StringReader reader = new StringReader(node.ToString()))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(EmploymentContractsResponse));
                            employmentContracts = (EmploymentContractsResponse)xmlSerializer.Deserialize(reader);
                            if (employmentContracts != null)
                            {
                                Console.WriteLine(employmentContracts.ToString());
                            }
                        }
                        break;
                    case ReportType.JobSeekerStatus:
                        var jobSeekerStatus = new JobSeekerStatusResponse();
                        using (StringReader reader = new StringReader(node.ToString()))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(JobSeekerStatusResponse));
                            jobSeekerStatus = (JobSeekerStatusResponse)xmlSerializer.Deserialize(reader);
                            if (jobSeekerStatus != null)
                            {
                                Console.WriteLine(jobSeekerStatus.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }

                Console.WriteLine("End testing parse response to object");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing response to object: " + ex.Message);
            }


        }

        private static string AircraftToString(Aircraft aircraft)
        {
            string result = String.Format(@"ModelName: {0}/{1}
MSNSerialNumber: {2}
ICAO: {3}",
                aircraft.BGModelName,
                aircraft.ENModelName,
                aircraft.MSNSerialNumber,
                aircraft.ICAO);
            return result;
        }

        private static string PersonRelationToString(PersonRelationType relation)
        {
            string result = $"{relation.EGN} {relation.FirstName} {relation.SurName} {relation.FamilyName} {relation.RelationCode}";
            return result;
        }

        private static string PersonToString(ValidPersonResponse person)
        {
            string result = $"{person.BirthDate.ToString()} {person.FirstName} {person.SurName} {person.FamilyName} {person.DeathDate.ToString()}";
            return result;
        }

        private static string PensionRightToString(PensionRightResponse pensionRight)
        {
            string result = $"{pensionRight.Identifier} {pensionRight.Names.Name} {pensionRight.Names.Surname} {pensionRight.Names.FamilyName} {pensionRight.Status}";
            return result;
        }

        private static XElement GetResponseNodeFromRawResponse<T>(string rawResponse)
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

        private static async Task<RegiXResponse> GetRegiXResponse(ReportType reportType)
        {
            try
            {
                RegiXResponse response = null;
                switch (reportType)
                {
                    case ReportType.PersonDataSearch:
                        response = await UtilityTest.TestPersonDataSearchAsync("8506258485");
                        break;
                    case ReportType.ValidPersonSearch:
                        response = await UtilityTest.TestValidPersonSearchAsync("8506258485");
                        break;
                    case ReportType.ValidUICInfo:
                        response = await UtilityTest.TestValidUICInfoAsync();
                        break;
                    case ReportType.ActualState:
                        response = await UtilityTest.TestActualStateAsync();
                        break;
                    case ReportType.StateOfPlay:
                        response = await UtilityTest.TestStateOfPlayAsync();
                        break;
                    case ReportType.MotorVehicleRegistrationInfoV3:
                        response = await UtilityTest.TestMotorVehicleRegistrationInfoV3Async();
                        break;
                    case ReportType.AircraftsByOwner:
                        response = await UtilityTest.TestAircraftsByOwnerAsync();
                        break;
                    case ReportType.AircraftsByMSN:
                        response = await UtilityTest.TestAircraftsByMSNAsync();
                        break;
                    case ReportType.Relations:
                        response = await UtilityTest.TestRelationsRequestAsync("8506258485");
                        break;
                    case ReportType.PensionRightInfoReport:
                        response = await UtilityTest.TestPensionRightInfoReportAsync("8506258485");
                        break;
                    case ReportType.EmploymentContracts:
                        response = await UtilityTest.TestEmploymentContractsAsync("8506258485");
                        break;
                    case ReportType.JobSeekerStatus:
                        response = await UtilityTest.TestJobSeekerStatusAsync("8506258485");
                        break;
                    default:
                        break;
                }
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting RegiX response: " + ex.Message);
                return null;
            }
        }


    }
}
