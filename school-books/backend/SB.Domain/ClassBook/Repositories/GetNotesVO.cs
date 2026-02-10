namespace SB.Domain;

public partial interface IClassBookPrintRepository
{
    public record GetNotesVO(
        string[] Students,
        string? Description,
        bool IsForAllStudents);
}
