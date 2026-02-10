namespace RegStamps.Services.Signature
{
    using System.Security.Cryptography.X509Certificates;

    using Models.Stamp.SendStampRequest.Request;

    public class CertificateService : ICertificateService
    {
        public SignatureRequestModel PrepareSignatureData(X509Certificate2 certificate)
        {
            SignatureRequestModel signature = new SignatureRequestModel();

            if (this.Parse(certificate.Subject, "CN") is not null)
            {
                signature.SignerName = this.Parse(certificate.Subject, "CN")[0];
            }

            if (this.Parse(certificate.Subject, "S") is not null)
            {
                if (certificate.Subject.IndexOf('"') > 0)
                {
                    int pFrom = certificate.Subject.IndexOf('"') + 1;
                    int pTo = certificate.Subject.LastIndexOf('"');

                    string[] result = certificate.Subject.Substring(pFrom, pTo - pFrom).Split(',');
                    foreach (string str in result)
                    {
                        if (str.Split(':')[0] == "EGN")
                        {
                            signature.SignerIdent = str.Split(':')[1];
                        }
                        if (str.Split(':')[0] == "B")
                        {
                            signature.SignerBulstat = str.Split(':')[1];
                        }
                    }
                }
                else
                {
                    foreach (string str in this.Parse(certificate.Subject.Replace("\"", ""), "S"))
                    {
                        if (str.Split(':')[0] == "EGN")
                        {
                            signature.SignerIdent = str.Split(':')[1];
                        }
                        if (str.Split(':')[0] == "B")
                        {
                            signature.SignerBulstat = str.Split(':')[1];
                        }
                    }
                }
            }
            if (this.Parse(certificate.Subject, "E") is not null)
            {
                signature.SignerEmail = this.Parse(certificate.Subject, "E")[0];
            }

            if (this.Parse(certificate.Subject, "O") is not null)
            {
                signature.Organisation = this.Parse(certificate.Subject, "O")[0];
            }
            if (this.Parse(certificate.Subject, "OU") is not null)
            {
                foreach (string str in this.Parse(certificate.Subject, "OU"))
                {
                    if (str.Split(':')[0] == "BULSTAT")
                    {
                        signature.SignerBulstat = str.Split(':')[1];
                    }
                    if (str.Split(':')[0] == "EGN")
                    {
                        signature.SignerIdent = str.Split(':')[1];
                    }
                }
            }
            if (this.Parse(certificate.Subject, "OID.2.5.4.10.100.1.1") is not null)
            {
                signature.SignerBulstat = this.Parse(certificate.Subject, "OID.2.5.4.10.100.1.1")[0];
            }

            /// eIDAS Personal
            if (this.Parse(certificate.Subject, "SERIALNUMBER") is not null)
            {
                foreach (string str in this.Parse(certificate.Subject, "SERIALNUMBER"))
                {
                    signature.SignerIdent = str;
                }
            }

            /// eIDAS BULSTAT
            if (this.Parse(certificate.Subject, "2.5.4.97") is not null)
            {
                foreach (string str in this.Parse(certificate.Subject, "2.5.4.97"))
                {
                    signature.SignerBulstat = str;
                }
            }

            ////Subject Alternative Name
            X509ExtensionCollection x509ExtensionCollection = certificate.Extensions;
            string data = null;
            foreach (var item in x509ExtensionCollection)
            {
                if (item.Oid.FriendlyName.ToLower() == "subject alternative name")
                {
                    data = item.Format(false);
                }
            }

            if (data is not null)
            {
                if (this.Parse(data, "OID.2.5.4.3.100.1.1") is not null)
                {
                    signature.SignerBulstat = this.Parse(data, "OID.2.5.4.3.100.1.1")[0];
                }
            }

            return signature;
        }
        private List<string> Parse(string data, string delimiter)
        {
            if (data is null)
            {
                return null;
            }

            if (!delimiter.EndsWith("="))
            {
                delimiter = delimiter + "=";
            }

            if (!data.Contains(delimiter))
            {
                return null;
            }

            var result = new List<string>();

            if (data.Contains("+359"))
            {
                int start = data.IndexOf(delimiter) + delimiter.Length;
                int length = data.IndexOf(',', start) - start;
                if (length == 0)
                {
                    return null;
                }

                if (length > 0)
                {
                    result.Add(data.Substring(start, length));
                    //recurse when +
                    var rec = Parse(data.Substring(start + length), delimiter);
                    if (rec is not null)
                    {
                        result.AddRange(rec);
                    }
                }
                else
                {
                    result.Add(data.Substring(start));
                }
            }
            else if (data.Contains("+"))
            {

                int start = data.IndexOf(delimiter) + delimiter.Length;
                int length = data.IndexOf('+', start) - start;
                if (length == 0)
                {
                    return null;
                }

                if (length > 0)
                {
                    result.Add(data.Substring(start, length));
                    //recurse when ,
                    var rec = Parse(data.Substring(start + length), delimiter);
                    if (rec is not null)
                    {
                        result.AddRange(rec);
                    }
                }
                else
                {
                    result.Add(data.Substring(start));
                }

            }
            else
            {
                int start = data.IndexOf(delimiter) + delimiter.Length;
                int length = data.IndexOf(',', start) - start;
                if (length == 0)
                {
                    return null;
                }

                if (length > 0)
                {
                    result.Add(data.Substring(start, length));
                    //recurse when +
                    var rec = Parse(data.Substring(start + length), delimiter);
                    if (rec is not null)
                    {
                        result.AddRange(rec);
                    }
                }
                else
                {
                    result.Add(data.Substring(start));
                }
            }
            return result;
        }
    }
}
