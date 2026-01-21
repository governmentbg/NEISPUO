namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IAbsencesDplrQueryRepository
{
    public Task<GetAllDplrForClassBookVO[]> GetAllDplrForClassBookAsync(
        int schoolYear,
        int classBookId,
        AbsenceType type,
        int? curruculumId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct);

    public Task<GetAllDplrForStudentAndTypeVO[]> GetAllDplrForStudentAndTypeAsync(
        int schoolYear,
        int classBookId,
        int studentPersonId,
        AbsenceType type,
        int? curruculumId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct);

    public Task<GetAllDplrForWeekVO[]> GetAllDplrForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        AbsenceType type,
        CancellationToken ct);
}
