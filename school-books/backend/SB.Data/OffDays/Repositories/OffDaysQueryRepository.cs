namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IOffDaysQueryRepository;

internal class OffDaysQueryRepository : Repository, IOffDaysQueryRepository
{
    private readonly HashSet<GradeType> excludedGradeTypes = new() { GradeType.Term, GradeType.Final, GradeType.OtherClass, GradeType.OtherSchool, GradeType.OtherClassTerm, GradeType.OtherSchoolTerm }; 

    public OffDaysQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var res = await (
            from od in this.DbContext.Set<OffDay>()

            where od.SchoolYear == schoolYear &&
                od.InstId == instId

            orderby od.CreateDate

            select new GetAllVO(
                od.OffDayId,
                od.From,
                od.To,
                od.Description,
                od.IsForAllClasses,
                Array.Empty<string>(),
                Array.Empty<string>())
        ).ToTableResultAsync(offset, limit, ct);

        var ids = res.Result.Select(od => od.OffDayId).ToArray();

        var basicClassNames = (await (
            from odc in this.DbContext.Set<OffDayClass>()
            join bc in this.DbContext.Set<BasicClass>()
            on odc.BasicClassId equals bc.BasicClassId

            where odc.SchoolYear == schoolYear &&
                this.DbContext
                    .MakeIdsQuery(ids)
                    .Any(id => odc.OffDayId == id.Id)
            select new
            {
                odc.OffDayId,
                bc.Name
            }).ToArrayAsync(ct))
            .ToLookup(r => r.OffDayId, r => r.Name);

        var classBookNames = (await (
            from odcb in this.DbContext.Set<OffDayClassBook>()
            join cb in this.DbContext.Set<ClassBook>()
            on new { odcb.SchoolYear, odcb.ClassBookId }
            equals new { cb.SchoolYear, cb.ClassBookId }

            where odcb.SchoolYear == schoolYear &&
                this.DbContext
                    .MakeIdsQuery(ids)
                    .Any(id => odcb.OffDayId == id.Id)
            select new
            {
                odcb.OffDayId,
                cb.FullBookName
            }).ToArrayAsync(ct))
            .ToLookup(r => r.OffDayId, r => r.FullBookName);

        var resFixup = res with
            {
                Result = res.Result
                    .Select(r =>
                        r with
                        {
                            BasicClassNames = basicClassNames[r.OffDayId].ToArray(),
                            ClassBookNames = classBookNames[r.OffDayId].ToArray(),
                        })
                    .ToArray()
            };

        return resFixup;
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int offDayId,
        CancellationToken ct)
    {
        return await (
            from od in this.DbContext.Set<OffDay>()

            where od.SchoolYear == schoolYear &&
                od.OffDayId == offDayId

            select new GetVO(
                od.OffDayId,
                od.From,
                od.To,
                od.Description,
                od.IsForAllClasses,
                (from odc in this.DbContext.Set<OffDayClass>()
                    where odc.SchoolYear == od.SchoolYear &&
                        odc.OffDayId == od.OffDayId
                    select odc.BasicClassId
                    ).ToArray(),
                (from odcb in this.DbContext.Set<OffDayClassBook>()
                    where odcb.SchoolYear == od.SchoolYear &&
                        odcb.OffDayId == od.OffDayId
                    select odcb.ClassBookId
                    ).ToArray(),
                od.IsPgOffProgramDay)
        ).SingleAsync(ct);
    }

    public async Task<GetAllForRebuildVO[]> GetAllForRebuildAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        return await (
            from od in this.DbContext.Set<OffDay>()

            where od.SchoolYear == schoolYear &&
                od.InstId == instId

            select new GetAllForRebuildVO(
                od.OffDayId,
                od.Description,
                od.From,
                od.To,
                od.IsForAllClasses,
                od.IsPgOffProgramDay,
                (from odc in this.DbContext.Set<OffDayClass>()
                    where odc.SchoolYear == od.SchoolYear &&
                        odc.OffDayId == od.OffDayId
                    select odc.BasicClassId
                    ).ToArray(),
                (from odcb in this.DbContext.Set<OffDayClassBook>()
                    where odcb.SchoolYear == od.SchoolYear &&
                        odcb.OffDayId == od.OffDayId
                    select odcb.ClassBookId
                    ).ToArray())
            ).ToArrayAsync(ct);
    }

    public async Task<GetAllBasicClassNamesVO[]> GetAllBasicClassNamesAsync(CancellationToken ct)
    {
        return await this.DbContext.Set<BasicClass>()
            .Select(bc => new GetAllBasicClassNamesVO(bc.BasicClassId, bc.Name))
            .ToArrayAsync(ct);
    }

    public async Task<GetAllClassBookNamesVO[]> GetAllClassBookNamesAsync(int instId, CancellationToken ct)
    {
        return await this.DbContext.Set<ClassBook>()
            .Select(cb => new GetAllClassBookNamesVO(cb.ClassBookId, cb.FullBookName))
            .ToArrayAsync(ct);
    }

    public async Task<GetAllClassBooksVO[]> GetAllClassBooksAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId

            select new GetAllClassBooksVO(
                cb.SchoolYear,
                cb.ClassBookId,
                cb.BasicClassId)
        ).ToArrayAsync(ct);
    }

    public async Task<bool> ExistOffDayForAllClassesAsync(
        int schoolYear,
        int instId,
        int? exceptOffDayId,
        DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<OffDay>();
        if (exceptOffDayId != null)
        {
            predicate = predicate.And(od => od.OffDayId != exceptOffDayId);
        }

        return await this.DbContext.Set<OffDay>()
            .Where(predicate)
            .AnyAsync(
                od =>
                    od.SchoolYear == schoolYear &&
                    od.InstId == instId &&
                    od.IsForAllClasses &&
                    from <= od.To && to >= od.From, // date range overlap
                ct);
    }

    public async Task<bool> ExistOffDayForClassesAsync(
        int schoolYear,
        int instId,
        int? exceptOffDayId,
        DateTime from,
        DateTime to,
        int[] basicClassIds,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<OffDay>();
        if (exceptOffDayId != null)
        {
            predicate = predicate.And(od => od.OffDayId != exceptOffDayId);
        }

        return await (
            from od in this.DbContext.Set<OffDay>().Where(predicate)
            join odc in this.DbContext.Set<OffDayClass>()
            on new { od.SchoolYear, od.OffDayId }
            equals new { odc.SchoolYear, odc.OffDayId }

            where od.SchoolYear == schoolYear &&
                od.InstId == instId &&
                !od.IsForAllClasses &&
                @from <= od.To && to >= od.From && // date range overlap
                this.DbContext
                    .MakeIdsQuery(basicClassIds)
                    .Any(id => odc.BasicClassId == id.Id)

            select od
        ).AnyAsync(ct);
    }

    public async Task<bool> ExistOffDayForClassBooksAsync(
        int schoolYear,
        int instId,
        int? exceptOffDayId,
        DateTime from,
        DateTime to,
        int[] classBookIds,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<OffDay>();
        if (exceptOffDayId != null)
        {
            predicate = predicate.And(od => od.OffDayId != exceptOffDayId);
        }

        return await (
            from od in this.DbContext.Set<OffDay>().Where(predicate)
            join odcb in this.DbContext.Set<OffDayClassBook>()
            on new { od.SchoolYear, od.OffDayId }
            equals new { odcb.SchoolYear, odcb.OffDayId }

            where od.SchoolYear == schoolYear &&
                od.InstId == instId &&
                !od.IsForAllClasses &&
                @from <= od.To && to >= od.From && // date range overlap
                this.DbContext
                    .MakeIdsQuery(classBookIds)
                    .Any(id => odcb.ClassBookId == id.Id)

            select od
        ).AnyAsync(ct);
    }

    public async Task<bool> HasHoursInUseAsync(
        int schoolYear,
        int instId,
        DateTime from,
        DateTime to,
        bool isForAllClasses,
        int[] basicClassIds,
        int[] classBookIds,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<ClassBook>();
        if (!isForAllClasses && basicClassIds.Any())
        {
            predicate = predicate.And(a => this.DbContext.MakeIdsQuery(basicClassIds).Any(id => a.BasicClassId == id.Id));
        }
        if (!isForAllClasses && classBookIds.Any())
        {
            predicate = predicate.And(a => this.DbContext.MakeIdsQuery(classBookIds).Any(id => a.ClassBookId == id.Id));
        }

        return await (
            (
                from a in this.DbContext.Set<Absence>()
                join cb in this.DbContext.Set<ClassBook>().Where(predicate) on new { a.SchoolYear, a.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
                where cb.SchoolYear == schoolYear && cb.InstId == instId && a.Date >= @from && a.Date <= to
                select a.AbsenceId
            )
            .Union(
                from g in this.DbContext.Set<Grade>()
                join cb in this.DbContext.Set<ClassBook>().Where(predicate) on new { g.SchoolYear, g.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
                where cb.SchoolYear == schoolYear && cb.InstId == instId && g.Date >= @from && g.Date <= to && !this.excludedGradeTypes.Contains(g.Type)
                select g.GradeId
            )
            .Union(
                from t in this.DbContext.Set<Topic>()
                join cb in this.DbContext.Set<ClassBook>().Where(predicate) on new { t.SchoolYear, t.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
                where cb.SchoolYear == schoolYear && cb.InstId == instId && t.Date >= @from && t.Date <= to
                select t.TopicId
            )
            .Union(
                from tah in this.DbContext.Set<TeacherAbsenceHour>()
                join ta in this.DbContext.Set<TeacherAbsence>() on new { tah.SchoolYear, tah.TeacherAbsenceId } equals new { ta.SchoolYear, ta.TeacherAbsenceId }
                join sl in this.DbContext.Set<ScheduleLesson>() on new { tah.SchoolYear, tah.ScheduleLessonId } equals new { sl.SchoolYear, sl.ScheduleLessonId }
                join s in this.DbContext.Set<Schedule>() on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }
                join cb in this.DbContext.Set<ClassBook>().Where(predicate) on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
                where ta.SchoolYear == schoolYear && ta.InstId == instId && sl.Date >= @from && sl.Date <= to
                select tah.TeacherAbsenceId
            )
            .Union(
                from lsh in this.DbContext.Set<LectureScheduleHour>()
                join ls in this.DbContext.Set<LectureSchedule>() on new { lsh.SchoolYear, lsh.LectureScheduleId } equals new { ls.SchoolYear, ls.LectureScheduleId }
                join sl in this.DbContext.Set<ScheduleLesson>() on new { lsh.SchoolYear, lsh.ScheduleLessonId } equals new { sl.SchoolYear, sl.ScheduleLessonId }
                join s in this.DbContext.Set<Schedule>() on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }
                join cb in this.DbContext.Set<ClassBook>().Where(predicate) on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
                where ls.SchoolYear == schoolYear && ls.InstId == instId && sl.Date >= @from && sl.Date <= to
                select ls.LectureScheduleId
            )
        ).AnyAsync(ct);
    }
}
