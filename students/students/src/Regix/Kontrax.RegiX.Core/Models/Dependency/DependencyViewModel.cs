using System.ComponentModel.DataAnnotations;

namespace Kontrax.RegiX.Core.TestStandard.Models
{
    public class DependencyViewModel
    {
        public RegiXReportBaseModel RegiXReport { get; set; }

        [Display(Name = "на основание")]
        public string LegalBasis { get; set; }
    }
}
