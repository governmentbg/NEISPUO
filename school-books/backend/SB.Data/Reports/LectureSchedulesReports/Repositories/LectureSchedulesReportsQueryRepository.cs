namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.ILectureSchedulesReportsQueryRepository;

internal class LectureSchedulesReportsQueryRepository : Repository, ILectureSchedulesReportsQueryRepository
{
    public LectureSchedulesReportsQueryRepository(UnitOfWork unitOfWork)
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
            from r in this.DbContext.Set<LectureSchedulesReport>()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.LectureSchedulesReportId,
                EnumUtils.GetEnumDescription(r.Period),
                GetMonthString(r.Year, r.Month),
                r.TeacherPersonName,
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int lectureSchedulesReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<LectureSchedulesReport>()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.LectureSchedulesReportId == lectureSchedulesReportId

            select new GetVO(
                r.SchoolYear,
                r.LectureSchedulesReportId,
                r.Period,
                r.Year,
                r.Month,
                r.TeacherPersonName,
                r.CreateDate)
         ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int lectureSchedulesReportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var query =
            from ri in this.DbContext.Set<LectureSchedulesReportItem>()

            join ls in this.DbContext.Set<LectureSchedule>()
                on new { ri.SchoolYear, ri.LectureScheduleId }
                equals new { ls.SchoolYear, ls.LectureScheduleId }
            into j1 from ls in j1.DefaultIfEmpty()

            where ri.SchoolYear == schoolYear &&
                ri.LectureSchedulesReportId == lectureSchedulesReportId

            orderby
                ri.TeacherPersonName,
                ri.TeacherPersonId,
                ri.ClassBookName,
                ri.Date,
                ri.CurriculumName

            select new
            {
                ri.TeacherPersonId,
                ri.TeacherPersonName,
                ri.Date,
                ri.ClassBookName,
                ri.CurriculumName,
                LectureScheduleId = (int?)ls.LectureScheduleId,
                ri.OrderNumber,
                ri.OrderDate,
                ri.HoursTaken
            };

        int length = await query.CountAsync(ct);
        if (length == 0)
        {
            return TableResultVO.Empty<GetItemsVO>();
        }

        var totalRows = await
            query.GroupBy(lsri => new { lsri.TeacherPersonId, lsri.TeacherPersonName })
            .Select(g =>
                new
                {
                    g.Key.TeacherPersonId,
                    g.Key.TeacherPersonName,
                    RowCount = g.Count(),
                    TotalHoursTaken = g.Sum(ri => ri.HoursTaken)
                })
            .OrderBy(g => g.TeacherPersonName)
            .ThenBy(g => g.TeacherPersonId)
            .ToArrayAsync(ct);

        var teacherTotalHoursTaken =
            totalRows
            .ToDictionary(
                x => x.TeacherPersonId,
                x => x.TotalHoursTaken);

        // adjust the length with the number of total rows
        length += totalRows.Length;

        // calculate the indexes of the total rows
        int[] totalRowIndexes = new int[totalRows.Length];
        for (int i = 0; i < totalRows.Length; i++)
        {
            int lastGroupEndIndex = i == 0 ? -1 : totalRowIndexes[i - 1];
            totalRowIndexes[i] = lastGroupEndIndex + 1 + totalRows[i].RowCount;
        }

        offset = offset ?? 0;
        limit = limit ?? int.MaxValue;

        var result = await query
            .WithOffsetAndLimit(
                // adjust the offset with the number of total rows
                // before the start of the current page as that much
                // fewer rows were shown on the previous pages
                offset - totalRowIndexes.Count(i => i < offset),
                // adjust the limit with the number of total rows
                // in the current page as that much fewer rows will
                // be shown on the current page
                limit - totalRowIndexes.Count(i => offset <= i && i < offset + limit - 1))
            .Select(i =>
                new GetItemsVO(
                    new GetItemsVOInfoRow(
                        i.TeacherPersonId,
                        i.TeacherPersonName,
                        i.Date,
                        i.ClassBookName,
                        i.CurriculumName,
                        i.LectureScheduleId,
                        i.OrderNumber,
                        i.OrderDate,
                        i.HoursTaken),
                    null))
            .ToListAsync(ct);

        for (int i = 0; i < totalRowIndexes.Length; i++)
        {
            int totalRowIndex = totalRowIndexes[i];
            if (offset <= totalRowIndex && totalRowIndex < (offset + limit))
            {
                GetItemsVO totalRow = new(null, new(teacherTotalHoursTaken[totalRows[i].TeacherPersonId]));

                int totalRowPageIndex = totalRowIndex - offset.Value;

                // the total row is either somewhere within the page
                // or the last row of the page
                if (totalRowPageIndex == result.Count)
                {
                    result.Add(totalRow);
                }
                else
                {
                    result.Insert(totalRowPageIndex, totalRow);
                }
            }
        }

        return new TableResultVO<GetItemsVO>(result.ToArray(), length);
    }

    public async Task<string> GetTeacherNameAsync(
        int personId,
        CancellationToken ct)
    {
        return await (
            from p in this.DbContext.Set<Person>()
            where p.PersonId == personId
            select StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName)
        ).SingleAsync(ct);
    }

    public async Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        LectureSchedulesReportPeriod period,
        int? year,
        int? month,
        int? teacherPersonId,
        DateTime createDate,
        CancellationToken ct)
    {
        // make sure we use only date part
        createDate = createDate.Date;

        var query = (
            from cb in this.DbContext.Set<ClassBook>()
            join yi in this.DbContext.Set<ClassBookSchoolYearSettings>() on new { cb.SchoolYear, cb.ClassBookId } equals new { yi.SchoolYear, yi.ClassBookId }

            join t in this.DbContext.Set<Topic>() on new { cb.SchoolYear, cb.ClassBookId } equals new { t.SchoolYear, t.ClassBookId }

            join tt in this.DbContext.Set<TopicTeacher>() on new { t.SchoolYear, t.TopicId } equals new { tt.SchoolYear, tt.TopicId }
            join p in this.DbContext.Set<Person>() on tt.PersonId equals p.PersonId

            join sl in this.DbContext.Set<ScheduleLesson>() on new { t.SchoolYear, t.ScheduleLessonId } equals new { sl.SchoolYear, sl.ScheduleLessonId }
            join c in this.DbContext.Set<Curriculum>() on sl.CurriculumId equals c.CurriculumId
            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join lsh in this.DbContext.Set<LectureScheduleHour>() on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { lsh.SchoolYear, lsh.ScheduleLessonId }
            join ls in this.DbContext.Set<LectureSchedule>() on new { lsh.SchoolYear, lsh.LectureScheduleId } equals new { ls.SchoolYear, ls.LectureScheduleId }

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.IsValid &&
                tt.PersonId == ls.TeacherPersonId

            orderby sl.Date, cb.BasicClassId, cb.FullBookName, sl.HourNumber ascending, c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

            select new
            {
                sl.ScheduleLessonId,
                sl.Date,
                sl.HourNumber,
                sl.CurriculumId,
                s.SubjectName,
                SubjectTypeName = st.Name,
                cb.BasicClassId,
                cb.ClassBookId,
                ClassBookName = cb.FullBookName,
                TeacherPersonId = p.PersonId,
                TeacherPersonName = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                ls.LectureScheduleId,
                ls.OrderNumber,
                ls.OrderDate,

                // columns needed for filtering
                yi.SchoolYearStartDate,
                yi.FirstTermEndDate,
                yi.SecondTermStartDate,
                yi.SchoolYearEndDate
            });

        var predicate = PredicateBuilder.True(query);

        if (teacherPersonId != null)
        {
            predicate = predicate.And(r => r.TeacherPersonId == teacherPersonId);
        }

        if (period == LectureSchedulesReportPeriod.Month)
        {
            if (year == null || month == null)
            {
                throw new Exception("Period=Month requires year and month.");
            }

            DateTime periodStart = new(year.Value, month.Value, 1);
            DateTime periodEnd =
                DateExtensions.Min(
                    periodStart.AddMonths(1).AddDays(-1),
                    createDate);
            predicate = predicate.And(r => r.Date >= periodStart && r.Date <= periodEnd);
        }
        else
        {
            predicate = period switch
            {
                LectureSchedulesReportPeriod.TermOne => predicate.And(r => r.Date >= r.SchoolYearStartDate && r.Date <= r.FirstTermEndDate && r.Date <= createDate),
                LectureSchedulesReportPeriod.TermTwo => predicate.And(r => r.Date >= r.SecondTermStartDate && r.Date <= r.SchoolYearEndDate && r.Date <= createDate),
                LectureSchedulesReportPeriod.WholeYear => predicate.And(r => r.Date >= r.SchoolYearStartDate && r.Date <= r.SchoolYearEndDate && r.Date <= createDate),
                _ => throw new Exception("Invalid period"),
            };
        }

        var result = await query
            .Where(predicate)
            .OrderBy(lsh => lsh.Date)
            .ThenBy(lsh => lsh.BasicClassId)
            .ThenBy(lsh => lsh.ClassBookName)
            .ThenBy(lsh => lsh.HourNumber)
            .ToArrayAsync(ct);

        return result
            .GroupBy(lsh =>
                new
                {
                    lsh.Date,

                    lsh.ClassBookId,
                    lsh.ClassBookName,

                    lsh.CurriculumId,
                    lsh.SubjectName,
                    lsh.SubjectTypeName,

                    lsh.TeacherPersonId,
                    lsh.TeacherPersonName,

                    lsh.LectureScheduleId,
                    lsh.OrderNumber,
                    lsh.OrderDate
                })
            .Select(g =>
                new GetItemsForAddVO(
                    g.Key.TeacherPersonId,
                    g.Key.TeacherPersonName,
                    g.Key.Date,
                    g.Key.ClassBookId,
                    g.Key.ClassBookName,
                    g.Key.CurriculumId,
                    $"{g.Key.SubjectName} / {g.Key.SubjectTypeName}",
                    g.Key.LectureScheduleId,
                    g.Key.OrderNumber,
                    g.Key.OrderDate,
                    g.Count()))
            .ToArray();
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int lectureSchedulesReportId,
        CancellationToken ct)
    {
        var items = await (
            from ri in this.DbContext.Set<LectureSchedulesReportItem>()

            where ri.SchoolYear == schoolYear &&
                ri.LectureSchedulesReportId == lectureSchedulesReportId

            select new
            {
                ri.LectureSchedulesReportItemId,
                ri.TeacherPersonId,
                ri.TeacherPersonName,
                ri.Date,
                ri.ClassBookName,
                ri.CurriculumName,
                ri.OrderNumber,
                ri.OrderDate,
                ri.HoursTaken
            }
        ).ToArrayAsync(ct);

        var resultItems = items
            .GroupBy(lsh => new { lsh.TeacherPersonId, lsh.TeacherPersonName })
            .OrderBy(g => g.Key.TeacherPersonName)
            .ThenBy(g => g.Key.TeacherPersonId)
            .Select(gls =>
                new GetExcelDataVOTeacher(
                    gls.Select(i =>
                            new GetExcelDataVOTeacherHour(
                                i.TeacherPersonName,
                                i.Date,
                                i.ClassBookName,
                                i.CurriculumName,
                                i.OrderNumber,
                                i.OrderDate,
                                i.HoursTaken
                            ))
                        .OrderBy(lsh => lsh.ClassBookName)
                        .ThenBy(lsh => lsh.Date)
                        .ThenBy(lsh => lsh.CurriculumName)
                        .ToArray(),
                    gls.Sum(i => i.HoursTaken)
                )
            )
            .ToArray();

        return await (
            from r in this.DbContext.Set<LectureSchedulesReport>()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.LectureSchedulesReportId == lectureSchedulesReportId

            select new GetExcelDataVO(
                EnumUtils.GetEnumDescription(r.Period),
                GetMonthString(r.Year, r.Month),
                r.TeacherPersonName,
                r.CreateDate,
                resultItems)
         ).SingleAsync(ct);
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int lectureSchedulesReportId,
        CancellationToken ct)
    {
        return await (
            from mtr in this.DbContext.Set<LectureSchedulesReport>()
            where mtr.SchoolYear == schoolYear &&
                mtr.LectureSchedulesReportId == lectureSchedulesReportId
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
