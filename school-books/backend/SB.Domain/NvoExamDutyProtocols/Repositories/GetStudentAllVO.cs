namespace SB.Domain;
public partial interface INvoExamDutyProtocolsQueryRepository
{
    public record GetStudentAllVO(
        int ClassId,
        int PersonId,
        string ClassName,
        string Name);
}
