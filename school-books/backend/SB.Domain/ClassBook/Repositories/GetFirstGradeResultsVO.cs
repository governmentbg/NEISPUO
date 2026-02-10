namespace SB.Domain;

public partial interface IClassBookPrintRepository
{
    public record GetFirstGradeResultsVO(
        int? ClassNumber,
        string FullName,
        bool IsTransferred,
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade);
}
