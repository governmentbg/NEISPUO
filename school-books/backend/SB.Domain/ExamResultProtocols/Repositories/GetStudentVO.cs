namespace SB.Domain;
public partial interface IExamResultProtocolsQueryRepository
{
    public record GetStudentVO(
        int ClassId,
        int PersonId,
        string ClassName,
        string Name);
}
