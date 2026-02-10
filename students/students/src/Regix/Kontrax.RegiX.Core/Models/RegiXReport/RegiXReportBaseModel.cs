using System.ComponentModel.DataAnnotations;

namespace Kontrax.RegiX.Core.TestStandard.Models
{
    public class RegiXReportBaseModel
    {
        public int Id { get; set; }

        [Display(Name = "Регистър")]
        public string RegisterName { get; set; }

        [Display(Name = "Наименование")]
        public virtual string ReportName { get; set; }

        [Display(Name = "Справката вече не се предоставя от RegiX")]
        public bool IsDeleted { get; set; }
    }
}
