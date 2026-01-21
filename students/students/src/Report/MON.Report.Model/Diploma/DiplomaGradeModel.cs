using MON.Report.Model.Diploma;
using MON.Report.Model.Enums;
using MON.Report.Model.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MON.Report.Model
{

    public class DiplomaGradeModel : IGradeWithSubjectName
    {
        private readonly Dictionary<int, int> SpecialNeedGradeCodeMapper = new Dictionary<int, int>
        {
            { 1, 22 }, // Среща затруднени
            { 2, 21 }, // Справя се
            { 3, 20 }, // Постига изискванията
        };

        private readonly Dictionary<int, int> OtherGradeCodeMapper = new Dictionary<int, int>
        {
            { 1, 30 }, // Зачита се
            { 2, 31 }, // Не се зачита
            { 3, 32 }, // Освободен
            { 4, 33 }, // Интензивно
            { 5, 34 }, // Без оценка
        };

        public int Id { get; set; }
        public int DocumentPartId { get; set; }
        public string DocumentPartCode { get; set; }
        public string DocumentPartName { get; set; }
        public string DocumentPartDescription { get; set; }
        public string ExternalEvaluationTypeList { get; set; }
        public int? ExternalEvaluationTypeId { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameForeign { get; set; }
        public string SubjectNameShort { get; set; }
        public int? SubjectTypeId { get; set; }
        public int GradeCategory { get; set; }
        public decimal? Grade { get; set; }
        public int? SpecialNeedsGrade { get; set; }
        public int? OtherGrade { get; set; }
        public int? QualitativeGrade { get; set; }
        public string GradeText { get; set; }
        public string ECTS { get; set; }
        public int? Horarium { get; set; }
        public int? ParentPosition { get; set; }
        public int? Position { get; set; }
        public int? BasicClassId { get; set; }
        public string BasicClass { get; set; }
        public int? ParentId { get; set; }
        public string BasicSubjectTypeAbrev { get; set; }
        public int? BasicSubjectType { get; set; }
        public decimal? Points { get; set; }
        public string FLLevel { get; set; }
        public int? FLSubjectId { get; set; }
        public string FLSubjectName { get; set; }
        public int? FLHorarium { get; set; }
        /// <summary>
        /// Текст за изучаване на чужд език, под формата на (на английски език - 108 часа)
        /// </summary>
        public string FLAddition { get; set; }

        public void TextGradeFixture(Dictionary<int, string> gradeNomenclature, string basicDocumentName)
        {
            string[] split = (this.GradeText ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Има оценки като "Много добър 5". Трябва да махнем числото.
            this.GradeText = string.Join(" ", split.Where(x => !decimal.TryParse(x, out decimal num)));

            switch (GradeCategory)
            {
                // За 1-ви, 2-ри и 3-ти клас 2-ката се печата като "Незадоволителен" вместо "Слаб".
                // Todo: Да се мисли за махане на хардкоднататата проверка на името на BasicDocument-а.
                case (int)GradeCategoryEnum.Normal:
                    if(this.GradeText.Equals("Слаб", StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(basicDocumentName) && basicDocumentName.StartsWith("3-23"))
                    {
                        this.GradeText = "Незадоволителен";
                    }

                    break;
                // СОП Оценка
                case (int)GradeCategoryEnum.SpecialNeeds:
                    if (this.SpecialNeedsGrade.HasValue && this.SpecialNeedsGrade < 20 && SpecialNeedGradeCodeMapper.TryGetValue(this.SpecialNeedsGrade.Value, out int specialNeedGradeId))
                    {
                        this.SpecialNeedsGrade = specialNeedGradeId;
                    }

                    this.GradeText = this.SpecialNeedsGrade.HasValue && gradeNomenclature.ContainsKey(this.SpecialNeedsGrade.Value)
                        ? gradeNomenclature[this.SpecialNeedsGrade.Value]
                        : this.GradeText;
                    break;
                // Друга Оценка
                case (int)GradeCategoryEnum.Other:
                    if (this.OtherGrade.HasValue && this.OtherGrade < 30 && OtherGradeCodeMapper.TryGetValue(this.OtherGrade.Value, out int otherGradeId))
                    {
                        this.OtherGrade = otherGradeId;
                    }

                    if (this.OtherGrade.HasValue && this.OtherGrade.Value == 34)
                    {
                        // Без оценка
                        this.GradeText = null;
                    } else
                    {
                        this.GradeText = this.OtherGrade.HasValue && gradeNomenclature.ContainsKey(this.OtherGrade.Value)
                          ? gradeNomenclature[this.OtherGrade.Value]
                          : this.GradeText;
                    }
                    break;
                // Качествена Оценка
                case (int)GradeCategoryEnum.Qualitative:
                    if (this.QualitativeGrade.HasValue && this.QualitativeGrade.Value == 45)
                    {
                        // Без оценка
                        this.GradeText = null;
                    }
                    else
                    {
                        this.GradeText = this.QualitativeGrade.HasValue && gradeNomenclature.ContainsKey(this.QualitativeGrade.Value)
                          ? gradeNomenclature[this.QualitativeGrade.Value]
                          : this.GradeText;
                    }
                    break;
            }
        }
    }
}
