namespace SB.Domain;
public partial interface IExamDutyProtocolsQueryRepository
{
    public record GetStudentAllVO(
        int ClassId,
        int PersonId,
        string ClassName,
        string Name);
}
