namespace MON.Models.StudentModels
{
    public class StudentEnvironmentCharacteristicsModel
    {
        public int PersonId { get; set; }
        public string GPPhone { get; set; }
        public string GPName { get; set; }
        public bool HasParentConsent { get; set; }
        public bool LivesWithFosterFamily { get; set; }
        public bool RepresentedByTheMajor { get; set; }
        public decimal FamilyEducationWeight { get; set; }
        public decimal FamilyWorkStatusWeight { get; set; }
        public DropdownViewModel NativeLanguage { get; set; }
    }
}
