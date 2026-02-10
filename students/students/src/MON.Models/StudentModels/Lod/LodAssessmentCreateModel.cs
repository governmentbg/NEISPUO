namespace MON.Models.StudentModels.Lod
{
    using MON.Shared.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LodAssessmentCreateModel
    {
        public int? Id { get; set; }
        public int SubjectId { get; set; }
        public int SubjectTypeId { get; set; }
        public int? AnnualHorarium { get; set; }
        public bool ShowFlSubject { get; set; }
        public int? FlSubjectId { get; set; }
        public string FlSubjectName { get; set; }
        public int? FlHorarium { get; set; }
        public int SortOrder { get; set; }
        public bool IsModule { get; set; }
        public string Uid { get; set; } = Guid.NewGuid().ToString();
        public string SubjectName { get; set; }
        public string SubjectTypeName { get; set; }
        public bool IsLodSubject { get; set; }
        public HashSet<string> Categories { get; set; }
        public int? PersonId { get; set; }
        public short? SchoolYear { get; set; }
        public int? BasicClassId { get; set; }
        public int? CurriculumPartId { get; set; }
        public bool? IsSelfEduForm { get; set; }
        public int? CurriculumId { get; set; }
        public List<LodAssessmentGradeCreateModel> FirstTermAssessments { get; set; } = new List<LodAssessmentGradeCreateModel>();
        public List<LodAssessmentGradeCreateModel> SecondTermAssessments { get; set; } = new List<LodAssessmentGradeCreateModel>();
        public List<LodAssessmentGradeCreateModel> AnnualTermAssessments { get; set; } = new List<LodAssessmentGradeCreateModel>();
        public List<LodAssessmentGradeCreateModel> FirstRemedialSession { get; set; } = new List<LodAssessmentGradeCreateModel>();
        public List<LodAssessmentGradeCreateModel> SecondRemedialSession { get; set; } = new List<LodAssessmentGradeCreateModel>();
        public List<LodAssessmentGradeCreateModel> AdditionalRemedialSession { get; set; } = new List<LodAssessmentGradeCreateModel>();
        public List<LodAssessmentCreateModel> Modules { get; set; } = new List<LodAssessmentCreateModel>();


        public List<LodAssessmentGradeCreateModel> LodAssessmentGrades { get; set; } = new List<LodAssessmentGradeCreateModel>();
        public List<LodAssessmentCreateModel> LodAssessmentChildren { get; set; } = new List<LodAssessmentCreateModel>();

        /// <summary>
        /// Показва дали предмета се зарежда от учебния план или за него има оценки в дневника, LodAssessment, приравняване, признаване.
        /// </summary>
        public bool IsLoadedFromStudentCurriculum { get; set; }
        public int Index { get; set; }
        public int? StudentClassStatus { get; set; }
        public string CurriculumStudentId { get; set; }

        public HashSet<int?> CurriculumIds { get; set; }
        public LodAssessmentCreateModel GetLodAssessmentCreateModel()
        {
            LodAssessmentCreateModel model = new LodAssessmentCreateModel
            {
                Id = Id,
                PersonId = PersonId,
                SubjectId = SubjectId,
                SubjectTypeId = SubjectTypeId,
                CurriculumPartId = CurriculumPartId,
                SchoolYear = SchoolYear,
                //InstitutionId ??
                BasicClassId = BasicClassId,
                // IsSelfEduForm ??
                SortOrder = SortOrder,
                AnnualHorarium = AnnualHorarium,
                FlSubjectId = FlSubjectId,
                FlHorarium = FlHorarium,
                IsLoadedFromStudentCurriculum = IsLoadedFromStudentCurriculum,
                LodAssessmentGrades = new List<LodAssessmentGradeCreateModel>(),
                LodAssessmentChildren = Modules.Select(x => x.GetLodAssessmentCreateModel()).ToList(),
            };

            model.LodAssessmentGrades.AddRange(FirstTermAssessments.Select(x => new LodAssessmentGradeCreateModel
            {
                Id = x.Id,
                GradeId = x.GradeId,
                GradeCategoryId = (int)LodAssessmentGradeCategoryEnum.FirstTerm,
                DecimalGrade = x.DecimalGrade,
                GradeSource = x.GradeSource,
                ClassBookName= x.ClassBookName,
            }));

            model.LodAssessmentGrades.AddRange(SecondTermAssessments.Select(x => new LodAssessmentGradeCreateModel
            {
                Id = x.Id,
                GradeId = x.GradeId,
                GradeCategoryId = (int)LodAssessmentGradeCategoryEnum.SecondTerm,
                DecimalGrade = x.DecimalGrade,
                GradeSource = x.GradeSource,
                ClassBookName = x.ClassBookName,
            }));

            model.LodAssessmentGrades.AddRange(AnnualTermAssessments.Select(x => new LodAssessmentGradeCreateModel
            {
                Id = x.Id,
                GradeId = x.GradeId,
                GradeCategoryId = (int)LodAssessmentGradeCategoryEnum.Final,
                DecimalGrade = x.DecimalGrade,
                GradeSource = x.GradeSource,
                ClassBookName = x.ClassBookName,
            }));

            model.LodAssessmentGrades.AddRange(FirstRemedialSession.Select(x => new LodAssessmentGradeCreateModel
            {
                Id = x.Id,
                GradeId = x.GradeId,
                GradeCategoryId = (int)LodAssessmentGradeCategoryEnum.FirstRemedial,
                DecimalGrade = x.DecimalGrade,
                GradeSource = x.GradeSource,
                ClassBookName = x.ClassBookName,
            }));

            model.LodAssessmentGrades.AddRange(SecondRemedialSession.Select(x => new LodAssessmentGradeCreateModel
            {
                Id = x.Id,
                GradeId = x.GradeId,
                GradeCategoryId = (int)LodAssessmentGradeCategoryEnum.SecondRemedial,
                DecimalGrade = x.DecimalGrade,
                GradeSource = x.GradeSource,
                ClassBookName = x.ClassBookName,
            }));

            model.LodAssessmentGrades.AddRange(AdditionalRemedialSession.Select(x => new LodAssessmentGradeCreateModel
            {
                Id = x.Id,
                GradeId = x.GradeId,
                GradeCategoryId = (int)LodAssessmentGradeCategoryEnum.AdditionalRemedial,
                DecimalGrade = x.DecimalGrade,
                GradeSource = x.GradeSource,
                ClassBookName = x.ClassBookName,
            }));

            return model;
        }
    }
}
