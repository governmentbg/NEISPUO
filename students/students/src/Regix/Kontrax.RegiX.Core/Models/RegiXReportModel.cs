using Kontrax.RegiX.Core.TestStandard.Models.RegiXReport;
using System.Collections.Generic;

namespace Kontrax.RegiX.Core.TestStandard.Models
{
    public class RegiXReportModel
    {
        public int Id { get; set; }
        public string ProviderName { get; set; }
        public string RegisterName { get; set; }
        public string ReportName { get; set; }
        public string AdapterSubdirectory { get; set; }
        public string OperationName { get; set; }
        public string RequestXsd { get; set; }
        public string ResponseXsd { get; set; }
        public string Operation { get; set; }
        public bool IsDeleted { get; set; }

        public string FullName
        {
            get { return !string.IsNullOrEmpty(RegisterName) ? $"{ReportName} ({RegisterName})" : ReportName; }
        }

        public virtual ICollection<RegiXReportKey> RegiXReportKeys { get; set; }
    }
}
