namespace MON.DataAccess
{
    public partial class TmpCurriculum
    {
        public int CurriculumId { get; set; }
        public int SchoolYear { get; set; }
        public int InstitutionId { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public short? CurriculumGroupId { get; set; }
        public string CurriculumGroupName { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameAbreviation { get; set; }
        public short? SubjectTypeId { get; set; }
        public string SubjectTypeName { get; set; }
        public string HoursWeeklyFirstTerm { get; set; }
        public string HoursWeeklySecondTerm { get; set; }
        public bool? IsFl { get; set; }
        public int? FlsubjectId { get; set; }
        public string FlsubjectName { get; set; }
        public int? IsIndividual { get; set; }
        public int? CurriculumPartId { get; set; }
        public int? CurricIdOrig { get; set; }
        public int? ClassIdOrig { get; set; }
        public short? SubjectIdOrig { get; set; }
    }
}
