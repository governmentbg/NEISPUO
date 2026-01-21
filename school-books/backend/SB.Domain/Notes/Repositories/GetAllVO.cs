namespace SB.Domain;

public partial interface INotesQueryRepository
{
    public record GetAllVO(
        int NoteId,
        string Students,
        string? Description,
        bool IsForAllStudents);
}
