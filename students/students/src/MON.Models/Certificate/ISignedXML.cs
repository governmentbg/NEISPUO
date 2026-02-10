namespace MON.Models.Certificate
{
    public interface ISignedXML
    {
        SignatureType Signature { get; set; }
    }

    public class SignedXML
    {
        public SignatureType Signature { get; set; }
    }
}
