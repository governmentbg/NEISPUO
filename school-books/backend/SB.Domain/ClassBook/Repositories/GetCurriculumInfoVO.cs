namespace SB.Domain;

public partial interface IClassBookPrintRepository
{
    public record GetCurriculumInfoVO(
        int CurriculumId,
        string SubjectName,
        string SubjectTypeName);
}
