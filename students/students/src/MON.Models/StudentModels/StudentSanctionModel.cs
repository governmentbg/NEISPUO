namespace MON.Models.StudentModels
{
    using System;
    using System.Collections.Generic;

    public class StudentSanctionModel
    {
        public int? Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string RuoOrderNumber { get; set; }
        public DateTime? RuoOrderDate { get; set; }
        public string CancelOrderNumber { get; set; }
        public DateTime? CancelOrderDate { get; set; }
        public string CancelReason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PersonId { get; set; }
        public int SanctionTypeId { get; set; }
        public string Description { get; set; }
        public int? InstitutionId { get; set; }
        public short? SchoolYear { get; set; }
        public IEnumerable<DocumentViewModel> Documents { get; set; }
        public byte? SourceType { get; set; }
        public string SchoolYearName { get; set; }
        public string InstitutionName { get; set; }
    }
}
