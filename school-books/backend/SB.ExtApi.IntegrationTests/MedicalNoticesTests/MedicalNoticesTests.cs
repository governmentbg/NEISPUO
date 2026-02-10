namespace SB.ExtApi.IntegrationTests;

using System;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class MedicalNoticesTests : RestoreSnapshotFixture
{
    private readonly ExtApiWebApplicationFactory appFactory;

    public MedicalNoticesTests(
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
    public async Task Get_MedicalNotices_succeeds()
    {
        // Act
        var batch = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).MedicalNoticesNextAsync(100);

        // Assert
        Assert.NotNull(batch.LastHisSyncTime);
        Assert.False(batch.HasMore);
        Assert.NotNull(batch.MedicalNotices);
        Assert.Equal(3, batch.MedicalNotices.Count);
    }
}
