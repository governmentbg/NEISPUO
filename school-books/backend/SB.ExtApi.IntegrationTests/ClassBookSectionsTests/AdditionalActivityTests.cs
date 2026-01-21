namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SB.Common;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class AdditionalActivityTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private const string activityUpdate = "ActivityUpdate";
    private static readonly DateTime date = new(2025, 10, 3);
    private readonly AdditionalActivityDO additionalActivity;
    private readonly ExtApiWebApplicationFactory appFactory;

    public AdditionalActivityTests(
        OrderedFixtures<
            Tuple<
                CreateSnapshotFixture,
                DataFixture_V_XII,
                DataFixture_V_XII_TA,
                DataFixture_PG,
                ExtApiWebApplicationFactory
            >> fixtures)
    {
        DataFixture_PG data = fixtures.Values.Item4;
        ExtApiWebApplicationFactory appFactory = fixtures.Values.Item5;
        this.appFactory = appFactory;
        this.schoolYear = data.SchoolYear;
        this.institutionId = data.InstitutionId;
        this.classBookId = data.ClassBookId;
        this.additionalActivity =
            new AdditionalActivityDO
            {
                Year = date.GetIsoWeekYear(),
                WeekNumber = date.GetIsoWeek(),
                Activity = "Activity"
            };
    }


    [Fact]
    public async Task GetAll_returns_all_classbook_additionalActivities_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAdditionalActivitiesGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_additionalActivity()
    {
        var additionalActivityId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAdditionalActivitiesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.additionalActivity);
        Assert.InRange(additionalActivityId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_additionalActivity()
    {
        var additionalActivityId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAdditionalActivitiesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.additionalActivity);

        var additionalActivityUpdateData =
            new AdditionalActivityDO
            {
                Activity = activityUpdate
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAdditionalActivitiesPutAsync(this.schoolYear, this.institutionId, this.classBookId, additionalActivityId, additionalActivityUpdateData);

        var additionalActivityUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAdditionalActivitiesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.AdditionalActivityId == additionalActivityId)
           .SingleOrDefault();

        Assert.NotNull(additionalActivityUpdated);
        Assert.Equal(activityUpdate, additionalActivityUpdated.Activity);
    }

    [Fact]
    public async Task Should_remove_classbook_additionalActivity()
    {
        var additionalActivityId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAdditionalActivitiesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.additionalActivity);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAdditionalActivitiesDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, additionalActivityId);

        var additionalActivityDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAdditionalActivitiesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(e => e.AdditionalActivityId == additionalActivityId)
            .SingleOrDefault();

        Assert.Null(additionalActivityDeleted);
    }
}
