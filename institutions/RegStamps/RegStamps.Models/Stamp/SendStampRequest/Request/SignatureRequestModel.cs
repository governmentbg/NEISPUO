namespace RegStamps.Models.Stamp.SendStampRequest.Request
{
    public class SignatureRequestModel
    {
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public string Organisation { get; set; }
        public string SignerBulstat { get; set; }
        public string SignerIdent { get; set; }
        public string Signature { get; set; }
    }
}
