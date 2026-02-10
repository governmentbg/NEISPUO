namespace SB.Domain;

public partial interface ISessionStudentsReportsQueryRepository
{
    public record GetItemsVO(
        string StudentNames,
        string ClassBookName,
        bool IsTransferred,
        string? Session1CurriculumNames,
        string? Session2CurriculumNames,
        string? Session3CurriculumNames);
}
