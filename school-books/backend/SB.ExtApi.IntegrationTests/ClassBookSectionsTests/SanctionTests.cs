namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class SanctionTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private static readonly DateTime date = new(2023, 4, 6);
    private const string descriptionUpdate = "DescriptionUpdate";
    private readonly SanctionDO sanction;
    private readonly ExtApiWebApplicationFactory appFactory;

    public SanctionTests(
        OrderedFixtures<
            Tuple<
                CreateSnapshotFixture,
                DataFixture_V_XII,
                DataFixture_V_XII_TA,
                DataFixture_PG,
                ExtApiWebApplicationFactory
            >> fixtures)
    {
        DataFixture_V_XII data = fixtures.Values.Item2;
        ExtApiWebApplicationFactory appFactory = fixtures.Values.Item5;

        this.appFactory = appFactory;
        this.schoolYear = data.SchoolYear;
        this.institutionId = data.InstitutionId;
        this.classBookId = data.ClassBookId;
        this.classId = data.ClassId;
        this.personId = data.PersonId;
        this.sanction =
            new SanctionDO
            {
                ClassId = data.ClassId,
                PersonId = data.PersonId,
                Type = 1,
                OrderNumber = "SanctionOrderNumber",
                OrderDate = date,
                StartDate = date,
                EndDate = null,
                Description = "SanctionDescription",
                CancelOrderNumber = null,
                CancelOrderDate = null,
                CancelReason = null,
                RuoOrderNumber = null,
                RuoOrderDate = null,
            };
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_sanctions_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSanctionsGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_sanction()
    {
        var sanctionId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSanctionsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.sanction);
        Assert.InRange(sanctionId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_sanction()
    {
        var sanctionId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSanctionsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.sanction);

        var sanctionUpdateData =
            new SanctionDO
            {
                ClassId = this.classId,
                PersonId = this.personId,
                Type = 2,
                OrderNumber = "OrderNumber",
                OrderDate = date.AddDays(1),
                StartDate = date.AddDays(2),
                EndDate = date.AddDays(3),
                Description = descriptionUpdate,
                CancelOrderNumber = "CancelOrderNumber",
                CancelOrderDate = date.AddDays(4),
                CancelReason = "CancelReason",
                RuoOrderNumber = "RuoOrderNumber",
                RuoOrderDate = date.AddDays(5),
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSanctionsPutAsync(this.schoolYear, this.institutionId, this.classBookId, sanctionId, sanctionUpdateData);

        var sanctionUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSanctionsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.SanctionId == sanctionId)
           .SingleOrDefault();

        Assert.NotNull(sanctionUpdated);
        Assert.Equal(2, sanctionUpdated.Type);
        Assert.Equal("OrderNumber", sanctionUpdated.OrderNumber);
        Assert.Equal(date.AddDays(1), sanctionUpdated.OrderDate);
        Assert.Equal(date.AddDays(2), sanctionUpdated.StartDate);
        Assert.Equal(date.AddDays(3), sanctionUpdated.EndDate);
        Assert.Equal(descriptionUpdate, sanctionUpdated.Description);
        Assert.Equal("CancelOrderNumber", sanctionUpdated.CancelOrderNumber);
        Assert.Equal(date.AddDays(4), sanctionUpdated.CancelOrderDate);
        Assert.Equal("CancelReason", sanctionUpdated.CancelReason);
        Assert.Equal("RuoOrderNumber", sanctionUpdated.RuoOrderNumber);
        Assert.Equal(date.AddDays(5), sanctionUpdated.RuoOrderDate);
    }

    [Fact]
    public async Task Should_remove_classbook_sanction()
    {
        var sanctionId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSanctionsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.sanction);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSanctionsDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, sanctionId);

        var sanctionDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSanctionsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(s => s.SanctionId == sanctionId)
            .SingleOrDefault();

        Assert.Null(sanctionDeleted);
    }
}
