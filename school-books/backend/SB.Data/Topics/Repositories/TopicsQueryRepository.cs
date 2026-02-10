namespace SB.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.ITopicsQueryRepository;

internal class TopicsQueryRepository : Repository, ITopicsQueryRepository
{
    public TopicsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllForWeekVO[]> GetAllForWeekAsync(
    int schoolYear,
    int classBookId,
    int year,
    int weekNumber,
    CancellationToken ct)
    {
        DateTime startDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 1);
        DateTime endDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 7);

        bool isDplrClassBook =
            await this.DbContext.Set<ClassBook>()
                .AnyAsync(cb =>
                    cb.SchoolYear == schoolYear &&
                    cb.ClassBookId == classBookId &&
                    cb.BookType == ClassBookType.Book_DPLR, ct);

        var query = await (
            from t in this.DbContext.Set<Topic>()
            where t.SchoolYear == schoolYear &&
                  t.ClassBookId == classBookId &&
                  t.Date >= startDate &&
                  t.Date <= endDate
            select new
            {
                t.TopicId,
                Titles = t.Titles.Select(tt => tt.Title).ToArray(),
                Teachers = t.Teachers.Select(tt => new { tt.PersonId, tt.IsReplTeacher }).ToArray(),
                t.Date,
                t.ScheduleLessonId,
                t.CreateDate,
                t.CreatedBySysUserId
            }
        ).AsSplitQuery().ToArrayAsync(ct);

        var studentsByLessonId = new Dictionary<int, int[]>();
        if (isDplrClassBook)
        {
            var scheduleLessonIds = query
                .Select(t => t.ScheduleLessonId)
                .ToArray();

            var studentData = await (
                from a in this.DbContext.Set<Absence>()
                where a.SchoolYear == schoolYear &&
                      this.DbContext.MakeIdsQuery(scheduleLessonIds)
                        .Any(id => a.ScheduleLessonId == id.Id) &&
                      a.Type == AbsenceType.DplrAttendance
                select new { a.PersonId, a.ScheduleLessonId }
            ).ToArrayAsync(ct);

            studentsByLessonId = studentData
                .GroupBy(s => s.ScheduleLessonId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(s => s.PersonId).ToArray()
                );
        }

        var allPersonIds = query
            .SelectMany(t => t.Teachers.Select(tt => tt.PersonId))
            .Concat(studentsByLessonId.Values.SelectMany(ids => ids))
            .ToArray();

        var personData = await this.DbContext.Set<Person>()
            .Where(p => allPersonIds.Contains(p.PersonId))
            .Select(p => new { p.PersonId, p.FirstName, p.LastName })
            .ToDictionaryAsync(p => p.PersonId, p => new { p.FirstName, p.LastName }, ct);

        return query.Select(t =>
            new GetAllForWeekVO(
                t.TopicId,
                t.Titles,
                t.Teachers.Select(tt =>
                {
                    var names = personData[tt.PersonId];
                    return new GetAllForWeekVOTeacher(tt.PersonId, names.FirstName, names.LastName, tt.IsReplTeacher);
                }).ToArray(),
                studentsByLessonId.GetValueOrDefault(t.ScheduleLessonId, Array.Empty<int>())
                    .Select(studentId =>
                    {
                        var names = personData[studentId];
                        return new GetAllForWeekVOStudent(studentId, names.FirstName, names.LastName);
                    }).ToArray(),
                t.Date,
                t.ScheduleLessonId,
                t.CreateDate,
                t.CreatedBySysUserId
            )
        ).ToArray();
    }

    public async Task<GetUndoInfoByIdsVO[]> GetUndoInfoByIdsAsync(int schoolYear, int[] topicIds, CancellationToken ct)
    {
        return await (
            from t in this.DbContext.Set<Topic>()

            where t.SchoolYear == schoolYear &&
                this.DbContext.MakeIdsQuery(topicIds).Any(id => t.TopicId == id.Id)

            select new GetUndoInfoByIdsVO(
                t.TopicId,
                t.CreateDate,
                t.CreatedBySysUserId)
        ).ToArrayAsync(ct);
    }

    public async Task<int[]> GetCurriculumsWithTopicPlanAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        return await (
            from t in this.DbContext.Set<ClassBookTopicPlanItem>()
            where t.SchoolYear == schoolYear &&
                t.ClassBookId == classBookId
            select t.CurriculumId
        ).Distinct().ToArrayAsync(ct);
    }

    public async Task<GetCurriculumTopiPlanVO[]> GetCurriculumTopiPlanAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct)
    {
        return await (
            from tpi in this.DbContext.Set<ClassBookTopicPlanItem>()

            where tpi.SchoolYear == schoolYear && tpi.ClassBookId == classBookId && tpi.CurriculumId == curriculumId

            orderby tpi.Number, tpi.CreateDate

            select new GetCurriculumTopiPlanVO(
                tpi.ClassBookTopicPlanItemId,
                tpi.Number,
                tpi.Title,
                tpi.Taken))
            .ToArrayAsync(ct);
    }

    public async Task<GetScheduleLessonsTeachersVO[]> GetScheduleLessonsTeachersAsync(
        int schoolYear,
        int[] scheduleLessonIds,
        CancellationToken ct)
    {
        var teachers = await (
            from sl in this.DbContext.Set<ScheduleLesson>()
            join cteacher in this.DbContext.Set<CurriculumTeacher>() on sl.CurriculumId equals cteacher.CurriculumId
            join sp in this.DbContext.Set<StaffPosition>() on cteacher.StaffPositionId equals sp.StaffPositionId

            join tah in this.DbContext.Set<TeacherAbsenceHour>()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
            into g1
            from tah in g1.DefaultIfEmpty()

            where sl.SchoolYear == schoolYear &&
                this.DbContext.MakeIdsQuery(scheduleLessonIds)
                    .Any(id => sl.ScheduleLessonId == id.Id) &&
                (cteacher.IsValid || tah.ReplTeacherPersonId != null)

            select new
            {
                sl.ScheduleLessonId,
                PersonId = tah.ReplTeacherPersonId ?? sp.PersonId,
                IsReplTeacher = tah.ReplTeacherPersonId != null
            }
        ).ToArrayAsync(ct);

        return teachers
            .Select(t => new GetScheduleLessonsTeachersVO(t.ScheduleLessonId, t.PersonId, t.IsReplTeacher))
            // when a ScheduleLesson has a TeacherAbsenceHour the ReplTeacherPersonId
            // will be returned as many times as the number of curriculum teachers so
            // we need to distinct the result
            .Distinct()
            .ToArray();
    }

    public async Task<bool> ExistsVerifiedScheduleLessonForTopicsAsync(
        int schoolYear,
        int[] topicIds,
        CancellationToken ct)
    {
        return await (
            from t in this.DbContext.Set<Topic>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { t.SchoolYear, t.ScheduleLessonId } equals new { sl.SchoolYear, sl.ScheduleLessonId }

            where t.SchoolYear == schoolYear &&
                this.DbContext
                    .MakeIdsQuery(topicIds)
                    .Any(id => t.TopicId == id.Id) &&
                sl.IsVerified

            select t
        ).AnyAsync(ct);
    }

    public async Task<bool> ExistsTopicStudentsInInstitution(
        int schoolYear,
        int instId,
        int[] studentPersonIds,
        CancellationToken ct)
    {
        return await this.DbContext.Set<StudentClass>()
            .Where(sc =>
                sc.SchoolYear == schoolYear &&
                sc.InstitutionId == instId &&
                sc.IsNotPresentForm == false &&
                this.DbContext
                    .MakeIdsQuery(studentPersonIds)
                    .Any(id => sc.PersonId == id.Id))
            .Select(p => p.PersonId)
            .Distinct()
            .CountAsync(cancellationToken: ct) == studentPersonIds.Length;
    }
}
