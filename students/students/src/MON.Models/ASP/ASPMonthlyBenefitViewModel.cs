namespace MON.Models.ASP
{
    using System;

    public class ASPMonthlyBenefitViewModel
    {
        public int RecordsCount { get; set; }

        public string DateImported { get; set; }

        public short SchoolYear { get; set; }

        public string Month { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsActive { get; set; }

        // public IPagedList<ASPMonthlyBenefitModel> ImporteBenefitsDetails { get; set; }

        //public int TotalRecords { get; set; }
        public bool IsSigned { get; set; }
        public DateTime? SignedDate { get; set; }
        public int? AspSessionNo { get; set; }
        public int? MonSessionNo { get; set; }
        public int AspConfirmSessionCount { get; set; }
        public int MonConfirmSessionCount { get; set; }
    }
}
