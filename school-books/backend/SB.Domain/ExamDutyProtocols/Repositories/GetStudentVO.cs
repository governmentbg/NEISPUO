namespace SB.Domain;
public partial interface IExamDutyProtocolsQueryRepository
{
    public record GetStudentVO(
        int classId,
        int personId,
        string ClassName,
        string Name);
}
