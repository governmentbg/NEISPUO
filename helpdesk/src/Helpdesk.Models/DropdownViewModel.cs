namespace Helpdesk.Models
{
    using System.Text.Json.Serialization;

    public class DropdownViewModel
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        public int? RelatedObjectId { get; set; }
    }
}
