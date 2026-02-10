using System.ComponentModel.DataAnnotations;

namespace MON.Models.StudentModels.Update
{
    public class StudentUpdateModel
    {
        [Required]
        public int Id { get; set; }
        public bool InternationalProtectionStatus { get; set; }
        [Required]
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool HasIndividualStudyPlan { get; set; }
        public int? PermanentResidenceId { get; set; }
        public int? UsualResidenceId { get; set; }
    }
}
