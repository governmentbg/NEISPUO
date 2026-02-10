
using System.Text.Json.Serialization;

namespace MON.Models.UserManagement
{
    public class EnrollmentStudentToClassDeleteRequestDto : StudentRequestDto
    {
        [JsonPropertyName("curriculumIDs")]
        public int[] CurriculumIds { get; set; }
    }
}
