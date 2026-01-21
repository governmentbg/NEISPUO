namespace MON.Models.StudentModels.Lod
{
    using System.Collections.Generic;
    using System.Linq;

    public class LodAssessmentCurriculumPartModel
    {
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public int BasicClassId { get; set; }
        public int CurriculumPartId { get; set; }
        public string CurriculumPart { get; set; }
        public string CurriculumPartName { get; set; }
        public bool IsSelfEduForm { get; set; }
        public List<LodAssessmentCreateModel> SubjectAssessments { get; set; } = new List<LodAssessmentCreateModel>();

        public List<LodAssessmentCreateModel> GetLodAssessmentCreateModels()
        {
            List<LodAssessmentCreateModel> models = SubjectAssessments
                .Where(x => x.IsLodSubject)
                .Select(x => x.GetLodAssessmentCreateModel())
                .ToList();

            foreach (LodAssessmentCreateModel model in models)
            {
                if (!model.PersonId.HasValue) model.PersonId = PersonId;
                if (!model.SchoolYear.HasValue) model.SchoolYear = SchoolYear;
                if (!model.BasicClassId.HasValue) model.BasicClassId = BasicClassId;
                if (!model.CurriculumPartId.HasValue) model.CurriculumPartId = CurriculumPartId;
                if (!model.IsSelfEduForm.HasValue) model.IsSelfEduForm = IsSelfEduForm;

                foreach (var module in model.LodAssessmentChildren)
                {
                    if (!module.PersonId.HasValue) module.PersonId = PersonId;
                    if (!module.SchoolYear.HasValue) module.SchoolYear = SchoolYear;
                    if (!module.BasicClassId.HasValue) module.BasicClassId = BasicClassId;
                    if (!module.CurriculumPartId.HasValue) module.CurriculumPartId = CurriculumPartId;
                    if (!module.IsSelfEduForm.HasValue) module.IsSelfEduForm = IsSelfEduForm;
                }
            }

            return models;
        }
    }
}
