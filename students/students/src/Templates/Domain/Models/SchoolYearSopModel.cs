namespace Domain.Models
{
    using System.Collections.Generic;

    public class SchoolYearSopModel
    {
        public SchoolYearSopModel()
        {
            sops = new List<SopModel>();
        }
        public string schoolYear { get; set; }
        public List<SopModel> sops { get; set; }
    }

    public class SopModel
    {
        public string type { get; set; }
        public string subType { get; set; }
    }
}
