namespace MON.Models.StudentModels
{
    using System;
    using System.Collections.Generic;

    public class StudentAwardModel
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public int PersonId { get; set; }
        public int AwardTypeId { get; set; }
        public string Description { get; set; }
        public string OrderNumber { get; set; }
        public string AdditionalInformation { get; set; }
        public int? InstitutionId { get; set; }
        public int AwardCategoryId { get; set; }
        public int FounderId { get; set; }
        public int AwardReasonId { get; set; }
        public short? SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public IEnumerable<DocumentViewModel> Documents { get; set; }
        public string InstitutionName { get; set; }
        public string AwardTypeName { get; set; }
        public string AwardCategoryName { get; set; }
        public string AwardReasonName { get; set; }
        public string FounderName { get; set; }
    }
}
