namespace RegStamps.Services.Signature
{
    using System.Security.Cryptography.X509Certificates;

    using Models.Stamp.SendStampRequest.Request;

    public interface ICertificateService
    {
        SignatureRequestModel PrepareSignatureData(X509Certificate2 certificate2);
    }
}
