namespace Domain.Models
{
    using System.Collections.Generic;

    public class SchoolYearAdditionalPersonDevelopmentSupportModel
    {
        public SchoolYearAdditionalPersonDevelopmentSupportModel()
        {
            items = new List<PersonDevelopmentSupportModel>();
        }

        public string schoolYear { get; set; }
        public string period { get; set; }
        public string personDescription { get; set; }
        public string finalSchoolYear { get; set; }
        public string orderNumber {  get; set; }
        public string orderDate {  get; set; }
        public List<PersonDevelopmentSupportModel> items { get; set; }
    }

    public class SchoolYearCommonPersonDevelopmentSupportModel
    {
        public SchoolYearCommonPersonDevelopmentSupportModel()
        {
            items = new List<PersonDevelopmentSupportModel>();
        }

        public string schoolYear { get; set; }

        public List<PersonDevelopmentSupportModel> items { get; set; }
    }

    public class PersonDevelopmentSupportModel
    {
        public string type { get; set; }
        public string description { get; set; }
        public string suspensionDate { get; set; }
        public string suspensionReason { get; set; }
    }
}
