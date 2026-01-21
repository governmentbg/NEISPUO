namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
using static SB.Domain.IAttendancesQueryRepository;

internal class AttendancesQueryRepository : Repository, IAttendancesQueryRepository
{
    public AttendancesQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllForMonthVO[]> GetAllForMonthAsync(
        int schoolYear,
        int classBookId,
        int year,
        int month,
        CancellationToken ct)
    {
        DateTime monthStartDate = new DateTime(year, month, 1);
        DateTime monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);

        return await (
            from a in this.DbContext.Set<Attendance>()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.Date >= monthStartDate &&
                a.Date <= monthEndDate

            select new GetAllForMonthVO(
                a.AttendanceId,
                a.PersonId,
                a.Type,
                a.Date)
        ).ToArrayAsync(ct);
    }

    public async Task<GetAllForDateVO[]> GetAllForDateAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        CancellationToken ct)
    {
        return await (
            from a in this.DbContext.Set<Attendance>()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.Date == date

            select new GetAllForDateVO(
                a.AttendanceId,
                a.PersonId,
                a.Type,
                a.Date)
        ).ToArrayAsync(ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int attendanceId,
        CancellationToken ct)
    {
        return await (
            from a in this.DbContext.Set<Attendance>()

            join csu in this.DbContext.Set<SysUser>() on a.CreatedBySysUserId equals csu.SysUserId

            join cp in this.DbContext.Set<Person>() on csu.PersonId equals cp.PersonId

            join msu in this.DbContext.Set<SysUser>() on a.ModifiedBySysUserId equals msu.SysUserId

            join mp in this.DbContext.Set<Person>() on msu.PersonId equals mp.PersonId

            join ar in this.DbContext.Set<AbsenceReason>() on a.ExcusedReasonId equals ar.Id
            into g2
            from ar in g2.DefaultIfEmpty()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.AttendanceId == attendanceId

            select new GetVO(
                a.AttendanceId,
                a.PersonId,
                a.Date,
                a.Type,
                a.ExcusedReasonId,
                ar.Name,
                a.ExcusedReasonComment,
                a.CreateDate,
                a.CreatedBySysUserId,
                cp.FirstName,
                cp.MiddleName,
                cp.LastName,
                a.ModifyDate,
                a.ModifiedBySysUserId,
                mp.FirstName,
                mp.MiddleName,
                mp.LastName)
        ).SingleAsync(ct);
    }

    public async Task<GetSchoolYearLimitsVO> GetSchoolYearLimitsAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        return await (
            from cbsydi in this.DbContext.Set<ClassBookSchoolYearSettings>()

            where cbsydi.SchoolYear == schoolYear &&
                cbsydi.ClassBookId == classBookId

            select new GetSchoolYearLimitsVO(
                cbsydi.SchoolYearStartDateLimit,
                cbsydi.SchoolYearEndDateLimit)
        ).SingleAsync(ct);
    }
}
