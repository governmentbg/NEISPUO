using System.Collections.Generic;

namespace MON.Models.StudentModels
{
    public class StudentViewModel : StudentModel
    {
        public string Gender { get; set; }
        public bool Sop { get; set; }
        public string BirthDateString => BirthDate.HasValue ? BirthDate.Value.Day + "." + BirthDate.Value.Month + "." + BirthDate.Value.Year : string.Empty;
        public int PersonId { get; set; }
        public bool HasStudentClassInCurrentYear { get; set; }
        public AddressViewModel PermanentResidenceAddress { get; set; }
        public AddressViewModel UsualResidenceAddress { get; set; }
        public EducationViewModel Education { get; set; }
        public IEnumerable<InstitutionCacheModel> Institutions { get; set; }
        public IEnumerable<OtherInstitutionViewModel> OtherInstitutions { get; set; }
        public LodModel Lod { get; set; }
        public IEnumerable<RelocationDocumentModel> RelocationDocumentModels { get; set; }
        public IEnumerable<AdmissionDocumentModel> AdmissionDocumentModels { get; set; }
        public IEnumerable<DischargeDocumentModel> DischargeDocumentModels { get; set; }
        public IEnumerable<OtherDocumentModel> OtherDocuments { get; set; }
        public IEnumerable<InternationalProtectionModel> InternationalProtections { get; set; }
    }
}
