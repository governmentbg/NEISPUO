using MON.Models.StudentModels.Lod;
using System;
using System.Collections.Generic;

namespace MON.Models.StudentModels.StoredProcedures
{
    public class StudentTermGradesModel
    {
        public string Uid => Guid.NewGuid().ToString();
        public int RelocationDocumentId { get; set; }
        public int? Id { get; set; }
        public int CurriculumID { get; set; }
        public int? StudentID { get; set; }
        public int InstitutionID { get; set; }
        public short? SchoolYear { get; set; }
        public int? CurriculumPartID { get; set; }
        public string CurriculumPart { get; set; }
        public string CurriculumPartName { get; set; }
        public int? SubjectID { get; set; }
        public int? SubjectTypeID { get; set; }
        public string SubjectName { get; set; }
        public string BasicSubjectName { get; set; }
        public string BasicSubjectAbrev { get; set; }
        public string SubjectTypeName { get; set; }
        public string GradesString { get; set; }
        public int? Term { get; set; }
        public int PersonId { get; set; }
        public bool IsLoadedFromSchoolbook { get; set; }
        public int SortOrder { get; set; }
        public List<LodAssessmentGradeCreateModel> Grades { get; set; } = new List<LodAssessmentGradeCreateModel>();
    }
}
