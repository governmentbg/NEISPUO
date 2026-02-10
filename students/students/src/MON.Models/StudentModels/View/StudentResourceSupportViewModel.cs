namespace MON.Models.StudentModels
{
    public class StudentResourceSupportViewModel : StudentResourceSupportModel
    {
        public string ResourceSupportTypeName { get; set; }
        public string SchoolYearName { get; set; }
        public bool IsLodFinalized { get; set; }
        public int? RelatedAdditionalPersonalDevelopmentSupportId { get; set; }
    }
}
