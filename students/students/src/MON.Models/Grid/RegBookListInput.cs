namespace MON.Models.Grid
{
    using MON.Shared.Enums;

    public class RegBookListInput : PagedListInput
    {
        public short? Year { get; set; }
        public int? BasicDocumentId { get; set; }
        public int? InstitutionId { get; set; }
        public RegBookTypeEnum RegBookType { get; set; }
        public int? PersonId { get; set; }
        public bool? IsValidation { get; set; }
        public string PersonalId { get; set; }
        public bool? FilterForSigning { get; set; }
        public bool? IsSigned { get; set; }
        public string PinFilter { get; set; }
        public string PinFilterOp { get; set; }
        public string PersonNameFilter { get; set; }
        public string PersonNameFilterOp { get; set; }
        public string SeriesFilter { get; set; }
        public string SeriesFilterOp { get; set; }
        public string RegistrationNumberTotalFilter { get; set; }
        public string RegistrationNumberTotalFilterOp { get; set; }
        public string RegistrationNumberYearFilter { get; set; }
        public string RegistrationNumberYearFilterOp { get; set; }
        public string FactoryNumberFilter { get; set; }
        public string FactoryNumberFilterOp { get; set; }
        public string InstitutionIdFilter { get; set; }
        public string InstitutionIdFilterOp { get; set; }
        public string BasicDocumentTypeFilter { get; set; }
        public string BasicDocumentTypeFilterOp { get; set; }
        public string SchoolYearFilter { get; set; }
        public string SchoolYearFilterOp { get; set; }
    }
}
