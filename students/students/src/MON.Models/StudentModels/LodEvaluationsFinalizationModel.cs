namespace MON.Models.StudentModels
{
    public class LodEvaluationsFinalizationModel
    {
        public int PersonId { get; set; }
        public int SchoolYear { get; set; }
        public bool IsSelfEduForm { get; set; }
        public int? StudentClassId { get; set; }
    }
}
