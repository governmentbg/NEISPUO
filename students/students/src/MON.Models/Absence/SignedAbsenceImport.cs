using MON.Models.Certificate;

namespace MON.Models.Absence
{
    public class SignedAbsenceImport : ISignedXML
    {
        public int? AbsenceImportId { get; set; }
        public int? Version { get; set; }
        public string Contents { get; set; }
        public string BlobHash { get; set; }
        public SignatureType Signature { get; set; }
    }
}
