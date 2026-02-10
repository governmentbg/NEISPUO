namespace MON.Models.DocManagement
{
    public class DocManagementFreeDocListModel
    {
        public int BasicDocumentId { get; set; }
        public string BasicDocumentName { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string FreeDocNumbers { get; set; }
        public int? FreeDocCount { get; set; }
        public int? UsedDocCount { get; set; }
    }
}
