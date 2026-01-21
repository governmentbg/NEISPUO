namespace MON.Models.StudentModels.Lod
{
    using MON.Models.Certificate;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class SignedLod: ISignedXML
    {
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public SignatureType Signature { get; set; }
    }
}
