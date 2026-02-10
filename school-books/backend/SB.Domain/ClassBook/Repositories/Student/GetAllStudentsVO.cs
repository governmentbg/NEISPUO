namespace SB.Domain;

public partial interface IStudentMedicalNoticesQueryRepository
{
    public record GetAllStudentsVO(
        int PersonId,
        string FirstName,
        string? MiddleName,
        string LastName);
}
