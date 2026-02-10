namespace MON.Services.Implementations
{
    using MON.Models.Certificate;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Xml;
    using System.Xml.Serialization;

    public class CertificateExtensions
    {
        public const string XadesNamespaceUrl = "http://uri.etsi.org/01903/v1.3.2#";

        public static string SignXml(string xml, X509Certificate2 cert)
        {
            XmlDocument xmlDoc = new XmlDocument
            {

                // Load an XML file into the XmlDocument object.
                PreserveWhitespace = true
            };
            xmlDoc.LoadXml(xml);

            // Sign the XML document. 
            SignXml(xmlDoc, cert);

            // Save the document.
            return xmlDoc.OuterXml;
        }

        public static void SignXml(X509Certificate2 cert, string xmlToSign, string signedXmlFileName)
        {
            XmlDocument xmlDoc = new XmlDocument
            {

                // Load an XML file into the XmlDocument object.
                PreserveWhitespace = true
            };
            xmlDoc.Load(xmlToSign);

            // Sign the XML document. 
            SignXml(xmlDoc, cert);

            // Save the document.
            xmlDoc.Save(signedXmlFileName);
        }

        public static void SignXml(X509Certificate2 cert, string xmlToSign, string signedXml, RSA Key)
        {
            XmlDocument xmlDoc = new XmlDocument
            {

                // Load an XML file into the XmlDocument object.
                PreserveWhitespace = true
            };
            xmlDoc.Load(xmlToSign);

            // Sign the XML document. 
            SignXml(xmlDoc, cert, Key);

            // Save the document.
            xmlDoc.Save(signedXml);
        }

        public static void SignXml(XmlDocument aDoc, X509Certificate2 cert)
        {
            SignXml(aDoc, cert, "Invoice", (RSA)cert.PrivateKey);
        }

        public static void SignXml(XmlDocument aDoc, X509Certificate2 cert, RSA Key)
        {
            SignXml(aDoc, cert, "Invoice", Key);
        }


        public static void SignXml(XmlDocument aDoc, X509Certificate2 cert, string parentNode)
        {
            SignXml(aDoc, cert, parentNode, (RSA)cert.PrivateKey);
        }

        private static string DateTimeToCanonicalRepresentation()
        {
            var ahora = DateTime.Now.ToUniversalTime();
            return ahora.Year.ToString("0000") + "-" + ahora.Month.ToString("00") +
                   "-" + ahora.Day.ToString("00") +
                   "T" + ahora.Hour.ToString("00") + ":" +
                   ahora.Minute.ToString("00") + ":" + ahora.Second.ToString("00") +
                   "Z";
        }

        public static void SignXml(XmlDocument aDoc, X509Certificate2 cert, string parentNode, RSA Key)
        {
            // Check arguments.
            if (aDoc == null)
                throw new ArgumentException("Doc");

            // Create a SignedXml object.
            SignaturePropertiesSignedXml signedXml = new SignaturePropertiesSignedXml(aDoc, "SignatureID", "idSignedProperties")
            {

                // Add the key to the SignedXml document.
                SigningKey = Key
            };

            // Create a reference to be signed.
            Reference reference = new Reference
            {
                Uri = ""
            };

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            //------------------------
            // create a timestamp property
            XmlElement timestamp = aDoc.CreateElement("SigningTime", SignedXml.XmlDsigNamespaceUrl);
            timestamp.InnerText = DateTimeToCanonicalRepresentation();
            signedXml.AddProperty(timestamp);

            Reference reference2 = new Reference
            {
                Uri = "#" + "idSignedProperties"
            };
            //reference2.DigestMethod = @"http://www.w3.org/2001/04/xmlenc#sha256";
            reference2.AddTransform(new XmlDsigExcC14NTransform());
            signedXml.AddReference(reference2);

            //DataObject data = new DataObject();
            //data.Id = "#SignedProperty";

            //QualifyingPropertiesType q = new QualifyingPropertiesType();
            //q.SignedProperties = new SignedPropertiesType();
            //q.SignedProperties.SignedSignatureProperties = new SignedSignaturePropertiesType();
            //q.SignedProperties.SignedSignatureProperties.SigningTime = DateTime.Now.ToUniversalTime();
            //q.SignedProperties.SignedSignatureProperties.SigningTimeSpecified = true;
            //XmlSerializer xSerializer = new XmlSerializer(typeof(QualifyingPropertiesType));
            //MemoryStream ms = new MemoryStream();

            //XmlWriter xw = XmlTextWriter.Create(ms, new XmlWriterSettings() { Encoding = Encoding.UTF8 });

            ////XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //xSerializer.Serialize(xw, q);
            //xw.Close();

            //ms.Seek(0, SeekOrigin.Begin);
            //byte[] buffer = new byte[ms.Length];
            //ms.Read(buffer, 0, (int)ms.Length);

            //byte[] bom = new byte[] { 0xEF, 0xBB, 0xBF };

            //var t = Encoding.UTF8.GetString(buffer, 3, buffer.Length-3);

            //XmlDocument d = new XmlDocument();
            //d.LoadXml(t);

            //// remove xml declaration
            //d.RemoveChild(d.ChildNodes[0]);
            //data.Data =  d.ChildNodes;
            //signedXml.AddObject(data);

            //// Create a reference to be able to package everything into the
            //// message.
            //Reference reference2= new Reference();
            //reference2.Type = "http://uri.etsi.org/01903/v1.1.1#SignedProperties";
            //reference2.Uri = "#SignedProperty";

            //// Add an enveloped transformation to the reference.
            //reference2.AddTransform(new XmlDsigExcC14NTransform());

            //// Add it to the message.

            //------------------------

            // Compute the signature.
            signedXml.ComputeSignature();

            KeyInfo keyInfo = new KeyInfo();

            KeyInfoName KeyName = new KeyInfoName
            {
                Value = cert.IssuerName.Name.ToString()
            };

            keyInfo.AddClause(KeyName);

            keyInfo.AddClause(new RSAKeyValue(Key));

            keyInfo.AddClause(new KeyInfoX509Data(cert, X509IncludeOption.EndCertOnly));

            signedXml.KeyInfo = keyInfo;

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            aDoc.DocumentElement.AppendChild(aDoc.ImportNode(xmlDigitalSignature, true));

            //XmlNodeList invoice = aDoc.GetElementsByTagName(parentNode);
            //XmlNode invoiceNode = invoice[0];

            //invoiceNode.InsertBefore(xmlDigitalSignature, invoiceNode.FirstChild);

            //aDoc = invoiceNode.OwnerDocument;

            // Append the element to the XML document.
            //aDoc.DocumentElement.AppendChild(aDoc.ImportNode(xmlDigitalSignature, true));
        }

        public static Boolean VerifyXmlString(string xml)
        {
            // Create a new XML document.
            XmlDocument xmlDoc = new XmlDocument
            {

                // Load an XML file into the XmlDocument object.
                PreserveWhitespace = true
            };
            xmlDoc.LoadXml(xml);

            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDoc);

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception
            // if more than one signature was found.
            if (nodeList.Count >= 2)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }

            // Load the first <signature> node.
            signedXml.LoadXml((XmlElement)nodeList[0]);

            return signedXml.CheckSignature();
        }

        public static Boolean VerifyXml(string fileName)
        {
            // Create a new XML document.
            XmlDocument xmlDoc = new XmlDocument
            {

                // Load an XML file into the XmlDocument object.
                PreserveWhitespace = true
            };
            xmlDoc.Load(fileName);

            XmlReader xReader = XmlReader.Create(fileName);


            XmlSerializer xSerializer = new XmlSerializer(typeof(ISignedXML));
            ISignedXML inv = (ISignedXML)xSerializer.Deserialize(xReader);
            xReader.Close();

            return VerifyXml(xmlDoc, inv);
        }

        public static X509Certificate2 ExtractCertificate(string xml)
        {
            // Create a new XML document.
            XmlDocument xmlDoc = new XmlDocument
            {

                // Load an XML file into the XmlDocument object.
                PreserveWhitespace = true
            };
            xmlDoc.LoadXml(xml);

            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDoc);

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception
            // if more than one signature was found.
            if (nodeList.Count >= 2)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }

            // Load the first <signature> node.
            signedXml.LoadXml((XmlElement)nodeList[0]);

            XmlSerializer xSerializer = new XmlSerializer(typeof(SignatureType));
            using TextReader reader = new StringReader(nodeList[0].OuterXml);
            SignatureType st = (SignatureType)xSerializer.Deserialize(reader);

            if (st == null)
            {
                return null;
            }

            X509Certificate2 certificate = new X509Certificate2();
            foreach (object clause in st.KeyInfo.Items)
            {
                if (clause is X509DataType type)
                {
                    if (type.Items.Count() > 0)
                    {
                        for (int i = 0; i < type.Items.Count(); i++)
                        {
                            if (type.ItemsElementName[i] == ItemsChoiceType.X509Certificate)
                            {
                                certificate = new X509Certificate2((byte[])(type.Items[i]));
                            }
                        }

                    }
                }
            }

            return certificate;
        }

        public static bool VerifyXml(XmlDocument aDoc, ISignedXML aXmlSchemaObject)
        {
            // Check arguments.
            if (aDoc == null)
                throw new ArgumentException("Doc");
            if (aXmlSchemaObject == null)
                throw new ArgumentException("XmlSchemaObject");

            SignatureType st = aXmlSchemaObject.Signature;

            if (st == null)
            {
                return false;
            }

            X509Certificate2 X5091 = new X509Certificate2();
            foreach (object clause in aXmlSchemaObject.Signature.KeyInfo.Items)
            {
                if (clause is X509DataType type)
                {
                    if (type.Items.Count() > 0)
                    {
                        for (int i = 0; i < type.Items.Count(); i++)
                        {
                            if (type.ItemsElementName[i] == ItemsChoiceType.X509Certificate)
                            {
                                X5091 = new X509Certificate2((byte[])(type.Items[i]));
                            }
                        }

                    }
                }
            }

            //            X509Certificate2 X5091 = new X509Certificate2((byte[])((X509DataType)aXmlSchemaObject.Signature.KeyInfo.Items[2]).Items[0]);

            return VerifyXml(aDoc, X5091, true);
        }



        public static Boolean VerifyXml(XmlDocument aDoc, X509Certificate2 cert, Boolean aVerifySignatureOnly)
        {
            // Check arguments.
            if (aDoc == null)
                throw new ArgumentException("Doc");
            if (cert == null)
                throw new ArgumentException("cert");

            SignedXml signedXml = new SignedXml(aDoc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = aDoc.GetElementsByTagName("Signature");

            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception 
            // if more than one signature was found.
            if (nodeList.Count >= 2)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }

            // Load the first <signature> node.  
            signedXml.LoadXml((XmlElement)nodeList[0]);

            // Check the signature and return the result.

            return signedXml.CheckSignature(cert, aVerifySignatureOnly);
        }

        public static bool VerifyXml(string fileName, X509Certificate2 cert, Boolean aVerifySignatureOnly)
        {
            XmlDocument xmlDoc = new XmlDocument
            {

                // Load an XML file into the XmlDocument object.
                PreserveWhitespace = true
            };
            xmlDoc.Load(fileName);

            // Check arguments.
            if (xmlDoc == null)
                throw new ArgumentException("Doc");
            if (cert == null)
                throw new ArgumentException("cert");

            SignedXml signedXml = new SignedXml(xmlDoc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");

            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception 
            // if more than one signature was found.
            if (nodeList.Count >= 2)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }

            // Load the first <signature> node.  
            signedXml.LoadXml((XmlElement)nodeList[0]);

            // Check the signature and return the result.

            return signedXml.CheckSignature(cert, aVerifySignatureOnly);
        }

        private static bool VerifyCertificate(byte[] primaryCertificate, IEnumerable<byte[]> additionalCertificates)
        {
            var chain = new X509Chain();
            foreach (var cert in additionalCertificates.Select(x => new X509Certificate2(x)))
            {
                chain.ChainPolicy.ExtraStore.Add(cert);
            }

            // You can alter how the chain is built/validated.
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreWrongUsage;

            // Do the validation.
            var primaryCert = new X509Certificate2(primaryCertificate);
            return chain.Build(primaryCert);
        }
    }

    public sealed class SignaturePropertiesSignedXml : SignedXml
    {
        private readonly XmlDocument doc;
        private readonly XmlElement signaturePropertiesRoot;
        private readonly XmlElement qualifyingPropertiesRoot;

        private readonly string signaturePropertiesId;

        public SignaturePropertiesSignedXml(XmlDocument doc)
            : base(doc)
        {
            return;
        }

        public SignaturePropertiesSignedXml(XmlDocument doc, string signatureId, string propertiesId)
            : base(doc)
        {
            this.signaturePropertiesId = propertiesId;
            this.doc = null;
            this.signaturePropertiesRoot = null;
            if (string.IsNullOrEmpty(signatureId))
            {
                throw new ArgumentException("signatureId cannot be empty", "signatureId");
            }
            if (string.IsNullOrEmpty(propertiesId))
            {
                throw new ArgumentException("propertiesId cannot be empty", "propertiesId");
            }

            this.doc = doc;
            base.Signature.Id = signatureId;

            this.qualifyingPropertiesRoot = doc.CreateElement("QualifyingProperties", "http://www.w3.org/2000/09/xmldsig#");
            this.qualifyingPropertiesRoot.SetAttribute("Target", "#" + signatureId);

            this.signaturePropertiesRoot = doc.CreateElement("SignedProperties", "http://www.w3.org/2000/09/xmldsig#");
            this.signaturePropertiesRoot.SetAttribute("Id", propertiesId);


            qualifyingPropertiesRoot.AppendChild(signaturePropertiesRoot);
            DataObject dataObject = new DataObject
            {
                Data = qualifyingPropertiesRoot.SelectNodes("."),
                Id = "idObject"
            };
            AddObject(dataObject);


        }

        public void AddProperty(XmlElement content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            XmlElement newChild = this.doc.CreateElement("SignedSignatureProperties", "http://www.w3.org/2000/09/xmldsig#");

            newChild.AppendChild(content);
            this.signaturePropertiesRoot.AppendChild(newChild);
        }

        public override XmlElement GetIdElement(XmlDocument doc, string id)
        {
            if (string.Compare(id, signaturePropertiesId, StringComparison.OrdinalIgnoreCase) == 0)
                return this.signaturePropertiesRoot;

            if (string.Compare(id, this.KeyInfo.Id, StringComparison.OrdinalIgnoreCase) == 0)
                return this.KeyInfo.GetXml();

            return base.GetIdElement(doc, id);
        }
    }
}
