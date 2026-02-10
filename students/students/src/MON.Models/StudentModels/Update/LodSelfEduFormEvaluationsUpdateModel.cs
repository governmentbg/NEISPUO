namespace MON.Models.StudentModels.Update
{
    using System.ComponentModel.DataAnnotations;

    public class LodSelfEduFormEvaluationsUpdateModel : LodSelfEduFormEvaluationsViewModel
    {
        [Required]
        public int PersonId { get; set; }
        [Required]
        public short SchoolYear { get; set; }
        public int StudentClassId { get; set; }
    }
}
