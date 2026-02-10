namespace MON.Models.StudentModels.Class
{
    using System;

    public class CurriculumClassModel
    {
        public int Index;
        public int CurriculumId { get; set; }
        public int? SubjectTypeId { get; set; }
        public string SubjectTypeName { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int? FlsubjectId { get; set; }
        public string FlsubjectName { get; set; }
        public int? IsIndividualLesson { get; set; }
        public int? CurriculumPartID { get; set; }
        public short? CurriculumGroupNum { get; set; }
        public bool? IsAllStudents { get; set; }
        public bool? IsWholeClass { get; set; }
        public int? ParentCurriculumId { get; set; }
        public bool IsSelectable { get; set; } = true;
        public bool IsCurriculumIncluded { get; set; }
        public string CurriculumPartName { get; set; }
        public string CurriculumPartDescription { get; set; }
        public string BC { get; set; }
        public bool IsModule { get; set; }
        public bool IsValid { get; set; }
        public bool IsProfSubject { get; set; }
        public int? SortOrder { get; set; }
        public short? WeeksFirstTerm { get; set; }
        public float? HoursWeeklyFirstTerm { get; set; }
        public short? WeeksSecondTerm { get; set; }
        public float? HoursWeeklySecondTerm { get; set; }
        public int? CurriculumStudentId { get; set; }
        public bool? IsIndividualCurriculum { get; set; }
        public double? TotalTermHours { get; set; }
        public short? CurriculumWeeksFirstTerm { get; set; }
        public float? CurriculumHoursWeeklyFirstTerm { get; set; }
        public short? CurriculumWeeksSecondTerm { get; set; }
        public float? CurriculumHoursWeeklySecondTerm { get; set; }

        public double? ComputedTotalTermHours
        {
            get
            {
                double? totalTermHours = ((this.WeeksFirstTerm ?? CurriculumWeeksFirstTerm ?? 0) * (this.HoursWeeklyFirstTerm ?? CurriculumHoursWeeklyFirstTerm ?? 0f)) 
                    + ((this.WeeksSecondTerm ?? CurriculumWeeksSecondTerm ?? 0) * (this.HoursWeeklySecondTerm ?? CurriculumHoursWeeklySecondTerm ?? 0f));

                return totalTermHours > 0 ? Math.Round(totalTermHours.Value, 2, MidpointRounding.AwayFromZero) : (double?)null;
            }
        }
    }
}
