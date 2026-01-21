namespace SB.ExtApi.IntegrationTests;

using System;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class AuthorizationTests : RestoreSnapshotFixture
{
    private readonly ExtApiWebApplicationFactory appFactory;

    public AuthorizationTests(
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

    public enum ExtApiEndpoint
    {
        Post_HisMedicalNotices = 1,
        Get_MedicalNoticesNext,
        Get_ClassBooks,
        Get_SchoolYearDateInfos
    }

    [Theory,
        InlineData(TestExtSystem.HIS, ExtApiEndpoint.Post_HisMedicalNotices, true),
        InlineData(TestExtSystem.HIS, ExtApiEndpoint.Get_ClassBooks, false),

        InlineData(TestExtSystem.SchoolBooksProvider, ExtApiEndpoint.Get_MedicalNoticesNext, true),
        InlineData(TestExtSystem.SchoolBooksProvider, ExtApiEndpoint.Post_HisMedicalNotices, false),

        InlineData(TestExtSystem.ScheduleProvider, ExtApiEndpoint.Get_ClassBooks, true),
        InlineData(TestExtSystem.ScheduleProvider, ExtApiEndpoint.Get_SchoolYearDateInfos, false),
    ]
    public async Task TestExtSystem_access(TestExtSystem testExtSystem, ExtApiEndpoint extApiEndpoint, bool shouldHaveAccess)
    {
        bool hasAccess;
        try
        {
            await this.CallEndpoint(extApiEndpoint, testExtSystem);
            hasAccess = true;
        }
        catch (ApiException ex) when (ex.StatusCode == 403)
        {
            hasAccess = false;
        }

        Assert.Equal(shouldHaveAccess, hasAccess);
    }

    private async Task CallEndpoint(ExtApiEndpoint extApiEndpoint, TestExtSystem testExtSystem)
    {
        switch(extApiEndpoint)
        {
            case ExtApiEndpoint.Post_HisMedicalNotices:
                await this.Post_HisMedicalNotices(testExtSystem);
                break;
            case ExtApiEndpoint.Get_MedicalNoticesNext:
                await this.Get_MedicalNoticesNext(testExtSystem);
                break;
            case ExtApiEndpoint.Get_ClassBooks:
                await this.Get_ClassBooks(testExtSystem);
                break;
            case ExtApiEndpoint.Get_SchoolYearDateInfos:
                await this.Get_SchoolYearDateInfos(testExtSystem);
                break;
        };
    }

    private async Task Post_HisMedicalNotices(TestExtSystem testExtSystem)
    {
        await this.appFactory.CreateExtApiClient(testExtSystem).HisMedicalNoticesAsync(
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

    private async Task Get_MedicalNoticesNext(TestExtSystem testExtSystem)
    {
        await this.appFactory.CreateExtApiClient(testExtSystem).MedicalNoticesNextAsync(100);
    }

    private async Task Get_ClassBooks(TestExtSystem testExtSystem)
    {
        await this.appFactory.CreateExtApiClient(testExtSystem).ClassBooksGetAsync(2023, 2206409);
    }

    private async Task Get_SchoolYearDateInfos(TestExtSystem testExtSystem)
    {
        await this.appFactory.CreateExtApiClient(testExtSystem).SchoolYearDateInfosGetAsync(2023, 2206409);
    }
}
