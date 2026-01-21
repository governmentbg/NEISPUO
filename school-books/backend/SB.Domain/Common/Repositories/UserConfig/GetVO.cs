namespace SB.Domain;

public partial interface IUserConfigQueryRepository
{
    public record GetVO(
        int SystemSchoolYear,
        int? TokenInstId,
        int? TokenInstSchoolYear);
}
