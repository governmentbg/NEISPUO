namespace MON.Models.StudentModels.Update
{
    using System.Collections.Generic;

    public class StudentSopUpdateModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public IEnumerable<SopDetailsViewModel> SopDetails { get; set; } = new List<SopDetailsViewModel>();
        public IEnumerable<DocumentModel> Documents { get; set; } = new List<DocumentModel>();
    }
}
