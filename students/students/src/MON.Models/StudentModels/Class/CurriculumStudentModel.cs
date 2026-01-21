namespace MON.Models.StudentModels.Class
{
    using System;

    public class CurriculumStudentModel
    {
        public int PersonId { get; set; }
        public int? CurriculumId { get; set; }
        public int? ParentCurriculumId { get; set; }
        public int? CurriculumStudentId { get; set; }
        public int? SubjectId { get; set; }
        public int? SubjectTypeId { get; set; }
        public int? CurriculumPartId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameEn { get; set; }
        public string SubjectNameDe { get; set; }
        public string SubjectNameFr { get; set; }
        public string SubjectTypeName { get; set; }
        public int? FlSubjectid { get; set; }
        public string FlSubjectName { get; set; }
        public int? FlHorarium { get; set; }
        public string CurriculumPart { get; set; }
        public string CurriculumPartName { get; set; }
        public int? AnnualHorarium { get; set; }
        public int SortOrder { get; set; }
        public bool IsLodSubject { get; set; }
        public bool IsLoadedFromStudentCurriculum { get; set; }
        public bool IsFlSubject { get; set; }
        public short? WeeksFirstTerm { get; set; }
        public float? HoursWeeklyFirstTerm { get; set; }
        public short? WeeksSecondTerm { get; set; }
        public float? HoursWeeklySecondTerm { get; set; }
        public short? CurriculumWeeksFirstTerm { get; set; }
        public float? CurriculumHoursWeeklyFirstTerm { get; set; }
        public short? CurriculumWeeksSecondTerm { get; set; }
        public float? CurriculumHoursWeeklySecondTerm { get; set; }

        /// <summary>
        /// Въвеждане на хорариум за ученик #1279
        /// https://github.com/Neispuo/students/issues/1279
        /// Навсякъде, където се занимаваме с хорариум,
        /// трябва да минем да сменим логиката - 
        /// ако има нещо по въпроса в CurriculumStudent,
        /// да го взимаме от там, ако не - от Curriculum
        /// </summary>
        public void CalcHorarium()
        {
            double? totalTermHours = ((this.WeeksFirstTerm ?? CurriculumWeeksFirstTerm ?? 0) * (this.HoursWeeklyFirstTerm ?? CurriculumHoursWeeklyFirstTerm ?? 0f))
                + ((this.WeeksSecondTerm ?? CurriculumWeeksSecondTerm ?? 0) * (this.HoursWeeklySecondTerm ?? CurriculumHoursWeeklySecondTerm ?? 0f));

            totalTermHours =  totalTermHours > 0 ? Math.Round(totalTermHours.Value, 2, MidpointRounding.AwayFromZero) : (double?)null;

            if (IsFlSubject)
            {
                FlHorarium = totalTermHours > 0 ? (int)Math.Round(totalTermHours.Value, MidpointRounding.AwayFromZero) : (int?)null;
            } else
            {
                AnnualHorarium = totalTermHours > 0 ? (int)Math.Round(totalTermHours.Value, MidpointRounding.AwayFromZero) : (int?)null;
            }
           
        }
    }
}
