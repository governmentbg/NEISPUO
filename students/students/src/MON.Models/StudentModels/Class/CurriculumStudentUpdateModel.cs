namespace MON.Models.StudentModels.Class
{
    public class CurriculumStudentUpdateModel
    {
        public int PersonId { get; set; }
        public int[] CurriculumIds { get; set; }
        public short SchoolYear { get; set; }
        public int? StudentClassId { get; set; }

        public short? WeeksFirstTerm { get; set; }
        public float? HoursWeeklyFirstTerm { get; set; }
        public short? WeeksSecondTerm { get; set; }
        public float? HoursWeeklySecondTerm { get; set; }
        public int CurriculumStudentId { get; set; }
    }
}
