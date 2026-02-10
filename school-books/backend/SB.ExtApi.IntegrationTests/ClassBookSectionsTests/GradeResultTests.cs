namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class GradeResultTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private readonly int personId2;
    private readonly int personIdUpdate;
    private readonly int personIdRemove;
    private readonly int curriculumId;
    private readonly ExtApiWebApplicationFactory appFactory;

    public GradeResultTests(
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
        this.personId2 = data.PersonId2;
        this.personIdUpdate = data.PersonIdUpdate;
        this.personIdRemove = data.PersonIdRemove;
        this.curriculumId = data.CurriculumId;
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_gradeResults_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradeResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_gradeResult_with_grades()
    {
        var gradeResult =
            new GradeResultDO
            {
                ClassId = this.classId,
                PersonId = this.personId,
                InitialResultType = GradeResultType.MustRetakeExams,
                RetakeExamCurriculums =
                    new GradeResultCurriculumDO[]
                    {
                        new GradeResultCurriculumDO
                        {
                            CurriculumId = this.curriculumId,
                            Session1Grade = 3,
                            Session1NoShow = false
                        },
                    },
                FinalResultType = null
            };

        var gradeResultId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradeResultsPostAsync(
            this.schoolYear,
            this.institutionId,
            this.classBookId,
            gradeResult);

        Assert.InRange(gradeResultId, 1, int.MaxValue);

        var gradeResultCreated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradeResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.GradeResultId == gradeResultId)
           .SingleOrDefault();

        Assert.NotNull(gradeResultCreated);
        Assert.Equal(this.curriculumId, gradeResultCreated.RetakeExamCurriculums.FirstOrDefault()?.CurriculumId);
        Assert.Equal(3, gradeResultCreated.RetakeExamCurriculums.FirstOrDefault()?.Session1Grade);
    }

    [Fact]
    public async Task Should_create_classbook_gradeResult_with_RepeatsGrade_for_InitialResultType()
    {
        var gradeResult =
            new GradeResultDO
            {
                ClassId = this.classId,
                PersonId = this.personId2,
                InitialResultType = GradeResultType.RepeatsGrade,
                RetakeExamCurriculums = Array.Empty<GradeResultCurriculumDO>(),
                FinalResultType = null
            };

        var gradeResultId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradeResultsPostAsync(
            this.schoolYear,
            this.institutionId,
            this.classBookId,
            gradeResult);

        Assert.InRange(gradeResultId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_gradeResult()
    {
        var gradeResultId = await this.CreateGradeResultAsync(this.personIdUpdate);
        var gradeResultUpdateData =
            new GradeResultDO
            {
                ClassId = this.classId,
                PersonId = this.personId,
                InitialResultType = GradeResultType.CompletesGrade,
                RetakeExamCurriculums = Array.Empty<GradeResultCurriculumDO>(),
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradeResultsPutAsync(this.schoolYear, this.institutionId, this.classBookId, gradeResultId, gradeResultUpdateData);

        var gradeResultUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradeResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.GradeResultId == gradeResultId)
           .SingleOrDefault();

        Assert.NotNull(gradeResultUpdated);
        Assert.Equal(GradeResultType.CompletesGrade, gradeResultUpdated.InitialResultType);
        Assert.Equal(0, gradeResultUpdated.RetakeExamCurriculums.Count);
        Assert.Null(gradeResultUpdated.FinalResultType);
    }

    [Fact]
    public async Task Should_remove_classbook_gradeResult()
    {
        var gradeResultId = await this.CreateGradeResultAsync(this.personIdRemove);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradeResultsDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, gradeResultId);

        var gradeResultDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradeResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(gr => gr.GradeResultId == gradeResultId)
            .SingleOrDefault();

        Assert.Null(gradeResultDeleted);
    }

    private async Task<int> CreateGradeResultAsync(int personId)
    {
        var gradeResult =
            new GradeResultDO
            {
                ClassId = this.classId,
                PersonId = personId,
                InitialResultType = GradeResultType.MustRetakeExams,
                RetakeExamCurriculums = new[]
                {
                    new GradeResultCurriculumDO
                    {
                        CurriculumId = this.curriculumId,
                        Session1Grade = 3,
                        Session1NoShow = false
                    }
                },
                FinalResultType = null
            };

        return await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradeResultsPostAsync(this.schoolYear, this.institutionId, this.classBookId, gradeResult);
    }
}
