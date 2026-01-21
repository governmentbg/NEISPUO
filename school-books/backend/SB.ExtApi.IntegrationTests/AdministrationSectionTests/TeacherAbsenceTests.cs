namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class TeacherAbsenceTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int teacherPersonId;
    private readonly int scheduleLessonId;
    private readonly DateTime date;
    private const string reasonUpdate = "ReasonUpdate";
    private readonly TeacherAbsenceDO TeacherAbsence;
    private readonly ExtApiWebApplicationFactory appFactory;

    public TeacherAbsenceTests(
        OrderedFixtures<
            Tuple<
                CreateSnapshotFixture,
                DataFixture_V_XII,
                DataFixture_V_XII_TA,
                DataFixture_PG,
                ExtApiWebApplicationFactory
            >> fixtures)
    {
        DataFixture_V_XII_TA data = fixtures.Values.Item3;
        ExtApiWebApplicationFactory appFactory = fixtures.Values.Item5;

        this.appFactory = appFactory;
        this.schoolYear = data.SchoolYear;
        this.institutionId = data.InstitutionId;
        this.teacherPersonId = data.TeacherAbsenceTeacherId;
        this.scheduleLessonId = data.TeacherAbsenceScheduleLessonId;
        this.date = data.TeacherAbsenceScheduleLessonDate.AddDays(-1);

        this.TeacherAbsence =
            new TeacherAbsenceDO()
            {
                TeacherPersonId = this.teacherPersonId,
                StartDate = this.date,
                EndDate = this.date.AddDays(1),
                Reason = "Reason",
                Hours = new TeacherAbsenceHourDO[]
                {
                    new()
                    {
                        ScheduleLessonId = this.scheduleLessonId,
                        Type = TeacherAbsenceHourType.EmptyHour,
                        ReplTeacherPersonId = null
                    }
                }
            };
    }

    [Fact]
    public async Task GetAll_returns_all_teacherAbsences_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).TeacherAbsencesGetAsync(this.schoolYear, this.institutionId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_teacherAbsence()
    {
        var teacherAbsenceId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).TeacherAbsencesPostAsync(this.schoolYear, this.institutionId, this.TeacherAbsence);
        Assert.InRange(teacherAbsenceId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_teacherAbsence()
    {
        var teacherAbsenceId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).TeacherAbsencesPostAsync(this.schoolYear, this.institutionId, this.TeacherAbsence);

        var teacherAbsenceUpdateData =
            new TeacherAbsenceDO()
            {
                TeacherPersonId = this.teacherPersonId,
                TeacherAbsenceId = teacherAbsenceId,
                StartDate = this.date,
                EndDate = this.date.AddDays(1),
                Reason = reasonUpdate,
                Hours = new TeacherAbsenceHourDO[]
                {
                    new()
                    {
                        ScheduleLessonId = this.scheduleLessonId,
                        Type = TeacherAbsenceHourType.EmptyHour,
                        ReplTeacherPersonId = null
                    }
                }
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).TeacherAbsencesPutAsync(this.schoolYear, this.institutionId, teacherAbsenceId, teacherAbsenceUpdateData);

        var teacherAbsenceUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).TeacherAbsencesGetAsync(this.schoolYear, this.institutionId))
           .Where(e => e.TeacherAbsenceId == teacherAbsenceId)
           .SingleOrDefault();

        Assert.NotNull(teacherAbsenceUpdated);
        Assert.Equal(reasonUpdate, teacherAbsenceUpdated.Reason);
        Assert.Equal(1, teacherAbsenceUpdated.Hours.Count);
        Assert.Equal(this.date, teacherAbsenceUpdated.StartDate);
        Assert.Equal(this.date.AddDays(1), teacherAbsenceUpdated.EndDate);
        Assert.Equal(TeacherAbsenceHourType.EmptyHour, teacherAbsenceUpdated.Hours.First().Type);
    }

    [Fact]
    public async Task Should_remove_classbook_teacherAbsence()
    {
        var teacherAbsenceId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).TeacherAbsencesPostAsync(this.schoolYear, this.institutionId, this.TeacherAbsence);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).TeacherAbsencesDeleteAsync(this.schoolYear, this.institutionId, teacherAbsenceId);

        var teacherAbsenceDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).TeacherAbsencesGetAsync(this.schoolYear, this.institutionId))
            .Where(s => s.TeacherAbsenceId == teacherAbsenceId)
            .SingleOrDefault();

        Assert.Null(teacherAbsenceDeleted);
    }
}
