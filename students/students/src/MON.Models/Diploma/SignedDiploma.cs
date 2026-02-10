namespace MON.Models.Diploma
{
    using MON.Models.Certificate;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Подписана диплома
    /// </summary>
    public class SignedDiploma : ISignedXML
    {
        public SignedDiploma()
        {
            ImageHashes = new List<string>();
        }

        public int Version { get; set; }
        public string Contents { get; set; }
        public string PersonalId { get; set; }
        public int PersonalIdType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Series { get; set; }
        public string FactoryNumber { get; set; }
        public string RegNumberTotal { get; set; }
        public string RegNumberYear { get; set; }
        public DateTime? RegDate { get; set; }
        public short SchoolYear { get; set; }
        public List<string> ImageHashes { get; set; }
        public SignatureType Signature { get; set; }
    }
}
