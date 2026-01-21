namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IAbsencesQueryRepository
{
    public Task<GetAllForClassBookVO[]> GetAllForClassBookAsync(
        int schoolYear,
        int classBookId,
        int? curruculumId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct);

    public Task<GetAllForStudentAndTypeVO[]> GetAllForStudentAndTypeAsync(
        int schoolYear,
        int classBookId,
        int studentPersonId,
        AbsenceType type,
        int? curruculumId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct);

    public Task<GetAllForWeekVO[]> GetAllForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        CancellationToken ct);

    public Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int absenceId,
        CancellationToken ct);

    public Task<bool> ExistsVerifiedScheduleLessonAsync(
        int schoolYear,
        int[] scheduleLessonIds,
        CancellationToken ct);

    public Task<bool> ExistsVerifiedScheduleLessonForAbsencesAsync(
        int schoolYear,
        int[] absenceIds,
        CancellationToken ct);
}
