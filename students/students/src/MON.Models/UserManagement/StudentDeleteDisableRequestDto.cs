
using System.Text.Json.Serialization;

namespace MON.Models.UserManagement
{
    public class StudentDeleteDisableRequestDto : StudentRequestDto
    {
        [JsonPropertyName("positionID")]
        public int PositionId { get; set; }
    }
}
