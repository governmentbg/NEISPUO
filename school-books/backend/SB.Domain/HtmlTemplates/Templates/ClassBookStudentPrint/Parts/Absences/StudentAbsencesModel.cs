namespace SB.Domain;

public record StudentAbsencesModel(
    int FirstTermExcusedCount,
    decimal FirstTermUnexcusedCount,
    int SecondTermExcusedCount,
    decimal SecondTermUnexcusedCount,
    int WholeYearExcusedCount,
    decimal WholeYearUnexcusedCount
);
