using System.ComponentModel.DataAnnotations;

namespace MON.Models.StudentModels.Update
{
    public class LodFirstGradeEvaluationResultModel
    {
        [Required]
        public int PersonId { get; set; }
        [Required]
        public short SchoolYear { get; set; }
        [Required]
        public int GradeResult { get; set; }
        public int? StudentClassId { get; set; }
    }
}
