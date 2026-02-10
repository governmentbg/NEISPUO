namespace SB.Domain;
public partial interface IExamResultProtocolsQueryRepository
{
    public record GetStudentAllVO(
        int ClassId,
        int PersonId,
        string ClassName,
        string Name);
}
