namespace MON.Models.Diploma
{
    using System.Collections.Generic;

    public class BasicDocumentTemplateSubjectModel : BasicDocumentSubjectModel
    {
        public int? TemplateId { get; set; }
        public int BasicDocumentPartId { get; set; }
        public int? BasicDocumentSubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameShort { get; set; }
        public string SubjectTypeName { get; set; }
        public int? Horarium { get; set; }
        public bool IsHorariumHidden { get; set; }
        public int? BasicClassId { get; set; }
        public decimal? Grade { get; set; }
        public string GradeText { get; set; }
        public int? GradeCategory { get; set; }
        public int? SpecialNeedsGrade { get; set; }
        public int? OtherGrade { get; set; }
        public int? QualitativeGrade { get; set; }
        public decimal? NvoPoints { get; set; }
        public string FlLevel { get; set; }
        public int? FlSubjectId { get; set; }
        public string FlSubjectName { get; set; }
        public int? FlHorarium { get; set; }
        public string Ects { get; set; }
        public bool ShowFlSubject { get; set; }
        public List<BasicDocumentTemplateSubjectModel> Modules { get; set; }
        public bool ShowSubjectNamePreview { get; set; }
        /// <summary>
        /// Определя видимостта на опцията за избор на випуск в компонента за подрубрика.
        /// </summary>
        public bool IsProfSubjectHeader { get; set; }
    }

}
