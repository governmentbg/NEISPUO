namespace MON.Models.StudentModels.Lod
{
    using System;

    public class LodAssessmentGradeCreateModel
    {
        public string Uid => Guid.NewGuid().ToString();
        public int? Id { get; set; }
        public int? GradeId { get; set; }
        public string GradeText { get; set; }

        /// <summary>
        /// Източник на оценката: Дневник, Приравняване, Признаване, ЛОД
        /// </summary>
        public string GradeSource { get; set; }

        public int? GradeCategoryId { get; set; }
        public decimal? DecimalGrade { get; set; }

        /// <summary>
        /// Референция към student.GradeType (Рубрика, Нормална оценка, СОП оценка, Друга оценка, Оценка от начален етап, ECTS)
        /// </summary>
        public int? GradeTypeId { get; set; }
        public string ClassBookName { get; set; }

        public string PrintGradeText => GradeId.HasValue && GradeId >= 2 && GradeId <= 6 ? GradeId.Value.ToString() : GradeText;

        public byte? GradeNomSort { get; set; }
        public string CategoryAbbreviation { get; set; }
    }
}
