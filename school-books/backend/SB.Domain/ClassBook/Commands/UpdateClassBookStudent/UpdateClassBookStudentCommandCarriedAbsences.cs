namespace SB.Domain;

public record UpdateClassBookStudentCommandCarriedAbsences
{
    public int? FirstTermExcusedCount { get; init; }
    public int? FirstTermUnexcusedCount { get; init; }
    public int? FirstTermLateCount { get; init; }
    public int? SecondTermExcusedCount { get; init; }
    public int? SecondTermUnexcusedCount { get; init; }
    public int? SecondTermLateCount { get; init; }
}
