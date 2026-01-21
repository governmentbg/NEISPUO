using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MON.Models.UserManagement
{
    public class EnrollmentStudentToClassCreateRequestDto : StudentRequestDto
    {
        [JsonPropertyName("curriculumIDs")]
        public IEnumerable<int> CurriculumIds { get; set; }
        [JsonPropertyName("basicClassID")]
        public int? BasicClassId { get; set; }
    }
}
