using System;

namespace MON.Models.Diploma
{
    public class DiplomaFinalizationViewModel
    {
        public DateTime? SignedDate { get; set; }
        public DateTime? FinalizedDate { get; set; }
        public int CurrentStepNumber { get; set; }
        public bool IsPublic { get; set; }
        public bool IsSigned { get; set; }
        public bool IsFinalized { get; set; }
    }
}
