namespace Kontrax.RegiX.Core.TestStandard.Models.RegiXReport
{
    public partial class RegiXReportKey
    {
        public RegiXReportKey()
        { }

        public int RegiXReportId { get; set; }
        public string TypeCode { get; set; }
        public string ElementName { get; set; }

        public virtual RegiXReportModel RegiXReport { get; set; }
        public virtual KeyType KeyType { get; set; }
    }
}
