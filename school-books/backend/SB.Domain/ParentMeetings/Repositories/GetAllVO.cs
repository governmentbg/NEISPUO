namespace SB.Domain;

using SB.Common;
using System;
using System.Text.Json.Serialization;

public partial interface IParentMeetingsQueryRepository
{
    public record GetAllVO(
        int ParentMeetingId,
        DateTime Date,
        [property: JsonConverter(typeof(TimeSpanHourAndMinutesConverter))]TimeSpan StartTime,
        string? Location,
        string Title);
}
