namespace MON.DataAccess.Dto
{
    using System;

    public class UnsignedStudentLodListDto
    {
        public string Uid => Guid.NewGuid().ToString();
        public int PersonId { get; set; }
        public string FullName { get; set; }
        public string Identifier { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionAbbreviation { get; set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public short SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public string BasicClass { get; set; }
        public int? EduStateMainInstitutionId { get; set; }
        public string EduStateMainInstitutionName { get; set; }
        public string EduStateMainInstitutionAbbreviation { get; set; }
    }
}
