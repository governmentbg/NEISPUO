namespace SB.Domain;

using System.Text.Json.Serialization;

public partial interface IAdditionalActivitiesQueryRepository
{
    public record GetAllAdditionalActivitiesForWeekVO(
        int AdditionalActivityId,
        string Activity,
        [property: JsonIgnore] int CreatedBySysUserId)
    {
        public bool HasCreatorAccess { get; set; } // should be mutable
    }
}
