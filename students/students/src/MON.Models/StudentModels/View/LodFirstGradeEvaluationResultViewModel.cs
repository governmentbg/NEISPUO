using System;
using System.Collections.Generic;

namespace MON.Models.StudentModels.View
{
    public class LodFirstGradeEvaluationResultViewModel
    {
        public IEnumerable<LodFirstGradeEvaluationViewModel> LodFirstGradeEvaluationViewModels { get; set; }
        public int GradeResult { get; set; }
        public bool IsFinalized { get; set; }
        public DateTime? FinalizedDate { get; set; }
    }
}
