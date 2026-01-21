namespace SB.ExtApi.IntegrationTests;

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class HisMedicalNoticesTests : RestoreSnapshotFixture
{
    private readonly ExtApiWebApplicationFactory appFactory;

    public HisMedicalNoticesTests(
        OrderedFixtures<
            Tuple<
                CreateSnapshotFixture,
                DataFixture_V_XII,
                DataFixture_V_XII_TA,
                DataFixture_PG,
                ExtApiWebApplicationFactory
            >> fixtures)
    {
        ExtApiWebApplicationFactory appFactory = fixtures.Values.Item5;

        this.appFactory = appFactory;
    }

    [Fact]
    public async Task Post_HisMedicalNotices_succeeds()
    {
        // Act
        await this.appFactory.CreateExtApiClient(TestExtSystem.HIS).HisMedicalNoticesAsync(
            new HisMedicalNoticeDO[]
            {
                new()
                {
                    NrnMedicalNotice = "23234B000001",
                    NrnExamination = "23031B000001",
                    Patient = new()
                    {
                        IdentifierType = 1,
                        Identifier = "1234567890",
                        GivenName = "ИВАН",
                        FamilyName = "ТОДОРОВ"
                    },
                    Practitioner =
                    new()
                    {
                        Pmi = "2300011922"
                    },
                    MedicalNotice =
                    new()
                    {
                        FromDate = DateTime.Today.AddDays(-1),
                        ToDate = DateTime.Today.AddDays(1),
                        AuthoredOn = DateTime.Today
                    }
                },
            });
    }

    [Fact]
    public async Task Post_HisMedicalNotices_with_empty_patient_names_succeeds()
    {
        // Act
        var requestBody = new HisMedicalNoticeDO[]
        {
            new()
            {
                NrnMedicalNotice = "23234B000001",
                NrnExamination = "23031B000001",
                Patient = new HisMedicalNoticePatientDO
                {
                    IdentifierType = 1,
                    Identifier = "1234567890",
                    GivenName = null,
                    FamilyName = null

                },
                Practitioner =
                    new HisMedicalNoticePractitionerDO
                    {
                        Pmi = "2300011922"
                    },
                MedicalNotice =
                    new HisMedicalNoticeInfoDO
                    {
                        FromDate = DateTime.Today.AddDays(-1),
                        ToDate = DateTime.Today.AddDays(1),
                        AuthoredOn = DateTime.Today
                    }
            }
        };

        HttpClient client = this.CreateCustomClient();
        var jsonBody = JsonSerializer.Serialize(requestBody);

        var content = new StringContent(
            jsonBody,
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync(
            "/api/v1/hisMedicalNotices",
            content
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private HttpClient CreateCustomClient()
    {
        var client = this.appFactory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-Client-Cert", RequestCertificates.HISCertificateString);
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-Forwarded-Proto", "HTTPS");
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-Forwarded-For", "127.0.0.1");

        return client;
    }
}
