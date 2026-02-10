namespace MON.Models.ASP
{
    using MON.Models.Certificate;

    public class SignedAspBenefits : ISignedXML
    {
        public int? AspBenefitsImportId { get; set; }
        public int? Version { get; set; }
        public string Contents { get; set; }
        public SignatureType Signature { get; set; }
    }
}
