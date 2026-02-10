namespace SB.Domain;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentFirstGradeResultVO(
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade
    );
}
