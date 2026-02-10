namespace MON.Models.Grid
{
    using System;

    public class DiplomaListInput : PagedListInput
    {
        public short? Year { get; set; }
        public int[] BasicDocuments { get; set; } = Array.Empty<int>();
        public int? PersonId { get; set; }
        public bool? IsValidation { get; set; }
        public bool? IsEqualization { get; set; }
        public string PersonalId { get; set; }
        public bool? FilterForSigning { get; set; }
        public bool? IsSigned { get; set; }
        public string PinFilter { get; set; }
        public string PinFilterOp { get; set; }
        public string NameFilter { get; set; }
        public string NameFilterOp { get; set; }
        public string SeriesFilter { get; set; }
        public string SeriesFilterOp { get; set; }
        public string FactoryNumbeFilter { get; set; }
        public string FactoryNumbeFilterOp { get; set; }
        public string InstitutionIdFilter { get; set; }
        public string InstitutionIdFilterOp { get; set; }
        public string BasicDocumentTypeFilter { get; set; }
        public string BasicDocumentTypeFilterOp { get; set; }
        public string SchoolYearFilter { get; set; }
        public string SchoolYearFilterOp { get; set; }
        public string RegionNameFilter { get; set; }
        public string RegionNameFilterOp { get; set; }
    }
}
