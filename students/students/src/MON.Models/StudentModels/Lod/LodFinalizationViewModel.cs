namespace MON.Models.StudentModels.Lod
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class LodFinalizationViewModel
    {
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public bool IsApproved { get; set; }
        public bool IsFinalized { get; set; }
        public int? InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public DocumentModel Document { get; set; }

        public bool CanBeUndone { get; set; }
        public string FullName { get; set; }
        public string Pin { get; set; }
        public string PinType { get; set; }
    }
}
