using System;

namespace MON.Models.EduState
{
    [Serializable]
    public class EduStateModel
    {
        public int? PersonId { get; set; }
        public int? PositionId { get; set; }
        public int? InstitutionId { get; set; }
        public string PersonalId { get; set; }
    }
}
