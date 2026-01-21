namespace SB.Common;

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TimeSpanHourAndMinutesConverter : JsonConverter<TimeSpan>
{
    private readonly string format = "hh\\:mm";
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TimeSpan.ParseExact(reader.GetString()!, this.format, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(format: this.format, CultureInfo.InvariantCulture));
    }
}
