namespace SB.Domain;
public partial interface IQualificationAcquisitionProtocolsQueryRepository
{
    public record GetStudentVO(
        int ClassId,
        int PersonId,
        bool ExamsPassed,
        decimal? TheoryPoints,
        decimal? PracticePoints,
        decimal? AverageDecimalGrade);
}
