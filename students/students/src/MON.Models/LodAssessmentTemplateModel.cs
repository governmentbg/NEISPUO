namespace MON.Models
{
    using MON.Models.StudentModels.Lod;
    using System.Collections.Generic;

    public class LodAssessmentTemplateModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BasicClassId { get; set; }
        public bool IsSelfEduForm { get; set; } = true;
        public List<LodAssessmentCurriculumPartModel> CurriculumParts { get; set; } = new List<LodAssessmentCurriculumPartModel>();
    }
}
