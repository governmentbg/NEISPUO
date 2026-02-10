namespace SB.Common;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

// if links need not be converted to <a href>
// use <span class="whitespace-pre-line">
public class HtmlTextWithLinksConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.MakeHtml(true, "text-primary underline"));
    }
}
