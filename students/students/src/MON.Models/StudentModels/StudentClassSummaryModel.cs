namespace MON.Models.StudentModels
{
    using System;

    public class StudentClassSummaryModel
    {
        public int ClassId { get; set; }
        public int SchoolYear { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public int StudentClassId { get; set; }
        public string ClassName { get; set; }
        public int BasicClassId { get; set; }
        public string BasicClassName { get; set; }
        public string ClassTypeName { get; set; }
        public string ClassSpecialityName { get; set; }
        public int ClassEduFormId { get; set; }
        public string ClassEduFormName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool? IsCurrent { get; set; }
        public int? Status { get; set; }
        public int? PositionId { get; set; }
        public string Position { get; set; }
        public int? ClassKind { get; set; }
    }
}
