namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface ISchoolYearSettingsProvider
{
    Task<GetForClassBookVO> GetForClassBookAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int? basicClassId,
        int?[] childBasicClassIds,
        CancellationToken ct);
}
