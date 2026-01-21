namespace SB.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IAbsencesDplrQueryRepository;

internal class AbsencesDplrQueryRepository : Repository, IAbsencesDplrQueryRepository
{
    public AbsencesDplrQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllDplrForClassBookVO[]> GetAllDplrForClassBookAsync(
        int schoolYear,
        int classBookId,
        AbsenceType type,
        int? curriculumId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct)
    {
        this.AssertIsDplrAbsenceType(type);

        string sql = $@"
            SELECT
                a.PersonId,
                SUM(IIF(a.Type = @type, 1, 0)) AS Count
            FROM
                [school_books].[Absence] a
                INNER JOIN [school_books].[ScheduleLesson] sl ON a.SchoolYear = sl.SchoolYear AND a.ScheduleLessonId = sl.ScheduleLessonId
            WHERE
                a.SchoolYear = @schoolYear AND
                a.ClassBookId = @classBookId AND
                {(curriculumId != null ?
                    "sl.CurriculumId = @curriculumId AND"
                    : "1 = 1 AND")}
                {(fromDate != null ?
                    "a.Date >= @fromDate AND"
                    : "1 = 1 AND")}
                {(toDate != null ?
                    "a.Date <= @toDate"
                    : "1 = 1")}
            GROUP BY
                a.PersonId
            ";

        var parameters = new List<SqlParameter>()
        {
            new SqlParameter("schoolYear", schoolYear),
            new SqlParameter("classBookId", classBookId),
            new SqlParameter("type", type)
        };

        if (curriculumId != null)
        {
            parameters.Add(new SqlParameter("curriculumId", curriculumId));
        }

        if (fromDate != null)
        {
            parameters.Add(new SqlParameter("fromDate", fromDate));
        }
        if (toDate != null)
        {
            parameters.Add(new SqlParameter("toDate", toDate));
        }

        return await this.DbContext.Set<GetAllDplrForClassBookVO>()
            .FromSqlRaw(sql, parameters.ToArray())
            .ToArrayAsync(ct);
    }

    public async Task<GetAllDplrForStudentAndTypeVO[]> GetAllDplrForStudentAndTypeAsync(
        int schoolYear,
        int classBookId,
        int studentPersonId,
        AbsenceType type,
        int? curruculumId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct)
    {
        this.AssertIsDplrAbsenceType(type);

        var absencePredicate = PredicateBuilder.True<Absence>();
        absencePredicate = absencePredicate.AndDateGreaterThanOrEqual(p => p.Date, fromDate);
        absencePredicate = absencePredicate.AndDateLessThanOrEqual(p => p.Date, toDate);

        var scheduleLessonPredicate = PredicateBuilder.True<ScheduleLesson>();
        scheduleLessonPredicate = scheduleLessonPredicate.AndEquals(p => p.CurriculumId, curruculumId);

        return await (
            from a in this.DbContext.Set<Absence>().Where(absencePredicate)

            join sl in this.DbContext.Set<ScheduleLesson>().Where(scheduleLessonPredicate)
            on new { a.SchoolYear, a.ScheduleLessonId }
            equals new { sl.SchoolYear, sl.ScheduleLessonId }

            join c in this.DbContext.Set<Curriculum>() on sl.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join csu in this.DbContext.Set<SysUser>() on a.CreatedBySysUserId equals csu.SysUserId

            join cp in this.DbContext.Set<Person>() on csu.PersonId equals cp.PersonId

            join tah in this.DbContext.Set<TeacherAbsenceHour>()
            on new { a.SchoolYear, a.ScheduleLessonId, a.TeacherAbsenceId } equals new { tah.SchoolYear, tah.ScheduleLessonId, TeacherAbsenceId = (int?)tah.TeacherAbsenceId }
            into g1 from tah in g1.DefaultIfEmpty()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.PersonId == studentPersonId &&
                a.Type == type

            orderby a.Date descending, a.CreateDate descending

            select new GetAllDplrForStudentAndTypeVO(
                a.AbsenceId,
                a.PersonId,
                a.ScheduleLessonId,
                s.SubjectName,
                s.SubjectNameShort,
                st.Name,
                a.Date,
                a.Type,
                EnumUtils.GetEnumDescription(a.Type),
                tah.ReplTeacherIsNonSpecialist,
                a.CreateDate,
                a.CreatedBySysUserId,
                cp.FirstName,
                cp.MiddleName,
                cp.LastName)
        ).ToArrayAsync(ct);
    }

    public async Task<GetAllDplrForWeekVO[]> GetAllDplrForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        AbsenceType type,
        CancellationToken ct)
    {
        this.AssertIsDplrAbsenceType(type);

        DateTime startDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 1);
        DateTime endDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 7);

        return await (
            from a in this.DbContext.Set<Absence>()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.Type == type &&
                a.Date >= startDate &&
                a.Date <= endDate

            select new GetAllDplrForWeekVO(
                a.AbsenceId,
                a.PersonId,
                a.Date,
                a.ScheduleLessonId,
                a.CreateDate,
                a.CreatedBySysUserId)
        ).ToArrayAsync(ct);
    }

    private void AssertIsDplrAbsenceType(AbsenceType type)
    {
        if (type != AbsenceType.DplrAbsence &&
            type != AbsenceType.DplrAttendance)
        {
            throw new Exception("Dplr absence type assertion failed");
        }
    }
}
