using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MON.Models.StudentModels.Update
{
    public class StudentAdditionalDetailsModel
    {
        [Required]
        public int PersonId { get; set; }

        public bool HasInternationalProtectionStatus { get; set; }

        public IEnumerable<InternationalProtectionModel> InternationalProtections { get; set; }
    }
}
