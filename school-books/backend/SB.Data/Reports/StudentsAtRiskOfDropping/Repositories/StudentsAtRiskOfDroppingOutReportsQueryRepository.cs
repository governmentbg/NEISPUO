namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
using static SB.Domain.IStudentsAtRiskOfDroppingOutReportsQueryRepository;

internal class StudentsAtRiskOfDroppingOutReportsQueryRepository : Repository, IStudentsAtRiskOfDroppingOutReportsQueryRepository
{
    private const int MaxAllowedUnexcusedAbsenceDays = 3;
    private const decimal MaxAllowedUnexcusedAbsenceHours = 5.0M;

    public StudentsAtRiskOfDroppingOutReportsQueryRepository(UnitOfWork unitOfWork)
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
            from r in this.DbContext.Set<StudentsAtRiskOfDroppingOutReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.StudentsAtRiskOfDroppingOutReportId,
                r.ReportDate,
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        DateTime reportDate,
        CancellationToken ct)
    {
        var unexcusedAttendance = await (
            from a in this.DbContext.Set<Attendance>()

            join p in this.DbContext.Set<Person>() on a.PersonId equals p.PersonId
            join cb in this.DbContext.Set<ClassBook>() on a.ClassBookId equals cb.ClassBookId

            where cb.InstId == instId &&
                  cb.SchoolYear == schoolYear &&
                  cb.IsValid &&
                  a.Type == AttendanceType.UnexcusedAbsence &&
                  a.Date.Date <= reportDate.Date

            select new
            {
                p.PersonId,
                p.PersonalId,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId
            })
            .GroupBy(g => new
            {
                g.PersonId,
                g.PersonalId,
                g.FirstName,
                g.MiddleName,
                g.LastName,
                g.ClassBookName,
                g.BasicClassId
            })
            .Select(g =>
                new GetItemsForAddVO
                {
                    PersonId = g.Key.PersonId,
                    PersonalId = g.Key.PersonalId,
                    FirstName = g.Key.FirstName,
                    MiddleName = g.Key.MiddleName,
                    LastName = g.Key.LastName,
                    ClassBookName = g.Key.ClassBookName,
                    BasicClassId = g.Key.BasicClassId,
                    UnexcusedAbsenceHoursCount = null,
                    UnexcusedAbsenceDaysCount = g.Count()
                })
            .Where(item => item.UnexcusedAbsenceDaysCount > MaxAllowedUnexcusedAbsenceDays)
            .OrderBy(i => i.BasicClassId)
            .ThenBy(i => i.ClassBookName)
            .ThenBy(i => i.FirstName)
            .ThenBy(i => i.MiddleName)
            .ThenBy(i => i.LastName)
            .ToArrayAsync(ct);

        var unexcusedAbsences = await (
            from a in this.DbContext.Set<Absence>()

            join p in this.DbContext.Set<Person>() on a.PersonId equals p.PersonId
            join cb in this.DbContext.Set<ClassBook>() on a.ClassBookId equals cb.ClassBookId

            where cb.InstId == instId &&
                  cb.SchoolYear == schoolYear &&
                  cb.IsValid &&
                  (a.Type == AbsenceType.Unexcused || a.Type == AbsenceType.DplrAbsence || a.Type == AbsenceType.Late) &&
                  a.Date.Date <= reportDate.Date

            select new
            {
                p.PersonId,
                p.PersonalId,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId,
                a.Type
            })
            .GroupBy(g => new
            {
                g.PersonId,
                g.PersonalId,
                g.FirstName,
                g.MiddleName,
                g.LastName,
                g.ClassBookName,
                g.BasicClassId
            })
            .Select(g =>
                new GetItemsForAddVO
                {
                    PersonId = g.Key.PersonId,
                    PersonalId = g.Key.PersonalId,
                    FirstName = g.Key.FirstName,
                    MiddleName = g.Key.MiddleName,
                    LastName = g.Key.LastName,
                    ClassBookName = g.Key.ClassBookName,
                    BasicClassId = g.Key.BasicClassId,
                    UnexcusedAbsenceHoursCount =
                        g.Sum(x => x.Type == AbsenceType.Unexcused || x.Type == AbsenceType.DplrAbsence ? 1M : 0.5M),
                    UnexcusedAbsenceDaysCount = null
                })
            .Where(item => item.UnexcusedAbsenceHoursCount > MaxAllowedUnexcusedAbsenceHours)
            .OrderBy(i => i.BasicClassId)
            .ThenBy(i => i.ClassBookName)
            .ThenBy(i => i.FirstName)
            .ThenBy(i => i.MiddleName)
            .ThenBy(i => i.LastName)
            .ToArrayAsync(ct);

        return unexcusedAttendance
            .Union(unexcusedAbsences)
            .ToArray();
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int studentsAtRiskOfDroppingOutReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<StudentsAtRiskOfDroppingOutReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.StudentsAtRiskOfDroppingOutReportId == studentsAtRiskOfDroppingOutReportId

            select new GetVO(
                r.StudentsAtRiskOfDroppingOutReportId,
                r.ReportDate,
                r.CreateDate)
        ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int studentsAtRiskOfDroppingOutReportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from ri in this.DbContext.Set<StudentsAtRiskOfDroppingOutReportItem>()
            
            where ri.SchoolYear == schoolYear && ri.StudentsAtRiskOfDroppingOutReportId == studentsAtRiskOfDroppingOutReportId
            
            orderby ri.StudentsAtRiskOfDroppingOutReportItemId
            
            select new GetItemsVO(
                ri.PersonalId,
                ri.FirstName,
                ri.MiddleName,
                ri.LastName,
                ri.ClassBookName,
                ri.UnexcusedAbsenceHoursCount,
                ri.UnexcusedAbsenceDaysCount)
            ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int studentsAtRiskOfDroppingOutReportId,
        CancellationToken ct)
    {
        return await (
            from srod in this.DbContext.Set<StudentsAtRiskOfDroppingOutReport>()
            where srod.SchoolYear == schoolYear &&
                  srod.InstId == instId &&
                  srod.StudentsAtRiskOfDroppingOutReportId == studentsAtRiskOfDroppingOutReportId
            select srod.CreatedBySysUserId
        ).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int studentsAtRiskOfDroppingOutReportId,
        CancellationToken ct)
    {
        var resultItems = await (
            from ri in this.DbContext.Set<StudentsAtRiskOfDroppingOutReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.StudentsAtRiskOfDroppingOutReportId == studentsAtRiskOfDroppingOutReportId

            orderby ri.StudentsAtRiskOfDroppingOutReportItemId

            select new GetExcelDataVOItem(
                ri.PersonalId,
                ri.FirstName,
                ri.MiddleName,
                ri.LastName,
                ri.ClassBookName,
                ri.UnexcusedAbsenceHoursCount,
                ri.UnexcusedAbsenceDaysCount)
            ).ToArrayAsync(ct);

        return await (
            from r in this.DbContext.Set<StudentsAtRiskOfDroppingOutReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.StudentsAtRiskOfDroppingOutReportId == studentsAtRiskOfDroppingOutReportId

            select new GetExcelDataVO(
                r.ReportDate,
                r.CreateDate,
                resultItems)
        ).SingleAsync(ct);
    }
}
