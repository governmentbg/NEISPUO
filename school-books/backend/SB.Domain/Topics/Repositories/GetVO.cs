namespace SB.Domain;

using System.Text.Json.Serialization;

public partial interface IAdditionalActivitiesQueryRepository
{
    public record GetVO(
        int AdditionalActivityId,
        string Activity,
        [property: JsonIgnore] int CreatedBySysUserId);
}
