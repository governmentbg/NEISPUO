using System;

namespace MON.Models.StudentModels
{
    public class LodEvaluationGeneralModel
    {
        public int? Id { get; set; }
        public string MajorCourse1 { get; set; }
        public string MajorCourse2 { get; set; }
        public string MajorCourse3 { get; set; }
        public string MajorCourse4 { get; set; }
        public bool IsSelfEduForm { get; set; }
        public bool IsFinalized { get; set; }
        public DateTime? FinalizedDate { get; set; }
        public DropdownViewModel Result { get; set; }
    }
}