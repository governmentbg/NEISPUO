namespace SB.Data;

public partial interface IInstitutionStudentNomsRepository
{
    public record InstitutionStudentNomVO(InstitutionStudentNomVOStudent Id, string Name);

    public record InstitutionStudentNomVOStudent(int ClassId, int PersonId, string? FirstName, string? LastName);
}
