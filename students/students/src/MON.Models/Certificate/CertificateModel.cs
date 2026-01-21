namespace MON.Models.Certificate
{
    using System;

    public class CertificateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime NotAfter { get; set; }
        public DateTime NotBefore { get; set; }
        public string SerialNumber { get; set; }
        public string Thumbprint { get; set; }
        public string Issuer { get; set; }
        public string Subject { get; set; }
        public byte[] Contents { get; set; }
        public int CertificateType { get; set; }
        public bool IsValid { get; set; }
        public string Description { get; set; }
    }
}
