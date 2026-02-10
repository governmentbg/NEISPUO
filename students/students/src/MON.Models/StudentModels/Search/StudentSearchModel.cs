namespace MON.Models.StudentModels.Search
{
    public class StudentSearchModel
    {
        public string Pin { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool ExactMatch { get; set; }
        public string PublicEduNumber { get; set; }
        public string District { get; set; }
        public string Municipality { get; set; }
        public string School { get; set; }

        public int? PinType { get; set; }

        public bool? OnlyOwnInstitution { get; set; }


        public bool HasValue =>
            !string.IsNullOrWhiteSpace(Pin) ||
            !string.IsNullOrWhiteSpace(FirstName) ||
            !string.IsNullOrWhiteSpace(MiddleName) ||
            !string.IsNullOrWhiteSpace(LastName) ||
            !string.IsNullOrWhiteSpace(PublicEduNumber) ||
            !string.IsNullOrWhiteSpace(District) ||
            !string.IsNullOrWhiteSpace(Municipality) ||
            !string.IsNullOrWhiteSpace(School);
    }
}
