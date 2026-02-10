namespace SB.Domain;

public partial interface IClassBookPrintRepository
{
    public record GetGradeResultSessionsVO(
        int? ClassNumber,
        string FullName,
        bool IsTransferred,
        string SubjectName,
        bool? Session1NoShow,
        int? Session1Grade,
        bool? Session2NoShow,
        int? Session2Grade,
        bool? Session3NoShow,
        int? Session3Grade,
        GradeResultType? FinalResultType);
}
