namespace SB.Domain;

public partial interface IClassBooksAdminQueryRepository
{
    public record GetSchoolYearProgramVO(
        int ClassBookId,
        string? SchoolYearProgram);
}
