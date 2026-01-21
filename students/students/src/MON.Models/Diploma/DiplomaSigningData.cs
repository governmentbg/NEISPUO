namespace MON.Models.Diploma
{
    using System;

    public class DiplomaSigningData
    {
        public string Signature { get; set; }
        public DateTime? SignedDate { get; set; }
        public bool IsSigned { get; set; }
        public bool IsValid { get; set; }
        public string Certificate { get; set; }
    }
}
