namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class ExamTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly DateTime date = new(2023, 4, 6);
    private readonly int curriculumIdUpdate;
    private const string descriptionUpdate = "DescriptionUpdate";
    private readonly ExamDO exam;
    private readonly ExtApiWebApplicationFactory appFactory;

    public ExamTests(
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
        this.curriculumIdUpdate = data.CurriculumIdUpdate;
        this.exam =
            new ExamDO
            {
                Type = BookExamType.ClassExam,
                CurriculumId = data.CurriculumId,
                Date = this.date,
                Description = null
            };
    }


    [Fact]
    public async Task GetAll_returns_all_classbook_exams_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksExamsGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_exam()
    {
        var examId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksExamsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.exam);
        Assert.InRange(examId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_exam()
    {
        var examId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksExamsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.exam);

        var examUpdateData =
            new ExamDO
            {
                Type = BookExamType.ClassExam,
                CurriculumId = this.curriculumIdUpdate,
                Date = this.date.AddDays(7),
                Description = descriptionUpdate
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksExamsPutAsync(this.schoolYear, this.institutionId, this.classBookId, examId, examUpdateData);

        var examUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksExamsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.ExamId == examId)
           .SingleOrDefault();

        Assert.NotNull(examUpdated);
        Assert.Equal(this.curriculumIdUpdate, examUpdated.CurriculumId);
        Assert.Equal(this.date.AddDays(7), examUpdated.Date);
        Assert.Equal(descriptionUpdate, examUpdated.Description);
    }

    [Fact]
    public async Task Should_remove_classbook_exam()
    {
        var examId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksExamsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.exam);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksExamsDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, examId);

        var examDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksExamsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(e => e.ExamId == examId)
            .SingleOrDefault();

        Assert.Null(examDeleted);
    }
}
