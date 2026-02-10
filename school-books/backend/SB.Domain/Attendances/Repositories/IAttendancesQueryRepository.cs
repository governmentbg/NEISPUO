namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IAttendancesQueryRepository
{
    public Task<GetAllForMonthVO[]> GetAllForMonthAsync(
        int schoolYear,
        int classBookId,
        int year,
        int month,
        CancellationToken ct);

    public Task<GetAllForDateVO[]> GetAllForDateAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        CancellationToken ct);

    public Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int attendanceId,
        CancellationToken ct);

    public Task<GetSchoolYearLimitsVO> GetSchoolYearLimitsAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);
}
