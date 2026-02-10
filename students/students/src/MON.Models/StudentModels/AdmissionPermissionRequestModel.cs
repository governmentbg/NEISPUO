namespace MON.Models.StudentModels
{
    using System.Collections.Generic;

    public class AdmissionPermissionRequestModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public int RequestingInstitutionId { get; set; }
        public int AuthorizingInstitutionId { get; set; }
        public string Note { get; set; }
        public bool IsPermissionGranted { get; set; }
        public IEnumerable<DocumentViewModel> Documents { get; set; }
    }
}
