namespace MON.Models.StudentModels.Update
{
    using System.ComponentModel.DataAnnotations;

    public class LodEvaluationsUpdateModel : LodEvaluationsViewModel
    {
        [Required]
        public int PersonId { get; set; }
        [Required]
        public short SchoolYear { get; set; }
        public int StudentClassId { get; set; }
    }
}
