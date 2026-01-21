using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MON.Shared.Certificate
{
    public interface ISignedXML
    {
        SignatureType Signature { get; set; }
    }
}
