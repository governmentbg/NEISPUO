namespace SB.Domain;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentGradeResultSessionsVO(
        GradeResultType? FinalResult,
        GetStudentGradeResultSessionVO[] Sessions
    );

    public record GetStudentGradeResultSessionVO(
        string SubjectName,
        bool? Session1NoShow,
        int? Session1Grade,
        bool? Session2NoShow,
        int? Session2Grade,
        bool? Session3NoShow,
        int? Session3Grade
    );
}
