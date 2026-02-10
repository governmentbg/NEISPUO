namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class AbsenceTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private readonly int personIdExcuse;
    private readonly int personIdRemove;
    private readonly int scheduleLessonId;
    private readonly DateTime scheduleLessonDate;
    private readonly ExtApiWebApplicationFactory appFactory;

    public AbsenceTests(
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
        this.personIdExcuse = data.PersonIdUpdate;
        this.personIdRemove = data.PersonIdRemove;
        this.scheduleLessonId = data.ScheduleLessonId;
        this.scheduleLessonDate = data.ScheduleLessonDate;
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_absences_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAbsencesGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_throw_403_on_forbidden_institutionId()
    {
        var fakeInstituionId = 999999;

        var ex = await Assert.ThrowsAsync<ApiException>(
            async () => await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAbsencesGetAsync(this.schoolYear, fakeInstituionId, this.classBookId));

        Assert.Equal(403, ex.StatusCode);
    }

    [Fact]
    public async Task Should_throw_404_on_not_matching_classBookId()
    {
        var fakeInstituionId = 300110;

        var ex = await Assert.ThrowsAsync<ApiException>(
            async () => await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAbsencesGetAsync(this.schoolYear, fakeInstituionId, this.classBookId));

        Assert.Equal(404, ex.StatusCode);
    }

    [Fact]
    public async Task Should_create_classbook_absence()
    {
        var absenceId = await this.CreateAbsenceAsync(this.personId);
        Assert.InRange(absenceId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_excuse_classbook_absence()
    {
        var excuse =
            new ExcusedReasonDO
            {
                ExcusedReason = 1,
                ExcusedReasonComment = "ExcusedReasonComment"
            };

        var absenceId = await this.CreateAbsenceAsync(this.personIdExcuse);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAbsencesExcuseAsync(this.schoolYear, this.institutionId, this.classBookId, absenceId, excuse);

        var absenceExcused = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAbsencesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.AbsenceId == absenceId)
           .SingleOrDefault();

        Assert.NotNull(absenceExcused);
        Assert.Equal(1, absenceExcused.ExcusedReason);
        Assert.Equal("ExcusedReasonComment", absenceExcused.ExcusedReasonComment);
    }

    [Fact]
    public async Task Should_remove_classbook_absence()
    {
        var absenceId = await this.CreateAbsenceAsync(this.personIdRemove);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAbsencesDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, absenceId);

        var absenceDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAbsencesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(a => a.AbsenceId == absenceId)
            .SingleOrDefault();

        Assert.Null(absenceDeleted);
    }

    private async Task<int> CreateAbsenceAsync(int personId)
    {
        var absence =
            new AbsenceDO
            {
                ClassId = this.classId,
                PersonId = personId,
                Date = this.scheduleLessonDate,
                Type = AbsenceType.Unexcused,
                ExcusedReason = null,
                ExcusedReasonComment = null,
                ScheduleLessonId = this.scheduleLessonId
            };

        return await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAbsencesPostAsync(this.schoolYear, this.institutionId, this.classBookId, absence);
    }
}
