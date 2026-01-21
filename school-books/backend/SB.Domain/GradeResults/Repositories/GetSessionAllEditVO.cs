namespace SB.Domain;

public partial interface IGradeResultsQueryRepository
{
    public record GetSessionAllEditVO(
        int PersonId,
        int CurriculumId,
        int? ClassNumber,
        string FirstName,
        string? MiddleName,
        string LastName,
        bool IsTransferred,
        bool IsRemoved,
        string Curriculum,
        bool? Session1NoShow,
        int? Session1Grade,
        bool? Session2NoShow,
        int? Session2Grade,
        bool? Session3NoShow,
        int? Session3Grade);
}
