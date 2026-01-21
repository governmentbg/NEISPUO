using System.ComponentModel.DataAnnotations;

namespace MON.Models.StudentModels.Update
{
    public class StudentBasicDetailsUpdateModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Pin { get; set; }
        public int? PinType { get; set; }
        [Required]
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int? PermanentResidenceId { get; set; }
        public int? UsualResidenceId { get; set; }
    }
}