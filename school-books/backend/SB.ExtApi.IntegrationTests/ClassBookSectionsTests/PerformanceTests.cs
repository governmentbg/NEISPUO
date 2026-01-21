namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class PerformanceTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private static readonly DateTime date = new(2023, 4, 6);
    private const string nameUpdate = "NameUpdate";
    private const string descriptionUpdate = "DescriptionUpdate";
    private const string locationUpdate = "LocationUpdate";
    private readonly PerformanceDO performance;
    private readonly ExtApiWebApplicationFactory appFactory;

    public PerformanceTests(
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
        this.performance =
            new PerformanceDO
            {
                PerformanceTypeId = 1,
                Name = "Name",
                StartDate = date,
                EndDate = date,
                Description = "PerformanceDescription",
                Location = "Location",
                StudentAwards = null
            };
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_performances_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPerformancesGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_performance()
    {
        var performanceId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPerformancesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.performance);
        Assert.InRange(performanceId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_performance()
    {
        var performanceId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPerformancesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.performance);

        var performanceUpdateData =
            new PerformanceDO
            {
                PerformanceTypeId = 2,
                Name = nameUpdate,
                StartDate = date.AddDays(1),
                EndDate = date.AddDays(2),
                Description = descriptionUpdate,
                Location = locationUpdate,
                StudentAwards = "StudentAwards"
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPerformancesPutAsync(this.schoolYear, this.institutionId, this.classBookId, performanceId, performanceUpdateData);

        var performanceUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPerformancesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.PerformanceId == performanceId)
           .SingleOrDefault();

        Assert.NotNull(performanceUpdated);
        Assert.Equal(2, performanceUpdated.PerformanceTypeId);
        Assert.Equal(nameUpdate, performanceUpdated.Name);
        Assert.Equal(date.AddDays(1), performanceUpdated.StartDate);
        Assert.Equal(date.AddDays(2), performanceUpdated.EndDate);
        Assert.Equal(descriptionUpdate, performanceUpdated.Description);
        Assert.Equal(locationUpdate, performanceUpdated.Location);
        Assert.Equal("StudentAwards", performanceUpdated.StudentAwards);
    }

    [Fact]
    public async Task Should_remove_classbook_performance()
    {
        var performanceId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPerformancesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.performance);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPerformancesDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, performanceId);

        var performanceDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPerformancesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(s => s.PerformanceId == performanceId)
            .SingleOrDefault();

        Assert.Null(performanceDeleted);
    }
}
