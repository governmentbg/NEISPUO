namespace SB.Domain;

public partial interface ISessionStudentsReportsQueryRepository
{
    public record GetItemsForAddVO
    {
        public required string StudentNames { get; init; }
        public required string ClassBookName { get; init; }
        public required bool IsTransferred { get; init; }
        public string? Session1CurriculumNames { get; init; }
        public string? Session2CurriculumNames { get; init; }
        public string? Session3CurriculumNames { get; init; }
    }
}
