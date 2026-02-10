namespace SB.Domain;
public partial interface IRemarksQueryRepository
{
    public record GetSubjectIdAndSubjectTypeIdVO(
        int SubjectId,
        int SubjectTypeId);
}
