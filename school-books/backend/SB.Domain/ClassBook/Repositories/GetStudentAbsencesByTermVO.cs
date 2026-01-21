namespace SB.Domain;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentAbsencesByTermVO(
        int ExcusedAbsencesCount,
        int UnexcusedAbsencesCount,
        int LateAbsencesCount,
        int CarriedExcusedCount,
        int CarriedUnexcusedCount,
        int CarriedLateCount
    );
}
