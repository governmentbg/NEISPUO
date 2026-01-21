namespace MON.Models.Refugee
{
    using System;

    public class RefugeeAdmissionViewModel
    {
        public int? PersonId { get; set; }
        public string FullName { get; set; }
        public int? Age { get; set; }
        public string InstitutionCode { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionTown { get; set; }
        public string InstitutioRegion { get; set; }
        public string AdmissionStatus { get; set; }
        public string Classes { get; set; }
        public int? ApplicationId { get; set; }
        public int? CurrentInstitutionId { get; set; }
        public string RuodocNumber { get; set; }
        public DateTime? RuodocDate { get; set; }
        public string Pin { get; set; }
        public string PinType { get; set; }
    }
}
