namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IStudentClassQueryRepository
{
    Task<StudentClass[]> FindAllByClassBookAndPersonIdAsync(int schoolYear, int classBookId, int personId, CancellationToken ct);
    Task<FindAllByClassBookVO[]> FindAllByClassBookAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<FindAllByClassBookVO[]> FindAllByClassBookAsync(int schoolYear, int classId, bool classIsLvl2, CancellationToken ct);
    Task<int[]> GetPersonIdsWithAbsencesAttendancesAsync(int schoolYear, int classBookId, CancellationToken ct);
}
