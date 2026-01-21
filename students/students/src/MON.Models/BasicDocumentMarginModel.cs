namespace MON.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class BasicDocumentMarginModel
    {
        public int Id { get; set; }
        public int Left1Margin { get; set; }
        public int Top1Margin { get; set; }
        public int Left2Margin { get; set; }
        public int Top2Margin { get; set; }
        public int? InstitutionId { get; set; }
        public int? RuoRegId { get; set; }
        public int BasicDocumentId { get; set; }
        public string ReportForm { get; set; }
    }
}
