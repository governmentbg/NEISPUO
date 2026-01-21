namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Xunit;

public sealed class DataFixture_V_XII_TA : IAsyncLifetime
{
    private const int schoolYear = 2025;
    private const int institutionId = 300125;

    public async Task InitializeAsync()
    {
        await using var connection = new SqlConnection(DatabaseUtils.ConnectionString);

        var studentDataIds =
            (await DataQueries.GetStudentDataIdsAsync(connection, schoolYear, institutionId, ClassBookType.Book_V_XII))
            .GroupBy(x => x.ClassBookId)
            .Where(r => r.Where(s => !s.IsTransferred).DistinctBy(s => s.PersonId).Count() >= 3)
            .FirstOrDefault();

        if (studentDataIds != null)
        {
            this.ClassBookId = studentDataIds.Key;
            var scheduleLessonIds = await DataQueries.GetUnusedScheduleLessonIdsAsync(connection, schoolYear, this.ClassBookId);
            var scheduleLessonId = scheduleLessonIds[0].ScheduleLessonId;
            var teacherId = await DataQueries.GetTeacherAbsenceTeacherIdAsync(connection, schoolYear, institutionId, scheduleLessonId);
            this.TeacherAbsenceScheduleLessonId = scheduleLessonId;
            this.TeacherAbsenceScheduleLessonDate = scheduleLessonIds[0].Date;
            this.TeacherAbsenceTeacherId = teacherId;
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public int SchoolYear => schoolYear;

    public int InstitutionId => institutionId;

    public int ClassBookId { get; private set; }

    public int TeacherAbsenceTeacherId { get; private set; }

    public int TeacherAbsenceScheduleLessonId { get; private set; }

    public DateTime TeacherAbsenceScheduleLessonDate { get; private set; }
}
