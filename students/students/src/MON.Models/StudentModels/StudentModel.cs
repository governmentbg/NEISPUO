using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MON.Models.StudentModels
{
    public class StudentModel : StudentCreateModel
    {
        /// <summary>
        /// Единен номер в образователната система
        /// </summary>
        public string PublicEduNumber { get; set; }
        [Required]
        public int? CommuterType { get; set; }
        public bool? Active { get; set; }
        public string Nationality { get; set; }
        public bool InternationalProtectionStatus { get; set; }
        public bool? HasIndividualStudyPlan { get; set; }
        public bool? HasSuportiveEnvironment { get; set; }
        public string SupportiveEnvironment { get; set; }
        public string PreviousSchoolName { get; set; }
        public int NumberInClass { get; set; }
        public string PinType { get; set; }
        public bool? IsNotForSubmissionToNRA { get; set; }
        [Required]
        [JsonPropertyName("repeater")]
        public int Repeater { get; set; }
        public bool LivesWithFosterFamily { get; set; }
        public bool HasParentConsent { get; set; }
        public string GPName { get; set; }
        public string GPPhone { get; set; }
        public List<StudentRelativeModel> StudentRelativeDetails { get; set; }
        public List<ScholarshipViewModel> ScholarshipDetails { get; set; }
    }
}
