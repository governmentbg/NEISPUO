namespace MON.Shared.Extensions.Converters
{
    using Newtonsoft.Json;
    using System;

    public class DecimalJsonConverter : JsonConverter<decimal?>
    {
        public override void WriteJson(JsonWriter writer, decimal? value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override decimal? ReadJson(JsonReader reader, Type objectType, decimal? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            decimal? result = default;
            try
            {
                result = Convert.ToDecimal(reader.Value);
            }
            catch
            {
                string str = reader.Value.ToString();
                if(string.IsNullOrWhiteSpace(str)) return result;

                result = Convert.ToDecimal(reader.Value.ToString().Replace(".", ","));
            }

            return result;
        }
    }
}
