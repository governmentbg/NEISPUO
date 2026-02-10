namespace SB.Domain;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetGradeResultVO(
        GradeResultType? InitialResultType,
        GradeResultType? FinalResultType,
        GetGradeResultVOSession[] GradeResultSessions);

    public record GetGradeResultVOSession(
        string Curriculum,
        bool? Session1NoShow,
        string? Session1ResultText,
        bool? Session2NoShow,
        string? Session2ResultText,
        bool? Session3NoShow,
        string? Session3ResultText);
}
