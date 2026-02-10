namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class PgResultTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private const string startSchoolYearResultUpdate = "StartSchoolYearResultUpdate";
    private const string endSchoolYearResultUpdate = "EndSchoolYearResultUpdate";
    private readonly PgResultDO[] pgResults;
    private readonly ExtApiWebApplicationFactory appFactory;
    private readonly DataFixture_V_XII data;

    public PgResultTests(
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
        this.pgResults =
            new PgResultDO[]
            {
                new()
                {
                    ClassId = data.ClassId,
                    PersonId = data.PersonId,
                    CurriculumId = null,
                    StartSchoolYearResult = "StartSchoolYearResult",
                    EndSchoolYearResult = "EndSchoolYearResult",
                },
                new()
                {
                    ClassId = null,
                    PersonId = data.PersonId,
                    CurriculumId = data.CurriculumId,
                    StartSchoolYearResult = "StartSchoolYearResult",
                    EndSchoolYearResult = "EndSchoolYearResult",
                },
                new()
                {
                    ClassId = null,
                    PersonId = data.PersonId,
                    SubjectId = data.SubjectIdUpdate,
                    StartSchoolYearResult = "StartSchoolYearResult",
                    EndSchoolYearResult = "EndSchoolYearResult",
                }
            };
        this.data = data;
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_pgResults_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPgResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Theory, InlineData(0), InlineData(1), InlineData(2)]
    public async Task Should_create_classbook_pgResult(int pgResultIndex)
    {
        PgResultDO pgResult = this.pgResults[pgResultIndex];

        // Act
        var pgResultId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider)
            .ClassBooksPgResultsPostAsync(
                this.schoolYear,
                this.institutionId,
                this.classBookId,
                pgResult);

        // Verify
        var pgResults = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider)
            .ClassBooksPgResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId);

        var comparer = KeyEqualityComparer<PgResultDO>.Create(
            r => (
                r.PersonId,
                r.SubjectId,
                r.CurriculumId,
                r.StartSchoolYearResult,
                r.EndSchoolYearResult));

        Assert.InRange(pgResultId, 1, int.MaxValue);

        if (pgResultIndex == 1)
        {
            // the extapi will automatically populate the SubjectId
            pgResult.SubjectId = this.data.SubjectId;
        }
        Assert.Equal(new[] { pgResult }, pgResults, comparer);
    }

    [Theory, InlineData(0), InlineData(1)]
    public async Task Should_update_classbook_pgResult(int pgResultIndex)
    {
        PgResultDO pgResult = this.pgResults[pgResultIndex];

        // Setup
        var pgResultId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider)
            .ClassBooksPgResultsPostAsync(
                this.schoolYear,
                this.institutionId,
                this.classBookId,
                pgResult);

        var pgResultUpdateData =
            new PgResultDO
            {
                StartSchoolYearResult = startSchoolYearResultUpdate,
                EndSchoolYearResult = endSchoolYearResultUpdate,
            };

        // Act
        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPgResultsPutAsync(this.schoolYear, this.institutionId, this.classBookId, pgResultId, pgResultUpdateData);

        // Verify
        var pgResultUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPgResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.PgResultId == pgResultId)
           .SingleOrDefault();

        Assert.NotNull(pgResultUpdated);
        Assert.Equal(startSchoolYearResultUpdate, pgResultUpdated.StartSchoolYearResult);
        Assert.Equal(endSchoolYearResultUpdate, pgResultUpdated.EndSchoolYearResult);
    }

    [Theory, InlineData(0), InlineData(1)]
    public async Task Should_remove_classbook_pgResult(int pgResultIndex)
    {
        PgResultDO pgResult = this.pgResults[pgResultIndex];

        // Setup
        var pgResultId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider)
            .ClassBooksPgResultsPostAsync(
                this.schoolYear,
                this.institutionId,
                this.classBookId,
                pgResult);

        // Act
        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPgResultsDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, pgResultId);

        // Verify
        var pgResultDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPgResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(n => n.PgResultId == pgResultId)
            .SingleOrDefault();

        Assert.Null(pgResultDeleted);
    }
}
