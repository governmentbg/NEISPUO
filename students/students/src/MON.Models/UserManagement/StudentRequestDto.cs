using System.Text.Json.Serialization;

namespace MON.Models.UserManagement
{
    public class StudentRequestDto
    {
        [JsonPropertyName("personID")]
        public int PersonId { get; set; }
    }
}
