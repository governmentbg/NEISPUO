namespace Domain.Models
{
    using System.Collections.Generic;

    public class SchoolYearResourceSupportDocumentModel
    {
        public SchoolYearResourceSupportDocumentModel()
        {
            resourceSupports = new List<ResourceSupportModel>();
        }
        public string schoolYear { get; set; }
        public string reportNumber { get; set; }
        public string reportDate { get; set; }
        public List<ResourceSupportModel> resourceSupports { get; set; }
    }

    public class ResourceSupportModel
    {
        public ResourceSupportModel()
        {
            specialists = new List<ResourceSupportSpecialistModel>();
        }

        public string type { get; set; }
        public string suspensionDate { get; set; }
        public string suspensionReason { get; set; }
        public List<ResourceSupportSpecialistModel> specialists { get; set; }
    }

    public class ResourceSupportSpecialistModel
    {
        public string type { get; set; }
        public string workplace { get; set; }
    }
}
