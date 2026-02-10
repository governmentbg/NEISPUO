namespace SB.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.ITopicsDplrQueryRepository;

internal class TopicsDplrQueryRepository : Repository, ITopicsDplrQueryRepository
{
    public TopicsDplrQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllDplrForWeekVO[]> GetAllForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        CancellationToken ct)
    {
        DateTime startDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 1);
        DateTime endDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 7);

        var topics = await (
            from t in this.DbContext.Set<TopicDplr>()

            join c in this.DbContext.Set<Curriculum>() on new { t.SchoolYear, t.CurriculumId } equals new { c.SchoolYear, c.CurriculumId }
            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where t.SchoolYear == schoolYear &&
                t.ClassBookId == classBookId &&
                t.Date >= startDate &&
                t.Date <= endDate

            select new
            {
                t.TopicDplrId,
                t.Title,
                t.Location,
                Teachers = t.Teachers.Select(tt => tt.PersonId).ToArray(),
                t.Date,
                t.Day,
                t.HourNumber,
                t.CurriculumId,
                s.SubjectName,
                s.SubjectNameShort,
                SubjectTypeName = st.Name,
                t.Students,
                t.CreateDate,
                t.CreatedBySysUserId
            }
        )
        .AsSplitQuery()
        .ToArrayAsync(ct);

        var teacherPersonIds = topics.SelectMany(t => t.Teachers).Distinct().ToArray();
        var teacherNames = (await (
            from p in this.DbContext.Set<Person>()

            where this.DbContext.MakeIdsQuery(teacherPersonIds).Any(id => p.PersonId == id.Id)

            select new
            {
                p.PersonId,
                p.FirstName,
                p.LastName
            }
        ).ToArrayAsync(ct))
        .ToDictionary(
            p => p.PersonId,
            p => new { p.FirstName, p.LastName });

        var studentPersonIds = topics
            .SelectMany(t => t.Students.Select(s => s.PersonId)).Distinct()
            .ToArray();
        var studentNames = (await (
                from p in this.DbContext.Set<Person>()

                where this.DbContext.MakeIdsQuery(studentPersonIds).Any(id => p.PersonId == id.Id)

                select new
                {
                    p.PersonId,
                    p.FirstName,
                    p.LastName
                }
            ).ToArrayAsync(ct))
            .ToDictionary(
                p => p.PersonId,
                p => new { p.FirstName, p.LastName });

        return topics
            .Select(t =>
                new GetAllDplrForWeekVO(
                    t.TopicDplrId,
                    t.Date,
                    t.Day,
                    t.HourNumber,
                    t.CurriculumId,
                    t.SubjectName,
                    t.SubjectNameShort,
                    t.SubjectTypeName,
                    t.Title,
                    t.Location,
                    t.Teachers
                        .Select(teacherId =>
                        {
                            var teacher = teacherNames[teacherId];
                            return new GetAllDplrForWeekVOTeacher(
                                teacherId,
                                teacher.FirstName,
                                teacher.LastName);
                        })
                        .ToArray(),
                    t.Students
                        .Select(student =>
                        {
                            var currentStudent = studentNames[student.PersonId];
                            return new GetAllDplrForWeekVOStudent(
                                student.PersonId,
                                currentStudent.FirstName,
                                currentStudent.LastName);
                        })
                        .ToArray(),
                    t.CreateDate,
                    t.CreatedBySysUserId))
            .ToArray();
    }

    public async Task<GetUndoInfoByIdsVO[]> GetTopicsDplrUndoInfoByIdAsync(
        int schoolYear,
        int topicDplrId,
        CancellationToken ct)
    {
        return await (
            from t in this.DbContext.Set<TopicDplr>()

            where t.SchoolYear == schoolYear &&
                  t.TopicDplrId == topicDplrId

            select new GetUndoInfoByIdsVO(
                t.TopicDplrId,
                t.CreateDate,
                t.CreatedBySysUserId)
        ).ToArrayAsync(ct);
    }

    public async Task<int[]> GetCurriculumTeacherIdsAsync(
        int schoolYear,
        int curriculumId,
        CancellationToken ct)
    {
        return await (
            from cct in this.DbContext.Set<CurriculumTeacher>()
            join sp in this.DbContext.Set<StaffPosition>() on cct.StaffPositionId equals sp.StaffPositionId

            where cct.SchoolYear == schoolYear &&
                  cct.CurriculumId == curriculumId &&
                  cct.IsValid

            select sp.PersonId
        ).ToArrayAsync(ct);
    }

    public async Task<bool> ExistsTopicStudentsInInstitution(
        int schoolYear,
        int instId,
        int[] studentPersonIds,
        CancellationToken ct)
    {
        var dbStudentsIds = await this.DbContext.Set<StudentClass>()
            .Where(sc =>
                sc.SchoolYear == schoolYear &&
                sc.InstitutionId == instId &&
                sc.IsNotPresentForm == false &&
                this.DbContext
                    .MakeIdsQuery(studentPersonIds)
                    .Any(id => sc.PersonId == id.Id))
            .Select(p => p.PersonId)
            .Distinct()
            .ToArrayAsync(ct);

        var dbStudentsSet = new HashSet<int>(dbStudentsIds);
        var studentPersonIdsSet = new HashSet<int>(studentPersonIds);

        return dbStudentsSet.SetEquals(studentPersonIdsSet);
    }

    public async Task<bool> ExistsVerifiedTopicDplrAsync(
        int schoolYear,
        int topicDplrId,
        CancellationToken ct)
    {
        return await (
            from t in this.DbContext.Set<TopicDplr>()

            where t.SchoolYear == schoolYear &&
                  t.TopicDplrId == topicDplrId &&
                  t.IsVerified

            select t
        ).AnyAsync(ct);
    }

    public async Task<int[]> GetExistingHourNumbersForDateAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        CancellationToken ct)
    {
        return await (
            from t in this.DbContext.Set<TopicDplr>()

            where t.SchoolYear == schoolYear &&
                  t.ClassBookId == classBookId &&
                  t.Date == date

            select t.HourNumber
        ).ToArrayAsync(ct);
    }
}
