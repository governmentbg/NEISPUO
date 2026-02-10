namespace SB.Domain;
public partial interface IStateExamsAdmProtocolQueryRepository
{
    public record GetStudentVO(
        int ClassId,
        int PersonId,
        bool HasFirstMandatorySubject,
        GetStudentVOSubject? SecondMandatorySubject,
        GetStudentVOSubject[] AdditionalSubjects,
        GetStudentVOSubject[] QualificationSubjects);

    public record GetStudentVOSubject(
        int SubjectId,
        int SubjectTypeId);
}
