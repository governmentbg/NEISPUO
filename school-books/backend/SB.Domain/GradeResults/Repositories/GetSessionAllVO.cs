namespace SB.Domain;

public partial interface IGradeResultsQueryRepository
{
    public record GetSessionAllVO(
        int PersonId,
        int? ClassNumber,
        string FirstName,
        string? MiddleName,
        string LastName,
        bool IsTransferred,
        bool IsRemoved,
        string Curriculum,
        bool? Session1NoShow,
        string? Session1ResultText,
        bool? Session2NoShow,
        string? Session2ResultText,
        bool? Session3NoShow,
        string? Session3ResultText);
}
