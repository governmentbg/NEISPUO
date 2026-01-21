namespace SB.Domain;
public partial interface IQualificationAcquisitionProtocolsQueryRepository
{
    public record GetStudentAllVO(
        int ClassId,
        int PersonId,
        string ClassName,
        string StudentName,
        bool ExamsPassed,
        string TheoryPoints,
        string PracticePoints,
        string AverageDecimalGrade);
}
