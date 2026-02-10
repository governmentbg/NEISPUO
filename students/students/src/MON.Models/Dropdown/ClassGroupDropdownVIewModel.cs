using System;
using System.Text.Json.Serialization;

namespace MON.Models
{
    public class ClassGroupDropdownViewModel : DropdownViewModel
    {
        [JsonPropertyName("isNotPresentForm")]
        public bool? IsNotPresentForm { get; set; }

        [JsonPropertyName("basicClassId")]
        public int? BasicClassId { get; set; }
        public int? ClassTypeId { get; set; }
        public int? ClassKindId { get; set; }

        public bool HasExternalSoProviderLimitation { get; set; }

        public int? SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public Guid Uid => Guid.NewGuid();

        public bool? IsLodFinalized { get; set; }
        public bool IsClosed { get; set; }
    }
}
