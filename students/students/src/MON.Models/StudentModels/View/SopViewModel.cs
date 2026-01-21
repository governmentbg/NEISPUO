namespace MON.Models.StudentModels
{
    using System.Collections.Generic;

    public class SopViewModel
    {
        public int Id { get; set; }
        public int SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public IEnumerable<SopDetailsViewModel> SopDetails { get; set; }
        public IEnumerable<DocumentModel> Documents { get; set; }
        public bool IsLodFinalized { get; set; }
        public int? RelatedAdditionalPersonalDevelopmentSupportId { get; set; }
    }
}
