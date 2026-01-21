namespace MON.Models.Grid
{
    public class DocManagementReportListInput : DocManagementApplicationsListInput
    {
        public bool? IsDiplomaSigned { get; set; }
        public bool? HasDiplomaDocument { get; set; }
        public string SeriesFilter { get; set; }
        public string SeriesFilterOp { get; set; }
        public string FactoryNumberFilter { get; set; }
        public string FactoryNumberFilterOp { get; set; }
        public string BasicDocumentFilter { get; set; }
        public string BasicDocumentFilterOp { get; set; }
        public string SchoolYearFilter { get; set; }
        public string SchoolYearFilterOp { get; set; }
        public string PersonFullNameFilter { get; set; }
        public string PersonFullNameFilterOp { get; set; }
        public string PersonIdentifierFilter { get; set; }
        public string PersonIdentifierFilterOp { get; set; }
    }
}
