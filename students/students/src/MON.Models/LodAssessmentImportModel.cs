namespace MON.Models
{
    using MON.Shared.ErrorHandling;
    using System;

    public class LodAssessmentImportModel
    {
        public int InstitutionId { get; set; }
        public short SchoolYear { get; set; }
        public string PersonalId { get; set; }
        public int PersonalIdType { get; set; }

        /// <summary>
        /// Разчитаме, че отговаря на колона tmp_CurricID_orig от inst_year.Curriculum
        /// </summary>
        public int CurriculumId { get; set; }
        public int GradeType { get; set; }
        public int GradeCode { get; set; }
        public int SubjectId { get; set; }
        public int? ProfSubjectId { get; set; }
        public int SubjectTypeId { get; set; }
        public string SubjectName { get; set; }

        public decimal? ProfSubjectDecimalGrade { get; set; }

        public ValidationErrorCollection Errors { get; set; } = new ValidationErrorCollection();
        public bool HasErrors => Errors != null && Errors.Count > 0;

        public int? BasicClassId { get; set; }
        public int PersonId { get; set; }
        public int CurriculumPartId { get; set; }

        /// <summary>
        /// Изполва се само при показване на валидационните грешки
        /// </summary>
        public string ProfSubjectDisplayText { get; set; }

        /// <summary>
        /// Използва се за ключ при листване в таблица
        /// </summary>
        public string Uid => Guid.NewGuid().ToString();

        public bool IsNotPresentForm { get; set; }

        public int? Horarium { get; set; }

        public string ToFileLine()
        {
            return $"{InstitutionId}|{SchoolYear}|{PersonalId}|{PersonalIdType}" +
                $"|{CurriculumId}|{GradeType}|{GradeCode}|{SubjectId}|{ProfSubjectId}" +
                $"|{SubjectTypeId}|{SubjectName}|{ProfSubjectDecimalGrade}|{BasicClassId}";
        }
    }
}
