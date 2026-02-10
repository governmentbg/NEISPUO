namespace MON.DataAccess.Dto
{
    using System;

    public class StudentsMainClassesDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public int ClassId { get; set; }
        public int BasicClassId { get; set; }
        public int PositionId { get; set; }
        public bool IsCurrent { get; set; }
        public bool? IsNotPresentForm { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionAbbreviation { get; set; }
        public string BasicClassName { get; set; }
        public string BasicClassRomeName { get; set; }
        public string ClassGroupName { get; set; }
        public string SchoolYearName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool? IsLodFinalized { get; set; }

    }
}
