namespace SB.Domain;

using SB.Common;
using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

public interface IShiftsQueryRepository
{
    public record GetAllVO(
        int ShiftId,
        string Name);
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int? offset,
        int? limit,
        CancellationToken ct);

    public record GetVO(
        int ShiftId,
        string Name,
        bool IsMultiday,
        bool IsAdhoc,
        GetVODay[] Days);
    public record GetVODay(
        int Day,
        GetVOHour[] Hours);
    public record GetVOHour(
        int HourNumber,
        TimeSpan StartTime,
        TimeSpan EndTime)
    {
        public int HourNumber { get; init; } = HourNumber;
        [JsonConverter(typeof(TimeSpanHourAndMinutesConverter))]public TimeSpan StartTime { get; init; } = StartTime;
        [JsonConverter(typeof(TimeSpanHourAndMinutesConverter))]public TimeSpan EndTime { get; init; } = EndTime;
    }
    Task<GetVO> GetAsync(
        int schoolYear,
        int shiftId,
        CancellationToken ct);

    public record GetShiftInfoVO(string Name, bool IsMultiday);
    Task<GetShiftInfoVO> GetShiftInfoAsync(
        int schoolYear,
        int instId,
        int shiftId,
        CancellationToken ct);

    Task<bool> HasLinkedEntitiesAsync(
        int schoolYear,
        int shiftId,
        CancellationToken ct);

    public record GetHoursUsedInScheduleVO(int Day, int HourNumber);
    Task<GetHoursUsedInScheduleVO[]> GetHoursUsedInScheduleAsync(
        int schoolYear,
        int instId,
        int shiftId,
        CancellationToken ct);
    Task<GetHoursUsedInScheduleVO[]> GetHoursUsedInScheduleAsync(
        int schoolYear,
        int scheduleId,
        CancellationToken ct);

    Task<string[]> GetShiftNamesByIdsAsync(
        int schoolYear,
        int instId,
        int[] shiftIds,
        CancellationToken ct);
}
