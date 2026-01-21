
using System.Text.Json.Serialization;

namespace MON.Models.UserManagement
{
    public class EnrollmentStudentToSchoolDeleteRequestDto : StudentRequestDto
    {
        [JsonPropertyName("institutionID")]
        public int InstitutionId { get; set; }
    }
}
