namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IMissingTopicsReportsQueryRepository;

internal class MissingTopicsReportsQueryRepository : Repository, IMissingTopicsReportsQueryRepository
{
    public MissingTopicsReportsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int sysUserId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<MissingTopicsReport>()
            join p in this.DbContext.Set<Person>() on r.TeacherPersonId equals p.PersonId
            into j0
            from p in j0.DefaultIfEmpty()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.MissingTopicsReportId,
                r.SchoolYear,
                EnumUtils.GetEnumDescription(r.Period),
                r.Year != null && r.Month != null ?
                    new DateTime(r.Year.Value, r.Month.Value, 1).ToString("yyyy-MM") : null,
                StringUtils.JoinNames(p.FirstName, p.LastName),
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int missingTopicsReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<MissingTopicsReport>()

            where r.SchoolYear == schoolYear && r.InstId == instId && r.MissingTopicsReportId == missingTopicsReportId

            select new GetVO(
                r.SchoolYear,
                r.MissingTopicsReportId,
                r.Period,
                r.Year,
                r.Month,
                r.TeacherPersonId,
                r.CreateDate)
         ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int missingTopicsReportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var query =
            from ri in this.DbContext.Set<MissingTopicsReportItem>()
            join r in this.DbContext.Set<MissingTopicsReport>() on new { ri.SchoolYear, ri.MissingTopicsReportId } equals new { r.SchoolYear, r.MissingTopicsReportId }
            where ri.SchoolYear == schoolYear && r.InstId == instId && ri.MissingTopicsReportId == missingTopicsReportId
            orderby ri.MissingTopicsReportItemId

            select new
            {
                ri.MissingTopicsReportItemId,
                ri.Date,
                ri.ClassBookName,
                ri.CurriculumName
            };

        int length = await query.CountAsync(ct);
        if (length == 0)
        {
            return TableResultVO.Empty<GetItemsVO>();
        }

        var result = await query
            .WithOffsetAndLimit(offset, limit)
            .ToArrayAsync(ct);

        var relatedTeacherNames =
            (await (
                from rit in this.DbContext.Set<MissingTopicsReportItemTeacher>()
                where rit.SchoolYear == schoolYear && rit.MissingTopicsReportId == missingTopicsReportId
                select new
                {
                    rit.MissingTopicsReportItemId,
                    TeacherNames = rit.PersonName,
                }
            ).ToArrayAsync(ct))
            .ToLookup(c => c.MissingTopicsReportItemId, c => c.TeacherNames);

        return new TableResultVO<GetItemsVO>(
            result
                .Select(
                    r => new GetItemsVO(
                        r.Date,
                        r.ClassBookName,
                        r.CurriculumName,
                        string.Join(", ", relatedTeacherNames[r.MissingTopicsReportItemId])))
                .ToArray(),
            length);
    }

    public async Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        MissingTopicsReportPeriod period,
        int? year,
        int? month,
        int? teacherPersonId,
        DateTime createDate,
        CancellationToken ct)
    {
        var query = (
            from cb in this.DbContext.Set<ClassBook>()
            join sch in this.DbContext.Set<Schedule>() on new { cb.SchoolYear, cb.ClassBookId } equals new { sch.SchoolYear, sch.ClassBookId }
            join stud in this.DbContext.Set<Person>() on sch.PersonId equals stud.PersonId
            into j1
            from student in j1.DefaultIfEmpty()

            join sl in this.DbContext.Set<ScheduleLesson>() on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }
            join cte in this.DbContext.Set<CurriculumTeacher>() on sl.CurriculumId equals cte.CurriculumId
            join sp in this.DbContext.Set<StaffPosition>() on cte.StaffPositionId equals sp.StaffPositionId
            join p in this.DbContext.Set<Person>() on sp.PersonId equals p.PersonId
            join yi in this.DbContext.Set<ClassBookSchoolYearSettings>() on new { cb.SchoolYear, cb.ClassBookId } equals new { yi.SchoolYear, yi.ClassBookId }
            join c in this.DbContext.Set<Curriculum>() on sl.CurriculumId equals c.CurriculumId
            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join tah in this.DbContext.Set<TeacherAbsenceHour>()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
            into j2 from tah in j2.DefaultIfEmpty()

            join rp in this.DbContext.Set<Person>() on tah.ReplTeacherPersonId equals rp.PersonId
            into j3
            from rp in j3.DefaultIfEmpty()

            join t in this.DbContext.Set<Topic>()
            on new { sl.SchoolYear, sl.ScheduleLessonId }
            equals new { t.SchoolYear, t.ScheduleLessonId }
            into j4 from t in j4.DefaultIfEmpty()

            join od in this.DbContext.Set<ClassBookOffDayDate>()
            on new { cb.SchoolYear, cb.ClassBookId, sl.Date }
            equals new { od.SchoolYear, od.ClassBookId, od.Date }
            into j5 from od in j5.DefaultIfEmpty()

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.IsValid &&
                od == null &&
                t == null &&
                (tah == null || tah.ReplTeacherPersonId != null) && // not an empty hour
                ((cte.SchoolYear <= 2022 && cte.IsValid) ||   // Getting only valid teachers for the date of the lesson
                 (cte.SchoolYear > 2022 &&
                  (cte.StaffPositionStartDate ?? cte.ValidFrom ?? new DateTime(schoolYear, 9, 1)) <= sl.Date &&
                  (cte.StaffPositionTerminationDate == null || cte.StaffPositionTerminationDate >= sl.Date || cte.NoReplacement)))

            orderby sl.Date, cb.BasicClassId, cb.FullBookName, sl.HourNumber ascending, c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

            select new
            {
                sl.ScheduleLessonId,
                sl.Date,
                cb.ClassBookId,
                sl.CurriculumId,
                CurriculumName =
                    (tah.ReplTeacherIsNonSpecialist == true ?
                        $"Гражданско образование (зам.)" :
                        $"{s.SubjectName} / {st.Name}{(tah == null ? string.Empty : !string.IsNullOrEmpty(tah.ExtReplTeacherName) ? " (външен лектор)" : " (зам.)" )}") +
                    ((c.IsIndividualLesson ?? 0) != 0 ? " (ИЧ)" : "") +
                    (sch.IsIndividualSchedule ?
                        $" (ИУП - {student.FirstName} {student.LastName})" :
                        c.IsIndividualCurriculum ?? false ? " (ИУП)" : ""),
                    ClassBookName = cb.FullBookName,
                StudentPersonId = sch.PersonId,
                TeacherPersonId = tah == null ? sp.PersonId : tah.ReplTeacherPersonId,
                TeacherPersonName = tah == null ? StringUtils.JoinNames(p.FirstName, p.LastName) : !string.IsNullOrEmpty(tah.ExtReplTeacherName) ? tah.ExtReplTeacherName : StringUtils.JoinNames(rp.FirstName, rp.LastName),
                yi.SchoolYearStartDate,
                yi.FirstTermEndDate,
                yi.SecondTermStartDate,
                SchoolYearEndDate = cb.BookType != ClassBookType.Book_PG ? yi.SchoolYearEndDate : yi.SchoolYearEndDateLimit
            });

        var predicate = PredicateBuilder.True(query);

        if (teacherPersonId != null)
        {
            predicate = predicate.And(mt => mt.TeacherPersonId == teacherPersonId);
        }

        if (period == MissingTopicsReportPeriod.Month)
        {
            if (year == null || month == null)
            {
                throw new Exception("Period=Month requires year and month.");
            }
            DateTime periodStart = new(year.Value, month.Value, 1);
            DateTime periodEnd =
                DateExtensions.Min(
                    periodStart.AddMonths(1).AddDays(-1),
                    createDate.Date);
            predicate = predicate.And(q => q.Date >= periodStart && q.Date <= periodEnd);
        }
        else
        {
            var cd = createDate.Date;
            predicate = period switch
            {
                MissingTopicsReportPeriod.TermOne => predicate.And(q => q.Date >= q.SchoolYearStartDate && q.Date <= q.FirstTermEndDate && q.Date <= cd),
                MissingTopicsReportPeriod.TermTwo => predicate.And(q => q.Date >= q.SecondTermStartDate && q.Date <= q.SchoolYearEndDate && q.Date <= cd),
                MissingTopicsReportPeriod.WholeYear => predicate.And(q => q.Date >= q.SchoolYearStartDate && q.Date <= q.SchoolYearEndDate && q.Date <= cd),
                _ => throw new Exception("Invalid period"),
            };
        }

        var result = await query
            .Where(predicate)
            .ToArrayAsync(ct);

        return result
            .GroupBy(e => new { e.ScheduleLessonId, e.Date, e.ClassBookId, e.ClassBookName, e.CurriculumId, e.CurriculumName, e.SchoolYearStartDate, e.FirstTermEndDate, e.SecondTermStartDate, e.SchoolYearEndDate, e.StudentPersonId })
            .Select(mt =>
                new GetItemsForAddVO(
                    mt.Key.Date,
                    mt.Key.ClassBookName,
                    mt.Key.CurriculumName,
                    mt.Select(x => x.TeacherPersonName).Distinct().ToArray())
                )
            .ToArray();
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int missingTopicsReportId,
        CancellationToken ct)
    {
        var reportItems = await (
            from mtri in this.DbContext.Set<MissingTopicsReportItem>()

            where mtri.SchoolYear == schoolYear &&
                  mtri.MissingTopicsReportId == missingTopicsReportId

            select new ExcelReportItemRow(
                mtri.Date,
                mtri.ClassBookName,
                mtri.CurriculumName,
                mtri.Teachers.Select(t => t.PersonName).ToArray())
        ).ToArrayAsync(ct);

        return await (
            from mtr in this.DbContext.Set<MissingTopicsReport>()

            join p in this.DbContext.Set<Person>() on mtr.TeacherPersonId equals p.PersonId
            into j0
            from p in j0.DefaultIfEmpty()

            where mtr.SchoolYear == schoolYear &&
                  mtr.InstId == instId &&
                  mtr.MissingTopicsReportId == missingTopicsReportId

            select new GetExcelDataVO(
                EnumUtils.GetEnumDescription(mtr.Period),
                GetMonthString(mtr.Year, mtr.Month),
                $"{p.FirstName} {p.MiddleName} {p.LastName}",
                mtr.CreateDate,
                reportItems)
         ).SingleAsync(ct);
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int missingTopicsReportId,
        CancellationToken ct)
    {
        return await (
            from mtr in this.DbContext.Set<MissingTopicsReport>()
            where mtr.SchoolYear == schoolYear &&
                mtr.MissingTopicsReportId == missingTopicsReportId
            select mtr.CreatedBySysUserId
        ).SingleAsync(ct);
    }

    private static string? GetMonthString(int? year, int? month)
    {
        if (year == null || month == null)
        {
            return null;
        }

        return new DateTime(year.Value, month.Value, 1).ToString("yyyy-MM");
    }
}
