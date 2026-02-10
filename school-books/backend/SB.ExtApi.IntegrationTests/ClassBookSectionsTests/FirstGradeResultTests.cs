namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class FirstGradeResultTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private readonly int personIdUpdate;
    private readonly int personIdRemove;
    private readonly int personId2;
    private readonly int personIdUpdate2;
    private readonly int personId3;
    private readonly ExtApiWebApplicationFactory appFactory;

    public FirstGradeResultTests(
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
        this.personIdUpdate = data.PersonIdUpdate;
        this.personIdRemove = data.PersonIdRemove;
        this.personId2 = data.PersonId2;
        this.personIdUpdate2 = data.PersonIdUpdate2;
        this.personId3 = data.PersonId3;
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_firstGradeResults_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksFirstGradeResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_firstGradeResult()
    {
        var firstGradeResultId = await this.CreateFirstGradeResultAsync(this.personId, QualitativeGrade.VeryGood, null);
        Assert.InRange(firstGradeResultId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_firstGradeResult()
    {
        var firstGradeResultId = await this.CreateFirstGradeResultAsync(this.personIdUpdate, QualitativeGrade.VeryGood, null);

        var firstGradeResultUpdateData =
           new FirstGradeResultDO
           {
               QualitativeGrade = QualitativeGrade.Excellent,
               SpecialGrade = null
           };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksFirstGradeResultsPutAsync(this.schoolYear, this.institutionId, this.classBookId, firstGradeResultId, firstGradeResultUpdateData);

        var firstGradeResultUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksFirstGradeResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(fgr => fgr.FirstGradeResultId == firstGradeResultId)
            .SingleOrDefault();

        Assert.NotNull(firstGradeResultUpdated);
        Assert.Equal(QualitativeGrade.Excellent, firstGradeResultUpdated.QualitativeGrade);
    }

    [Fact]
    public async Task Should_create_classbook_firstGradeResult_with_special_grade()
    {
        var firstGradeResultId = await this.CreateFirstGradeResultAsync(this.personId2, null, SpecialNeedsGrade.DoingOk);
        Assert.InRange(firstGradeResultId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_firstGradeResult_with_special_grade()
    {
        var firstGradeResultId = await this.CreateFirstGradeResultAsync(this.personIdUpdate2, null, SpecialNeedsGrade.DoingOk);

        var firstGradeResultUpdateData =
           new FirstGradeResultDO
           {
               QualitativeGrade = null,
               SpecialGrade = SpecialNeedsGrade.MeetsExpectations
           };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksFirstGradeResultsPutAsync(this.schoolYear, this.institutionId, this.classBookId, firstGradeResultId, firstGradeResultUpdateData);

        var firstGradeResultUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksFirstGradeResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(fgr => fgr.FirstGradeResultId == firstGradeResultId)
            .SingleOrDefault();

        Assert.NotNull(firstGradeResultUpdated);
        Assert.Equal(SpecialNeedsGrade.MeetsExpectations, firstGradeResultUpdated.SpecialGrade);
    }

    [Fact]
    public async Task Should_remove_classbook_firstGradeResult()
    {
        var firstGradeResultId = await this.CreateFirstGradeResultAsync(this.personIdRemove, QualitativeGrade.VeryGood, null);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksFirstGradeResultsDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, firstGradeResultId);

        var firstGradeResultDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksFirstGradeResultsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(fgr => fgr.FirstGradeResultId == firstGradeResultId)
            .SingleOrDefault();

        Assert.Null(firstGradeResultDeleted);
    }

    [Fact]
    public async Task Should_throw_on_firstGradeResult_with_both_grades_specified()
    {
        var ex = await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(
            async () => await this.CreateFirstGradeResultAsync(this.personId, QualitativeGrade.Good, SpecialNeedsGrade.DoingOk));

        Assert.Contains("Exactly one of QualitativeGrade, SpecialGrade must not be null", ex.Message);
    }

    [Fact]
    public async Task Should_throw_on_firstGradeResult_update_with_incompatible_grade()
    {
        var firstGradeResultId = await this.CreateFirstGradeResultAsync(this.personId3, QualitativeGrade.VeryGood, null);

        var firstGradeResultUpdateData =
           new FirstGradeResultDO
           {
               QualitativeGrade = null,
               SpecialGrade = SpecialNeedsGrade.HasDificulty
           };

        var ex = await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(
            async () => await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksFirstGradeResultsPutAsync(this.schoolYear, this.institutionId, this.classBookId, firstGradeResultId, firstGradeResultUpdateData));

        Assert.Contains("Cannot change the grade type of existing first grade result", ex.Message);
    }

    private async Task<int> CreateFirstGradeResultAsync(int personId, QualitativeGrade? qualitativeGrade, SpecialNeedsGrade? specialGrade)
    {
        var firstGradeResult =
            new FirstGradeResultDO
            {
                ClassId = this.classId,
                PersonId = personId,
                QualitativeGrade = qualitativeGrade,
                SpecialGrade = specialGrade
            };

        return await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksFirstGradeResultsPostAsync(this.schoolYear, this.institutionId, this.classBookId, firstGradeResult);
    }
}
