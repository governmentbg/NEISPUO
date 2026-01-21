namespace SB.Domain;
public partial interface IStateExamsAdmProtocolQueryRepository
{
    public record GetStudentAllVO(
        string ClassName,
        int ClassId,
        int PersonId,
        string StudentName,
        bool HasFirstMandatorySubject,
        string? SecondMandatorySubject,
        string[] AdditionalSubjects,
        string[] QualificationSubjects);
}
