namespace MON.Models.StudentModels
{
    public class ClassGroupViewModel
    {
        public int? Id { get; set; }
        public short? SchoolYear { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string ClassName { get; set; }
        public int? BasicClassId { get; set; }
        public int? ClassEduFormId { get; set; }
        public int? ClassSpecialityId { get; set; }
        public int? ClassProfessionId { get; set; }
        public string BasicClassName { get; set; }
        public string BasicClassDescription { get; set; }
        public string ClassTypeName { get; set; }
        public string ClassSpecialityName { get; set; }
        public string ClassEduFormName { get; set; }
        public string SchoolYearName { get; set; }
        public int? ClassTypeId { get; set; }
        public int? ClassKindId { get; set; }
        public int StudentsInClass { get; set; }
        public bool? IsNotPresentForm { get; set; }
        public string ClassKindName { get; set; }
        public bool IsValid { get; set; }
        public string BasicClassRomeName { get; set; }
        public string ClassProfessionName { get; set; }
        public bool IsCurrent { get; set; }
    }
}
