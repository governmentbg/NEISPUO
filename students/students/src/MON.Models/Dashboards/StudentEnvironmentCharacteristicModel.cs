namespace MON.Models.Dashboards
{
    using System;

    public class StudentEnvironmentCharacteristicModel
    {
        public string Uid => Guid.NewGuid().ToString();
        public int StudentClassId { get; set; }
        public int PersonId { get; set; }
        public string FullName { get; set; }
        public string Pin { get; set; }
        public string PinType { get; set; }
        public string ClassName { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public string RelativeFullName { get; set; }
        public string RelativeType { get; set; }
        public string WorkStatus { get; set; }
        public string EducationType { get; set; }
        public int BasicClassId { get; set; }
        public bool HasParentConsent { get; set; }
    }
}
