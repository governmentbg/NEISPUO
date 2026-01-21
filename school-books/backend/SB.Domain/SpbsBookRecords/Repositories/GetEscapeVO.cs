namespace SB.Domain;

using SB.Common;
using System;
using System.Text.Json.Serialization;

public partial interface ISpbsBookRecordsQueryRepository
{
    public record GetEscapeVO(
        int OrderNum,
        DateTime EscapeDate,
        [property: JsonConverter(typeof(TimeSpanHourAndMinutesConverter))] TimeSpan EscapeTime,
        DateTime PoliceNotificationDate,
        [property: JsonConverter(typeof(TimeSpanHourAndMinutesConverter))] TimeSpan PoliceNotificationTime,
        string PoliceLetterNumber,
        DateTime PoliceLetterDate,
        DateTime? ReturnDate)
    {
        public DateTime EscapeDateTime => this.EscapeDate + this.EscapeTime;

        public DateTime PoliceNotificationDateTime => this.PoliceNotificationDate + this.PoliceNotificationTime;

        public int? EscapeDays => this.ReturnDate.HasValue ?
            (this.ReturnDate.Value - this.EscapeDate).Days + 1 :
            null;
    }
}
