using MON.Shared.Interfaces;
using System.Text.Json.Serialization;

namespace MON.Models
{
    public class DropdownViewModel: IValidable
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("clearName")]
        public string ClearName { get; set; }

        [JsonPropertyName("relatedObjectId")]
        public int RelatedObjectId { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("isSelectable")]
        public bool Disabled { get; set; }

        [JsonPropertyName("isAvailable")]
        public bool IsAvailable { get; set; }
     
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }
    }
}
