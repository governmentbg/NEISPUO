namespace MON.Models.StudentModels.View
{
    public class LodFirstGradeEvaluationViewModel : LodEvaluationSectionBaseModel
    {
        public int? FirstTermGrade { get; set; }
        public int? SecondTermGrade { get; set; }
        public int? FinalGrade { get; set; }
    }
}