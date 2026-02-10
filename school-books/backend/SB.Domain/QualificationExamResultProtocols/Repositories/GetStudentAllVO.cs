namespace SB.Domain;
public partial interface IQualificationExamResultProtocolsQueryRepository
{
    public record GetStudentAllVO(
        int ClassId,
        int PersonId,
        string ClassName,
        string Name);
}
