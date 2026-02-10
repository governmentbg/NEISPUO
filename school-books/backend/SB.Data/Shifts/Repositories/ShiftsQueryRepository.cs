namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IShiftsQueryRepository;

internal class ShiftsQueryRepository : Repository, IShiftsQueryRepository
{
    public ShiftsQueryRepository(UnitOfWork unitOfWork)
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
        return await this.DbContext.Set<Shift>()
            .Where(s =>
                s.SchoolYear == schoolYear &&
                s.InstId == instId &&
                !s.IsAdhoc)
            .OrderByDescending(s => s.ShiftId)
            .Select(s => new GetAllVO(s.ShiftId, s.Name))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int shiftId,
        CancellationToken ct)
    {
        var shiftHours = await (
            from s in this.DbContext.Set<Shift>()

            join sh in this.DbContext.Set<ShiftHour>()
            on new { s.SchoolYear, s.ShiftId } equals new { sh.SchoolYear, sh.ShiftId }

            where s.SchoolYear == schoolYear &&
                s.ShiftId == shiftId

            select new
            {
                s.ShiftId,
                s.Name,
                s.IsMultiday,
                s.IsAdhoc,
                sh.Day,
                sh.HourNumber,
                sh.StartTime,
                sh.EndTime
            }
        ).ToListAsync(ct);

        if (!shiftHours.Any())
        {
            throw new DomainObjectNotFoundException(nameof(Shift));
        }

        var daysHours = shiftHours
            .GroupBy(sh => sh.Day)
            .Select(g => new GetVODay(
                g.Key,
                g.Select(h => new GetVOHour(
                    h.HourNumber,
                    h.StartTime,
                    h.EndTime))
                .ToArray()))
            .ToArray();

        return new GetVO(
            shiftHours[0].ShiftId,
            shiftHours[0].Name,
            shiftHours[0].IsMultiday,
            shiftHours[0].IsAdhoc,
            shiftHours[0].IsMultiday
                ? (from day in Schedule.ScheduleIsoDays
                    join dayHour in daysHours on day equals dayHour.Day into j1
                    from dayHour in j1.DefaultIfEmpty(new GetVODay(day, Array.Empty<GetVOHour>()))
                    select dayHour).ToArray()
                : daysHours.Where(dh => dh.Day == 1).ToArray()
            .ToArray());
    }

    public async Task<GetShiftInfoVO> GetShiftInfoAsync(
        int schoolYear,
        int instId,
        int shiftId,
        CancellationToken ct)
    {
        return await (
            from s in this.DbContext.Set<Shift>()

            where s.SchoolYear == schoolYear &&
                s.InstId == instId &&
                s.ShiftId == shiftId

            select new GetShiftInfoVO(
                s.Name,
                s.IsMultiday)
        ).SingleAsync(ct);
    }

    public async Task<bool> HasLinkedEntitiesAsync(
        int schoolYear,
        int shiftId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<Schedule>()
            .AnyAsync(
                s =>
                    s.SchoolYear == schoolYear &&
                    s.ShiftId == shiftId,
                ct);
    }

    public async Task<GetHoursUsedInScheduleVO[]> GetHoursUsedInScheduleAsync(
        int schoolYear,
        int instId,
        int shiftId,
        CancellationToken ct)
    {
        return (await (
            from cb in this.DbContext.Set<ClassBook>()
            join s in this.DbContext.Set<Schedule>() on new { cb.SchoolYear, cb.ClassBookId } equals new { s.SchoolYear, s.ClassBookId }
            join sh in this.DbContext.Set<ScheduleHour>() on new { s.SchoolYear, s.ScheduleId } equals new { sh.SchoolYear, sh.ScheduleId }
            where
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                s.ShiftId == shiftId &&
                cb.IsValid
            select new
            {
                sh.Day,
                sh.HourNumber
            }
        )
        .Distinct()
        .ToListAsync(ct))
        .Select(sh => new GetHoursUsedInScheduleVO(sh.Day, sh.HourNumber))
        .ToArray();
    }

    public async Task<GetHoursUsedInScheduleVO[]> GetHoursUsedInScheduleAsync(
        int schoolYear,
        int scheduleId,
        CancellationToken ct)
    {
        return (await (
            from sh in this.DbContext.Set<ScheduleHour>()
            where
                sh.SchoolYear == schoolYear &&
                sh.ScheduleId == scheduleId
            select new
            {
                sh.Day,
                sh.HourNumber
            }
        )
        .Distinct()
        .ToListAsync(ct))
        .Select(sh => new GetHoursUsedInScheduleVO(sh.Day, sh.HourNumber))
        .ToArray();
    }

    public async Task<string[]> GetShiftNamesByIdsAsync(int schoolYear, int instId, int[] shiftIds, CancellationToken ct)
    {
        return await (
            from s in this.DbContext.Set<Shift>()

            where s.SchoolYear == schoolYear &&
            s.InstId == instId &&
                this.DbContext.MakeIdsQuery(shiftIds)
                    .Any(id => s.ShiftId == id.Id)

            orderby s.Name

            select s.Name
        ).ToArrayAsync(ct);
    }
}
