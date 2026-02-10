namespace MON.Models.Diploma
{
    using System;

    public class DiplomaCreateRequestModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public int RequestingInstitutionId { get; set; }
        public int? CurrentInstitutionId { get; set; }
        public string CurrentInstitutionName { get; set; }
        public int BasicDocumentId { get; set; }
        public string RegistrationNumber { get; set; }
        public string RegistrationNumberYear { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string Note { get; set; }
        public bool IsGranted { get; set; }
        public int? DiplomaId { get; set; }
        public bool ArbitraryCurrentInstitutionName { get; set; }

    }
}
