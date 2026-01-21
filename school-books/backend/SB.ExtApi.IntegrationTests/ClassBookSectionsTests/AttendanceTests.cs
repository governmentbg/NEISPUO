namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class AttendanceTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private readonly int personIdExcuse;
    private readonly int personIdRemove;
    private readonly DateTime date = new(2025, 9, 16);
    private readonly DateTime outsideSchoolYearDate = new(2025, 8, 16);
    private readonly ExtApiWebApplicationFactory appFactory;

    public AttendanceTests(
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
        this.classId = data.ClassId;
        this.personId = data.PersonId;
        this.personIdExcuse = data.PersonIdUpdate;
        this.personIdRemove = data.PersonIdRemove;
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_attendances_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAttendancesGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_attendance()
    {
        var attendanceId = await this.CreateAttendanceAsync(this.personId);
        Assert.InRange(attendanceId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_excuse_classbook_attendance()
    {
        var attendanceId = await this.CreateAttendanceAsync(this.personIdExcuse);

        var excuse =
            new ExcusedReasonDO
            {
                ExcusedReason = 1,
                ExcusedReasonComment = "ExcusedReasonComment"
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAttendancesExcuseAsync(this.schoolYear, this.institutionId, this.classBookId, attendanceId, excuse);

        var attendanceExcused = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAttendancesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.AttendanceId == attendanceId)
           .SingleOrDefault();

        Assert.NotNull(attendanceExcused);
        Assert.Equal(1, attendanceExcused.ExcusedReason);
        Assert.Equal("ExcusedReasonComment", attendanceExcused.ExcusedReasonComment);
    }

    [Fact]
    public async Task Should_remove_classbook_attendance()
    {
        var attendanceId = await this.CreateAttendanceAsync(this.personIdRemove);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAttendancesDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, attendanceId);

        var attendanceDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAttendancesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(a => a.AttendanceId == attendanceId)
            .SingleOrDefault();

        Assert.Null(attendanceDeleted);
    }

    [Fact]
    public async Task Should_throw_on_attendance_outside_school_year_limits()
    {
        var ex = await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(
            async () => await this.CreateAttendanceAsync(this.personId, this.outsideSchoolYearDate));

        Assert.Contains("Cannot create attendance outside of school year limits", ex.Message);
    }

    [Fact]
    public async Task Should_return_400_on_duplicate_attendance()
    {
        await this.CreateAttendanceAsync(this.personId);

        var ex = await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(
            async () => await this.CreateAttendanceAsync(this.personId));

        Assert.Equal(409, ex.StatusCode);
        Assert.Contains("Violation of UNIQUE KEY constraint 'UK_Attendance_ClassBookId_PersonId_Date'", ex.Message);
    }

    private async Task<int> CreateAttendanceAsync(int personId, DateTime? date = null)
    {
        var attendance =
            new AttendanceDO
            {
                ClassId = this.classId,
                PersonId = personId,
                Date = date ?? this.date,
                Type = AttendanceType.UnexcusedAbsence,
                ExcusedReason = null,
                ExcusedReasonComment = null
            };

        return await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksAttendancesPostAsync(this.schoolYear, this.institutionId, this.classBookId, attendance);
    }
}
