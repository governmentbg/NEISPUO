namespace MON.DataAccess
{
    using MON.Models.StudentModels.Lod;
    using MON.Shared.Interfaces;
    using System;

    public partial class LodAssessmentGrade : ICreatable, IUpdatable
    {
        public static LodAssessmentGrade FromModel(LodAssessmentGradeCreateModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), nameof(LodAssessmentGradeCreateModel));
            }

            if (!model.GradeId.HasValue || !model.GradeCategoryId.HasValue)
            {
                return null;
            }
            

            return new LodAssessmentGrade
            {
                GradeId = model.GradeId.Value,
                GradeCategoryId = model.GradeCategoryId.Value,
                DecimalGrade = model.DecimalGrade
            };
        }
    }
}