namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IAbsencesByStudentsReportsQueryRepository;

internal class AbsencesByStudentsReportsQueryRepository : Repository, IAbsencesByStudentsReportsQueryRepository
{
    public AbsencesByStudentsReportsQueryRepository(UnitOfWork unitOfWork)
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
            from r in this.DbContext.Set<AbsencesByStudentsReport>()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.SchoolYear,
                r.AbsencesByStudentsReportId,
                EnumUtils.GetEnumDescription(r.Period),
                r.ClassBookNames,
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int absencesByStudentsReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<AbsencesByStudentsReport>()

            where r.SchoolYear == schoolYear && r.InstId == instId && r.AbsencesByStudentsReportId == absencesByStudentsReportId

            select new GetVO(
                r.SchoolYear,
                r.AbsencesByStudentsReportId,
                r.Period,
                r.ClassBookNames,
                r.CreateDate)
         ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int absencesByStudentsReportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from ri in this.DbContext.Set<AbsencesByStudentsReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.AbsencesByStudentsReportId == absencesByStudentsReportId

            orderby ri.AbsencesByStudentsReportItemId

            select new GetItemsVO(
                ri.ClassBookName,
                ri.StudentName,
                ri.IsTransferred,
                ri.ExcusedAbsencesCount,
                ri.UnexcusedAbsencesCount + (ri.LateAbsencesCount * 0.5M),
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

        var classBookPredicate = PredicateBuilder.True<ClassBook>();
        var gradePredicate = PredicateBuilder.True<Grade>();

        classBookPredicate = classBookPredicate.And(c => c.SchoolYear == schoolYear && c.InstId == instId && c.IsValid);

        classBookPredicate = classBookPredicate.And(
            cb => cb.BookType == ClassBookType.Book_I_III ||
                cb.BookType == ClassBookType.Book_IV ||
                cb.BookType == ClassBookType.Book_V_XII ||
                cb.BookType == ClassBookType.Book_CSOP ||
                cb.BookType == ClassBookType.Book_CDO);

        if (classBookIds.Any())
        {
            classBookPredicate = classBookPredicate.And(g => this.DbContext.MakeIdsQuery(classBookIds).Any(cid => g.ClassBookId == cid.Id));
        }

        var absences = await (
            from a in this.DbContext.Set<Absence>()
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
            g.ClassBookId,
            g.PersonId
        })
        .ToDictionary(
            g => g.Key,
            g => new
            {
                ExcusedAbsencesCount = g.Count(a => a.Type == AbsenceType.Excused),
                UnexcusedAbsencesCount = g.Count(a => a.Type == AbsenceType.Unexcused),
                LateAbsencesCount = g.Count(a => a.Type == AbsenceType.Late),
                IsTotal = true
            });

        var nonCombinedClassBooksItems = await (
            from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)
            join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, ClassId = (int?)cb.ClassId } equals new { cg.SchoolYear, ClassId = cg.ParentClassId }
            join sc in this.DbContext.Set<StudentClass>() on new { cg.SchoolYear, cg.InstitutionId, cg.ClassId } equals new { sc.SchoolYear, sc.InstitutionId, sc.ClassId }
            join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId
            orderby cb.BasicClassId, cb.FullBookName, sc.ClassNumber, p.FirstName, p.MiddleName, p.LastName

            select new
            {
                cb.ClassBookId,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId,
                p.PersonId,
                sc.ClassNumber,
                StudentName = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                IsTransferred = sc.Status == StudentClassStatus.Transferred
            }).ToArrayAsync(ct);

        var combinedClassBooksItems = await (
            from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)
            join sc in this.DbContext.Set<StudentClass>() on new { cb.SchoolYear, cb.ClassId } equals new { sc.SchoolYear, sc.ClassId }
            join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId
            orderby cb.BasicClassId, cb.FullBookName, sc.ClassNumber, p.FirstName, p.MiddleName, p.LastName

            select new
            {
                cb.ClassBookId,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId,
                p.PersonId,
                sc.ClassNumber,
                StudentName = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                IsTransferred = sc.Status == StudentClassStatus.Transferred
            }).ToArrayAsync(ct);

        var regularAbsenceItems =
            nonCombinedClassBooksItems
            .Union(combinedClassBooksItems)
            .Select(r =>
            {
                var absences = groupedAbsences.GetValueOrDefault(new { r.ClassBookId, r.PersonId });

                return new GetItemsForAddVO(
                    r.ClassBookName,
                    r.StudentName,
                    r.IsTransferred,
                    absences?.ExcusedAbsencesCount ?? 0,
                    absences?.UnexcusedAbsencesCount ?? 0,
                    absences?.LateAbsencesCount ?? 0,
                    false);
            })
            .ToArray();

        return Enumerable.Concat(
                new[] {
                    new GetItemsForAddVO
                    (
                        "Всички",
                        "Всички",
                        false,
                        regularAbsenceItems.Sum(i => i.ExcusedAbsencesCount),
                        regularAbsenceItems.Sum(i => i.UnexcusedAbsencesCount),
                        regularAbsenceItems.Sum(i => i.LateAbsencesCount),
                        true
                    )
                },
                regularAbsenceItems.GroupBy(r => r.ClassBookName).Select(g =>
                    Enumerable.Concat(
                        new[] {
                            new GetItemsForAddVO
                            (
                                g.Key,
                                "Всички",
                                false,
                                g.Sum(i => i.ExcusedAbsencesCount),
                                g.Sum(i => i.UnexcusedAbsencesCount),
                                g.Sum(i => i.LateAbsencesCount),
                                true
                            )
                        },
                        g
                    )
                ).SelectMany(i => i)
            ).ToArray();
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int absencesByStudentsReportId,
        CancellationToken ct)
    {
        return await (
            from mtr in this.DbContext.Set<AbsencesByStudentsReport>()
            where mtr.SchoolYear == schoolYear &&
                mtr.AbsencesByStudentsReportId == absencesByStudentsReportId
            select mtr.CreatedBySysUserId
        ).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int absencesByStudentsReportId,
        CancellationToken ct)
    {
        var items = await (
            from ri in this.DbContext.Set<AbsencesByStudentsReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.AbsencesByStudentsReportId == absencesByStudentsReportId

            orderby ri.AbsencesByStudentsReportItemId

            select new GetExcelDataVOItem
            (
                ri.ClassBookName,
                ri.StudentName,
                ri.IsTransferred,
                ri.ExcusedAbsencesCount,
                ri.UnexcusedAbsencesCount + (ri.LateAbsencesCount * 0.5M),
                ri.IsTotal
            )
        ).ToArrayAsync(ct);

        return await (
            from r in this.DbContext.Set<AbsencesByStudentsReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.AbsencesByStudentsReportId == absencesByStudentsReportId

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
