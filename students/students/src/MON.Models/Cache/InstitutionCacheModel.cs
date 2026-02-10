namespace MON.Models
{
    using MON.Shared.Enums;

    /// <summary>
    /// Използва се за кеширане на често използвани данни
    /// </summary>
    public class InstitutionCacheModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? InstTypeId { get; set; }
        public int RegionId { get; set; }
        public int DetailedSchoolTypeId { get; set; }
        public string SchoolYearName { get; set; }
        public short SchoolYear { get; set; }
        public int BaseSchoolTypeId { get; set; }

        public bool IsSchool => InstTypeId.HasValue && InstTypeId.Value == (int)InstitutionTypeEnum.School;
        public bool IsKinderGarden => InstTypeId.HasValue && InstTypeId.Value == (int)InstitutionTypeEnum.KinderGarden;
        public bool IsCSOP => InstTypeId.HasValue && InstTypeId.Value == (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport;
        public bool IsCPLR => InstTypeId.HasValue 
            && (InstTypeId.Value == (int)InstitutionTypeEnum.PersonalDevelopmentSupportCenter || InstTypeId.Value == (int)InstitutionTypeEnum.SpecializedServiceUnit);
    }
}
