namespace SB.Domain;

public partial interface IClassBooksAdminQueryRepository
{
    public record GetStudentsVO(
        int PersonId,
        int? ClassNumber,
        bool IsTransferred,
        string StudentFullName,
        bool HasSpecialNeeds,
        bool HasGradelessSubjects,
        string? Activities,
        string? Speciality,
        GetStudentsVOParent[] Parents);

    public record GetStudentsVOParent(
        string Name,
        GetStudentsVOParentEmail[] Emails);

    public record GetStudentsVOParentEmail(
        string Name,
        bool IsRegistered);
}
