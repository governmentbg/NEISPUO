namespace SB.Domain;
public partial interface INvoExamDutyProtocolsQueryRepository
{
    public record GetStudentVO(
        int ClassId,
        int PersonId,
        string ClassName,
        string Name);
}
