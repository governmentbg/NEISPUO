namespace MON.Models.StudentModels.Lod
{
    public class LodAssessmentViewModel : LodAssessmentModel
    {
        public string SubjectName { get; set; }
        public string SubjectNameEn { get; set; }
        public string SubjectNameDe { get; set; }
        public string SubjectNameFr { get; set; }
        public int InstitutionId { get; set; }
        public int Type { get; set; }
        public int Term { get; set; }
        /// <summary>
        /// Източника на оценката
        ///     Дневник
        ///     Признаване
        ///     Приравняване
        ///     СФО/Ръчно въвеждане в ЛОД
        /// </summary>
        public string Category { get; set; }
        public string ClassBookName { get; set; }
        public string GradeText { get; set; }
        public string TypeName { get; set; }
        public string GradeTooltip => $"{TypeName} оценка. Източник: {Category}{(Category.Equals("Дневник") ? $", {ClassBookName}" : "")}";
    }
}
