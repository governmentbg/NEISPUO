namespace SB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IAbsencesByClassesReportsQueryRepository;

class AbsencesByClassesReportsQueryRepository : Repository, IAbsencesByClassesReportsQueryRepository
{
    public AbsencesByClassesReportsQueryRepository(UnitOfWork unitOfWork)
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
            from r in this.DbContext.Set<AbsencesByClassesReport>()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.SchoolYear,
                r.AbsencesByClassesReportId,
                EnumUtils.GetEnumDescription(r.Period),
                r.ClassBookNames,
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int absencesByClassesReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<AbsencesByClassesReport>()

            where r.SchoolYear == schoolYear && r.InstId == instId && r.AbsencesByClassesReportId == absencesByClassesReportId

            select new GetVO(
                r.SchoolYear,
                r.AbsencesByClassesReportId,
                r.Period,
                r.ClassBookNames,
                r.CreateDate)
         ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int absencesByClassesReportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from ri in this.DbContext.Set<AbsencesByClassesReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.AbsencesByClassesReportId == absencesByClassesReportId

            orderby ri.AbsencesByClassesReportItemId

            select new GetItemsVO(
                ri.ClassBookName,
                ri.StudentsCount,
                ri.ExcusedAbsencesCount,
                ri.ExcusedAbsencesCountAverage,
                ri.UnexcusedAbsencesCount,
                ri.UnexcusedAbsencesCountAverage,
                ri.IsTotal)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        ReportPeriod period,
        int[] classBookIds,
        DateTime createDate,
        CancellationToken ct)
    {
        // make sure we use only date part
        createDate = createDate.Date;

         var absencesPredicate = PredicateBuilder.True<Absence>();

        var classBookPredicate = PredicateBuilder.True<ClassBook>();
        classBookPredicate =
            classBookPredicate
                .And(c => c.SchoolYear == schoolYear && c.InstId == instId && c.IsValid)
                .And(cb => cb.BookType == ClassBookType.Book_I_III ||
                           cb.BookType == ClassBookType.Book_IV ||
                           cb.BookType == ClassBookType.Book_V_XII ||
                           cb.BookType == ClassBookType.Book_CSOP ||
                           cb.BookType == ClassBookType.Book_CDO);

        if (classBookIds.Any())
        {
            absencesPredicate = absencesPredicate.And(g => this.DbContext.MakeIdsQuery(classBookIds).Any(cid => g.ClassBookId == cid.Id));
            classBookPredicate = classBookPredicate.And(g => this.DbContext.MakeIdsQuery(classBookIds).Any(cid => g.ClassBookId == cid.Id));
        }

        var absences = await (
            from a in this.DbContext.Set<Absence>().Where(absencesPredicate)
            join cb in this.DbContext.Set<ClassBook>() on new { a.SchoolYear, a.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
            join cbsys in this.DbContext.Set<ClassBookSchoolYearSettings>() on new { cb.SchoolYear, cb.ClassBookId } equals new { cbsys.SchoolYear, cbsys.ClassBookId }

            where cb.InstId == instId &&
                  cb.SchoolYear == schoolYear &&
                  cb.IsValid

            select new
            {
                a.PersonId,
                cb.ClassBookId,
                a.Type,
                a.Date,

                // columns needed for filtering
                cbsys.SchoolYearStartDate,
                cbsys.FirstTermEndDate,
                cbsys.SecondTermStartDate,
                cbsys.SchoolYearEndDate
            }).ToArrayAsync(ct);

        var groupedAbsences = absences.Where(r => period switch
        {
            ReportPeriod.TermOne => r.Date >= r.SchoolYearStartDate && r.Date <= r.FirstTermEndDate && r.Date <= createDate,
            ReportPeriod.TermTwo => r.Date >= r.SecondTermStartDate && r.Date <= r.SchoolYearEndDate && r.Date <= createDate,
            ReportPeriod.WholeYear => r.Date >= r.SchoolYearStartDate && r.Date <= r.SchoolYearEndDate && r.Date <= createDate,
            _ => throw new Exception("Invalid period"),
        })
        .GroupBy(g => new
        {
            g.ClassBookId
        })
        .ToDictionary(
            g => g.Key,
            g => new
            {
                ExcusedAbsencesCount = g.Count(a => a.Type == AbsenceType.Excused),
                UnexcusedAbsencesCount = g.Count(a => a.Type == AbsenceType.Unexcused),
                LateAbsencesCount = g.Count(a => a.Type == AbsenceType.Late),
                IsTotal = false
            });

        var nonCombinedClassBooksItems = await (
            from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)
            join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, ClassId = (int?)cb.ClassId } equals new { cg.SchoolYear, ClassId = cg.ParentClassId }
            join sc in this.DbContext.Set<StudentClass>() on new { cg.SchoolYear, cg.InstitutionId, cg.ClassId } equals new { sc.SchoolYear, sc.InstitutionId, sc.ClassId }
            orderby cb.BasicClassId, cb.FullBookName

            select new
            {
                cb.ClassBookId,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId,
                sc.PersonId
            }).ToArrayAsync(ct);

        var combinedClassBooksItems = await (
            from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)
            join sc in this.DbContext.Set<StudentClass>() on new { cb.SchoolYear, cb.ClassId } equals new { sc.SchoolYear, sc.ClassId }
            orderby cb.BasicClassId, cb.FullBookName

            select new
            {
                cb.ClassBookId,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId,
                sc.PersonId
            }).ToArrayAsync(ct);

        var regularAbsenceItems = nonCombinedClassBooksItems
            .Union(combinedClassBooksItems)
            .GroupBy(r => new { r.ClassBookId, r.ClassBookName, r.BasicClassId })
            .Select(g =>
            {
                var absences = groupedAbsences.GetValueOrDefault(new { g.Key.ClassBookId });

                return new GetItemsForAddVO(
                    g.First().ClassBookName,
                    g.Count(),
                    absences?.ExcusedAbsencesCount ?? 0,
                    (absences?.UnexcusedAbsencesCount ?? 0) + (absences?.LateAbsencesCount * 0.5M ?? 0),
                    false
                );
            })
            .ToArray();

        return Enumerable.Concat(
                new[] {
                    new GetItemsForAddVO
                    (
                        "Всички",
                        regularAbsenceItems.Sum(i => i.StudentsCount),
                        regularAbsenceItems.Sum(i => i.ExcusedAbsencesCount),
                        regularAbsenceItems.Sum(i => i.UnexcusedAbsencesCount),
                        true
                    )
                },
                regularAbsenceItems.GroupBy(r => r.ClassBookName).SelectMany(i => i)
            ).ToArray();
    }
    

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int absencesByClassesReportId,
        CancellationToken ct)
    {
        return await (
            from abc in this.DbContext.Set<AbsencesByClassesReport>()
            where abc.SchoolYear == schoolYear &&
                abc.AbsencesByClassesReportId == absencesByClassesReportId
            select abc.CreatedBySysUserId
        ).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int absencesByClassesReportId,
        CancellationToken ct)
    {
        var items = await (
            from ri in this.DbContext.Set<AbsencesByClassesReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.AbsencesByClassesReportId == absencesByClassesReportId

            orderby ri.AbsencesByClassesReportItemId

            select new GetExcelDataVOItem
            (
                ri.ClassBookName,
                ri.StudentsCount,
                ri.ExcusedAbsencesCount,
                ri.ExcusedAbsencesCountAverage,
                ri.UnexcusedAbsencesCount,
                ri.UnexcusedAbsencesCountAverage,
                ri.IsTotal
            )
        ).ToArrayAsync(ct);

        return await (
            from r in this.DbContext.Set<AbsencesByClassesReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.AbsencesByClassesReportId == absencesByClassesReportId

            select new GetExcelDataVO
            (
                EnumUtils.GetEnumDescription(r.Period),
                r.ClassBookNames,
                r.CreateDate,
                items
            )
        ).SingleAsync(ct);
    }
}
