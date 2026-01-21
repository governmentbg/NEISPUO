namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public partial interface IIndividualWorksQueryRepository
{
    public record GetVO(
        int IndividualWorkId,
        int PersonId,
        DateTime Date,
        string IndividualWorkActivity,
        [property: JsonIgnore] int CreatedBySysUserId)
    {
        public bool HasCreatorAccess { get; set; } // should be mutable
    }
}
