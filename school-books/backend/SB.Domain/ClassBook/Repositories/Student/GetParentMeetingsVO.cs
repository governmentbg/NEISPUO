namespace SB.Domain;

using System;
using System.Text.Json.Serialization;
using SB.Common;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetParentMeetingsVO(
        DateTime Date,
        [property: JsonConverter(typeof(TimeSpanHourAndMinutesConverter))] TimeSpan StartTime,
        string? Location,
        string Title,
        string? Description);
}
