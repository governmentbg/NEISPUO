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
using static SB.Domain.IAbsencesQueryRepository;

internal class AbsencesQueryRepository : Repository, IAbsencesQueryRepository
{
    public AbsencesQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllForClassBookVO[]> GetAllForClassBookAsync(
        int schoolYear,
        int classBookId,
        int? curriculumId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct)
    {
        string sql = $@"
            WITH Absences AS (
                SELECT
                    PersonId,
                    SUM(IIF(Type = {(int)AbsenceType.Late}, 1, 0)) AS LateAbsencesCount,
                    SUM(IIF(Type = {(int)AbsenceType.Unexcused}, 1, 0)) AS UnexcusedAbsencesCount,
                    SUM(IIF(Type = {(int)AbsenceType.Excused}, 1, 0)) AS ExcusedAbsencesCount
                FROM
                    [school_books].[Absence] a
                    INNER JOIN [school_books].[ScheduleLesson] sl ON a.SchoolYear = sl.SchoolYear AND a.ScheduleLessonId = sl.ScheduleLessonId
                WHERE
                    a.SchoolYear = @schoolYear AND a.ClassBookId = @classBookId AND
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
                    PersonId
            ),
            CarriedAbsences AS (
                SELECT
                    PersonId,
                    (FirstTermLateCount + SecondTermLateCount) AS CarriedLateAbsencesCount,
                    (FirstTermUnexcusedCount + SecondTermUnexcusedCount) AS CarriedUnexcusedAbsencesCount,
                    (FirstTermExcusedCount + SecondTermExcusedCount) AS CarriedExcusedAbsencesCount
                FROM
                    [school_books].[ClassBookStudentCarriedAbsence] ca
                WHERE
                    SchoolYear = @schoolYear AND ClassBookId = @classBookId
            )
            SELECT
                COALESCE(a.PersonId, ca.PersonId) AS PersonId,
                COALESCE(a.LateAbsencesCount, 0) AS LateAbsencesCount,
                COALESCE(a.UnexcusedAbsencesCount, 0) AS UnexcusedAbsencesCount,
                COALESCE(a.ExcusedAbsencesCount, 0) AS ExcusedAbsencesCount,
                COALESCE(ca.CarriedLateAbsencesCount, 0) AS CarriedLateAbsencesCount,
                COALESCE(ca.CarriedUnexcusedAbsencesCount, 0) AS CarriedUnexcusedAbsencesCount,
                COALESCE(ca.CarriedExcusedAbsencesCount, 0) AS CarriedExcusedAbsencesCount
            FROM
                Absences a
                FULL OUTER JOIN CarriedAbsences ca ON a.PersonId = ca.PersonId
            ";

        var parameters = new List<SqlParameter>()
        {
            new SqlParameter("schoolYear", schoolYear),
            new SqlParameter("classBookId", classBookId)
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

        return await this.DbContext.Set<GetAllForClassBookVO>()
            .FromSqlRaw(sql, parameters.ToArray())
            .ToArrayAsync(ct);
    }

    public async Task<GetAllForStudentAndTypeVO[]> GetAllForStudentAndTypeAsync(
        int schoolYear,
        int classBookId,
        int studentPersonId,
        AbsenceType type,
        int? curruculumId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct)
    {
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

            join msu in this.DbContext.Set<SysUser>() on a.ModifiedBySysUserId equals msu.SysUserId

            join mp in this.DbContext.Set<Person>() on msu.PersonId equals mp.PersonId

            join tah in this.DbContext.Set<TeacherAbsenceHour>()
            on new { a.SchoolYear, a.ScheduleLessonId, a.TeacherAbsenceId } equals new { tah.SchoolYear, tah.ScheduleLessonId, TeacherAbsenceId = (int?)tah.TeacherAbsenceId }
            into g1 from tah in g1.DefaultIfEmpty()

            join ar in this.DbContext.Set<AbsenceReason>() on a.ExcusedReasonId equals ar.Id
            into g2 from ar in g2.DefaultIfEmpty()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.PersonId == studentPersonId &&
                a.Type == type

            orderby a.Date descending, a.CreateDate descending

            select new GetAllForStudentAndTypeVO(
                a.AbsenceId,
                a.PersonId,
                a.ScheduleLessonId,
                s.SubjectName,
                s.SubjectNameShort,
                st.Name,
                a.Date,
                a.Type,
                EnumUtils.GetEnumDescription(a.Type),
                a.ExcusedReasonId,
                ar.Name,
                a.ExcusedReasonComment,
                tah.ReplTeacherIsNonSpecialist,
                a.IsReadFromParent,
                a.CreateDate,
                a.CreatedBySysUserId,
                cp.FirstName,
                cp.MiddleName,
                cp.LastName,
                a.ModifyDate,
                a.ModifiedBySysUserId,
                mp.FirstName,
                mp.MiddleName,
                mp.LastName
            )
        ).ToArrayAsync(ct);
    }

    public async Task<GetAllForWeekVO[]> GetAllForWeekAsync(int schoolYear, int classBookId, int year, int weekNumber, CancellationToken ct)
    {
        DateTime startDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 1);
        DateTime endDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 7);

        return await (
            from a in this.DbContext.Set<Absence>()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.Date >= startDate &&
                a.Date <= endDate

            select new GetAllForWeekVO(
                a.AbsenceId,
                a.PersonId,
                a.Type,
                a.Date,
                a.ScheduleLessonId,
                a.CreateDate,
                a.CreatedBySysUserId)
        ).ToArrayAsync(ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int absenceId,
        CancellationToken ct)
    {
        return await (
            from a in this.DbContext.Set<Absence>()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.AbsenceId == absenceId

            select new GetVO(
                a.AbsenceId,
                a.CreateDate,
                a.CreatedBySysUserId)
        ).SingleAsync(ct);
    }

    public async Task<bool> ExistsVerifiedScheduleLessonAsync(
        int schoolYear,
        int[] scheduleLessonIds,
        CancellationToken ct)
    {
        return await (
            from sl in this.DbContext.Set<ScheduleLesson>()
            where sl.SchoolYear == schoolYear &&
                this.DbContext
                    .MakeIdsQuery(scheduleLessonIds)
                    .Any(id => sl.ScheduleLessonId == id.Id) &&
                sl.IsVerified
            select sl
        ).AnyAsync(ct);
    }

    public async Task<bool> ExistsVerifiedScheduleLessonForAbsencesAsync(
        int schoolYear,
        int[] absenceIds,
        CancellationToken ct)
    {
        return await (
            from a in this.DbContext.Set<Absence>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { a.SchoolYear, a.ScheduleLessonId } equals new { sl.SchoolYear, sl.ScheduleLessonId }

            where a.SchoolYear == schoolYear &&
                this.DbContext
                    .MakeIdsQuery(absenceIds)
                    .Any(id => a.AbsenceId == id.Id) &&
                sl.IsVerified

            select a
        ).AnyAsync(ct);
    }
}
