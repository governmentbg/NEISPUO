namespace Kontrax.RegiX.Core.TestStandard.Models.RegiXReport
{
    public partial class Dependency
    {
        public Dependency()
        {

        }

        public int ActivityId { get; set; }
        public int RegiXReportId { get; set; }
        public byte StepNumber { get; set; }
        public string LegalBasis { get; set; }

        public virtual RegiXReportModel RegiXReport { get; set; }
    }
}
