namespace SB.Domain;

using Microsoft.EntityFrameworkCore;

public partial interface IClassBookStudentPrintRepository
{
    [Keyless]
    public record GetStudentAbsencesVO(
        int FirstTermExcusedCount,
        int FirstTermUnexcusedCount,
        int FirstTermLateCount,
        int SecondTermExcusedCount,
        int SecondTermUnexcusedCount,
        int SecondTermLateCount,
        int WholeYearExcusedCount,
        int WholeYearUnexcusedCount,
        int WholeYearLateCount,
        int FirstTermCarriedExcusedCount,
        int FirstTermCarriedUnexcusedCount,
        int FirstTermCarriedLateCount,
        int SecondTermCarriedExcusedCount,
        int SecondTermCarriedUnexcusedCount,
        int SecondTermCarriedLateCount,
        int WholeYearCarriedExcusedCount,
        int WholeYearCarriedUnexcusedCount,
        int WholeYearCarriedLateCount);
}
