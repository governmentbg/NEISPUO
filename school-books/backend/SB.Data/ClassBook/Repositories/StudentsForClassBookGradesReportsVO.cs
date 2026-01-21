namespace SB.Data;
using Microsoft.EntityFrameworkCore;

internal static partial class ClassBooksQueryHelper
{
    [Keyless]
    public record StudentsForClassBookGradesReportVO(
        int? BasicClassId,
        int ClassBookId,
        string ClassBookName,
        int PersonId);
}
