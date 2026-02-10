namespace SB.Domain;

public record AbsencesModel(
    AbsencesModelAbsence[] Absences
);

public record AbsencesModelAbsence(
    int? ClassNumber,
    string FullName,
    bool IsTransferred,
    int FirstTermExcusedCount,
    decimal FirstTermUnexcusedCount,
    int WholeYearExcusedCount,
    decimal WholeYearUnexcusedCount
);
