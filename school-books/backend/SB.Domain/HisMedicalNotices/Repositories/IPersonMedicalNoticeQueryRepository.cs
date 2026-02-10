namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using SB.Common;

public partial interface IPersonMedicalNoticeQueryRepository
{
    Task<GetAllByAbsencesVO[]> GetAllByAbsencesAsync(
        int schoolYear,
        (int personId, DateTime absenceDate)[] absences,
        CancellationToken ct);

    Task<TableResultVO<GetAllByClassBookVO>> GetAllByClassBookAsync(
        int schoolYear,
        int classBookId,
        int? studentPersonId,
        DateTime? fromDate,
        DateTime? toDate,
        int? offset,
        int? limit,
        CancellationToken ct);
}
