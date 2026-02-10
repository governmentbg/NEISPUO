namespace SB.Domain;

using Microsoft.EntityFrameworkCore;

public partial interface IClassBookPrintRepository
{
    [Keyless]
    public record GetAbsencesVO(
        int PersonId,
        int FirstTermExcusedCount,
        int FirstTermUnexcusedCount,
        int FirstTermLateCount,
        int WholeYearExcusedCount,
        int WholeYearUnexcusedCount,
        int WholeYearLateCount,
        int FirstTermCarriedExcusedCount,
        int FirstTermCarriedUnexcusedCount,
        int FirstTermCarriedLateCount,
        int WholeYearCarriedExcusedCount,
        int WholeYearCarriedUnexcusedCount,
        int WholeYearCarriedLateCount);
}
