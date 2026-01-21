namespace MON.Models.Refugee
{
    using MON.Shared.Enums;

    public class ApplicationChildViewModel : ApplicationChildModel
    {
        public string PersonalIdTypeName { get; set; }
        public string Nationality { get; set; }
        public string GenderName { get; set; }
        public string Town { get; set; }
        public string Institution { get; set; }
        public string LastInstitutionCountryName { get; set; }
        public string LastBasicClassName { get; set; }
    }
}
