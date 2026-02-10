namespace SB.Domain;

public partial interface IStudentsAtRiskOfDroppingOutReportsQueryRepository
{
    public record GetItemsForAddVO
    {
        public required int PersonId { get; init; }
        public required string PersonalId { get; init; }
        public required string FirstName { get; init; }
        public required string MiddleName { get; init; }
        public required string LastName { get; init; }
        public required string ClassBookName { get; init; }
        public int? BasicClassId { get; init; }
        public decimal? UnexcusedAbsenceHoursCount { get; init; }
        public int? UnexcusedAbsenceDaysCount { get; init; }
    }
}
