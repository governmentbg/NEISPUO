namespace SB.Domain;
public partial interface IQualificationExamResultProtocolsQueryRepository
{
    public record GetStudentVO(
        int ClassId,
        int PersonId,
        string ClassName,
        string Name);
}
