namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Xunit;

public sealed class DataFixture_V_XII : IAsyncLifetime
{
    private const int schoolYear = 2025;
    private const int institutionId = 300125;

    public async Task InitializeAsync()
    {
        await using var connection = new SqlConnection(DatabaseUtils.ConnectionString);
        this.ClassIdWithNoClassBook = await DataQueries.ClassIdWithNoClassBookOrDefaultAsync(connection, schoolYear, institutionId);
        this.SupportTeacherIds = await DataQueries.GetSupportTeacherIdsAsync(connection, schoolYear, institutionId);

        var studentDataIds =
            (await DataQueries.GetStudentDataIdsAsync(connection, schoolYear, institutionId, ClassBookType.Book_V_XII))
            .GroupBy(x => x.ClassBookId)
            .Where(r => r.Where(s => !s.IsTransferred).DistinctBy(s => s.PersonId).Count() >= 3)
            .FirstOrDefault();

        if (studentDataIds != null)
        {
            this.ClassBookId = studentDataIds.Key;
            this.AllClassBookStudentClasses = studentDataIds
                .Select(s => (
                    classId: s.ClassId,
                    personId: s.PersonId,
                    isTransferred: s.IsTransferred
                )).ToArray();

            var distinctEnrolledStudentDataIds = studentDataIds.Where(s => !s.IsTransferred).DistinctBy(s => s.PersonId);
            this.ClassId = distinctEnrolledStudentDataIds.ElementAt(0).ClassId;
            this.PersonId = distinctEnrolledStudentDataIds.ElementAt(0).PersonId;
            this.PersonIdUpdate = distinctEnrolledStudentDataIds.ElementAt(1).PersonId;
            this.PersonIdRemove = distinctEnrolledStudentDataIds.ElementAt(2).PersonId;
            this.PersonId2 = distinctEnrolledStudentDataIds.ElementAt(3).PersonId;
            this.PersonIdUpdate2 = distinctEnrolledStudentDataIds.ElementAt(4).PersonId;
            this.PersonId3 = distinctEnrolledStudentDataIds.ElementAt(5).PersonId;

            var shiftDataIds = await DataQueries.GetScheduleDataIdsAsync(connection, schoolYear, this.ClassBookId);
            this.ShiftId = shiftDataIds[0].ShiftId;
            this.ScheduleLessonId = shiftDataIds[0].ScheduleLessonId;
            this.ScheduleLessonDate = shiftDataIds[0].Date;
            this.ScheduleLessonCurriculumId = shiftDataIds[0].CurriculumId;
            this.ScheduleLessonIdRemove = shiftDataIds[1].ScheduleLessonId;
            this.ScheduleLessonDateRemove = shiftDataIds[1].Date;

            var curriculumDataIds = await DataQueries.GetCurriculumDataIdsAsync(connection, schoolYear, institutionId, this.ClassId, this.PersonId);
            this.CurriculumId = curriculumDataIds[0].CurriculumId;
            this.CurriculumIdUpdate = curriculumDataIds[^1].CurriculumId;
            this.SubjectId = curriculumDataIds[0].SubjectId;
            this.SubjectIdUpdate = curriculumDataIds[^1].SubjectId;
            this.ClassBookCurriculumIds = curriculumDataIds.Select(x => x.CurriculumId).ToArray();

            var classIdWithNoClassBookCurriculumDataIds = await DataQueries.GetCurriculumDataIdsAsync(connection, this.ClassIdWithNoClassBook);
            this.ClassIdWithNoClassBookCurriculumIdUpdate = classIdWithNoClassBookCurriculumDataIds.Last();
            this.ClassIdWithNoClassBookCurriculumIds = classIdWithNoClassBookCurriculumDataIds;

            var scheduleLessonId = shiftDataIds[0].ScheduleLessonId;
            var teacherId = await DataQueries.GetTeacherAbsenceTeacherIdAsync(connection, schoolYear, institutionId, scheduleLessonId);
            this.TeacherAbsenceScheduleLessonId = scheduleLessonId;
            this.TeacherAbsenceScheduleLessonDate = shiftDataIds[0].Date;
            this.TeacherAbsenceTeacherId = teacherId;
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public int SchoolYear => schoolYear;

    public int InstitutionId => institutionId;

    public int ClassIdWithNoClassBook { get; private set; }

    public int[] SupportTeacherIds { get; private set; } = Array.Empty<int>();

    public int ClassBookId { get; private set; }

    public (int classId, int personId, bool isTransferred)[] AllClassBookStudentClasses { get; private set; } = Array.Empty<(int classId, int personId, bool isTransferred)>();

    public int ClassId { get; private set; }

    public int PersonId { get; private set; }

    public int PersonIdUpdate { get; private set; }

    public int PersonIdRemove { get; private set; }

    public int PersonId2 { get; private set; }

    public int PersonIdUpdate2 { get; private set; }

    public int PersonId3 { get; private set; }

    public int ShiftId { get; private set; }

    public int ScheduleLessonId { get; private set; }

    public DateTime ScheduleLessonDate { get; private set; }

    public int ScheduleLessonCurriculumId { get; private set; }

    public int ScheduleLessonIdRemove { get; private set; }

    public DateTime ScheduleLessonDateRemove { get; private set; }

    public int CurriculumId { get; private set; }

    public int CurriculumIdUpdate { get; private set; }

    public int SubjectId { get; private set; }

    public int SubjectIdUpdate { get; private set; }

    public int[] ClassBookCurriculumIds { get; private set; } = Array.Empty<int>();

    public int ClassIdWithNoClassBookCurriculumIdUpdate { get; private set; }

    public int[] ClassIdWithNoClassBookCurriculumIds { get; private set; } = Array.Empty<int>();

    public int TeacherAbsenceTeacherId { get; private set; }

    public int TeacherAbsenceScheduleLessonId { get; private set; }

    public DateTime TeacherAbsenceScheduleLessonDate { get; private set; }
}
