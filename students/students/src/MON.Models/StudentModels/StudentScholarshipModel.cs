using System;
using System.Collections.Generic;

namespace MON.Models.StudentModels
{
    public class StudentScholarshipModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public int ScholarshipTypeId { get; set; }
        public decimal AmountRate { get; set; }
        public string Description { get; set; }
        public int Periodicity { get; set; }
        public string OrderNumber { get; set; }
        public short? SchoolYear { get; set; }
        public int? InstitutionId { get; set; }
        public int ScholarshipFinancingOrganId { get; set; }
        public int ScholarshipAmountId { get; set; }

        public DateTime? OrderDate { get; set; }
        public DateTime StartingDateOfReceiving { get; set; }
        public DateTime? EndDateOfReceiving { get; set; }
        public DateTime? CommissionDate { get; set; }
        public string CommissionNumber { get; set; }
        public IEnumerable<DocumentViewModel> Documents { get; set; }

        public string Currency { get; set; }
        public string AltCurrency { get; set; }

        public decimal? AltAmountRate { get; set; }
    }
}
