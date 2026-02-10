namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class GradeTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly DataFixture_V_XII data;
    private readonly GradeDO grade;
    private readonly GradeDO gradeWithoutScheduleLesson;
    private readonly GradeDO gradeWithoutCurriculum;
    private readonly GradeDO termGradeWithoutScheduleLesson;
    private readonly GradeDO finalGrade;
    private readonly ExtApiWebApplicationFactory appFactory;

    public GradeTests(
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
        this.data = data;
        this.grade =
            new GradeDO
            {
                ClassId = data.ClassId,
                PersonId = data.PersonId,
                CurriculumId = data.ScheduleLessonCurriculumId,
                Date = data.ScheduleLessonDate,
                Category = GradeCategory.Decimal,
                Type = GradeType.Homework,
                DecimalGrade = 5m,
                QualitativeGrade = null,
                SpecialGrade = null,
                Comment = null,
                ScheduleLessonId = data.ScheduleLessonId
            };

        this.gradeWithoutScheduleLesson =
            new GradeDO
            {
                ClassId = data.ClassId,
                PersonId = data.PersonId,
                CurriculumId = data.ScheduleLessonCurriculumId,
                Date = data.ScheduleLessonDate,
                Category = GradeCategory.Decimal,
                Type = GradeType.Homework,
                DecimalGrade = 5m,
                QualitativeGrade = null,
                SpecialGrade = null,
                Comment = null,
                ScheduleLessonId = null
            };

        this.gradeWithoutCurriculum =
            new GradeDO
            {
                ClassId = data.ClassId,
                PersonId = data.PersonId,
                CurriculumId = null,
                Date = data.ScheduleLessonDate,
                Category = GradeCategory.Decimal,
                Type = GradeType.Homework,
                DecimalGrade = 5m,
                QualitativeGrade = null,
                SpecialGrade = null,
                Comment = null,
                ScheduleLessonId = data.ScheduleLessonId
            };

        this.termGradeWithoutScheduleLesson =
            new GradeDO
            {
                ClassId = data.ClassId,
                PersonId = data.PersonId,
                CurriculumId = data.ScheduleLessonCurriculumId,
                Date = new DateTime(data.SchoolYear + 1, 03, 01),
                Category = GradeCategory.Decimal,
                Type = GradeType.Term,
                DecimalGrade = 5m,
                QualitativeGrade = null,
                SpecialGrade = null,
                Comment = null,
                ScheduleLessonId = null,
                Term = SchoolTerm.TermOne
            };

        this.finalGrade =
            new GradeDO
            {
                ClassId = data.ClassId,
                PersonId = data.PersonId,
                CurriculumId = data.ScheduleLessonCurriculumId,
                Date = new DateTime(data.SchoolYear + 1, 03, 01),
                Category = GradeCategory.Decimal,
                Type = GradeType.Final,
                DecimalGrade = 5m,
                QualitativeGrade = null,
                SpecialGrade = null,
                Comment = null,
                ScheduleLessonId = null,
                Term = null
            };
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_grades_successfully()
    {
        // Act
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesGetAsync(this.schoolYear, this.institutionId, this.classBookId);

        // Verify
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_decimal_grade()
    {
        // Act
        var gradeId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.grade);

        // Verify
        Assert.InRange(gradeId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_remove_classbook_grade()
    {
        // Setup
        var gradeId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.grade);

        // Act
        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, gradeId);

        // Verify
        var gradeDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .SingleOrDefault(g => g.GradeId == gradeId);

        Assert.Null(gradeDeleted);
    }

    [Fact]
    public async Task Should_throw_on_grade_without_schedule_lesson()
    {
        // Act
        var ex = await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(
            async () => await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.gradeWithoutScheduleLesson));

        // Verify
        Assert.Contains("'Schedule Lesson Id' must not be empty", ex.Message);
    }

    [Fact]
    public async Task Should_create_term_grade_without_schedule_lesson()
    {
        // Act
        var gradeId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.termGradeWithoutScheduleLesson);

        // Verify
        Assert.InRange(gradeId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_create_grade_without_curriculum()
    {
        // Act
        var gradeId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.gradeWithoutCurriculum);

        // Verify
        Assert.InRange(gradeId, 1, int.MaxValue);

        var gradeCreated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .SingleOrDefault(g => g.GradeId == gradeId);

        Assert.NotNull(gradeCreated);
        Assert.Equal(this.data.ScheduleLessonCurriculumId, gradeCreated.CurriculumId);
    }

    [Fact]
    public async Task Should_throw_on_double_sent_term_grade()
    {
        // Act
        var ex = await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(
            async () =>
            {
                await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.termGradeWithoutScheduleLesson);
                await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.termGradeWithoutScheduleLesson);
            });

        // Verify
        Assert.Contains("Ученикът вече има въведена срочна оценка оценка за първи срок.", ex.Message);
    }

    [Fact]
    public async Task Should_throw_on_double_sent_final_grade()
    {
        // Act
        var ex = await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(
            async () =>
            {
                await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.finalGrade);
                await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGradesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.finalGrade);
            });

        // Verify
        Assert.Contains("Ученикът вече има въведена годишна оценка.", ex.Message);
    }
}
