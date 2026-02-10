namespace SB.Domain;

using iTextSharp.text.exceptions;
using iTextSharp.text.io;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

public static class PdfUtils
{
    public static IEnumerable<(
        byte[] signingCertificate,
        bool coversDocument,
        bool isTimestamp,
        DateTime signDate,
        string signedRevisionContentHash)> ExtractPdfSignatures(string fileName)
    {
        var signatures =
            new List<(
                byte[] signingCertificate,
                bool coversDocument,
                bool isTimestamp,
                DateTime signDate,
                string signedRevisionContentHash)>();

        using PdfReader pdf = new(fileName);

        var signatureNames = pdf.AcroFields.GetSignatureNames();

        foreach (var signatureName in signatureNames)
        {
            bool coversDoc;
            PdfPKCS7 signature;
            string signedRevisionContentHash;

            int revision = pdf.AcroFields.GetRevision(signatureName);
            if (revision != pdf.AcroFields.TotalRevisions)
            {
                using PdfReader pdfAtRevision = new(pdf.AcroFields.ExtractRevision(signatureName));
                (coversDoc, signature) = ExtractSignature(pdfAtRevision, signatureName);
                signedRevisionContentHash = Convert.ToHexString(pdfAtRevision.GetPdfContentSha1Hash());
            }
            else
            {
                (coversDoc, signature) = ExtractSignature(pdf, signatureName);
                signedRevisionContentHash = Convert.ToHexString(pdf.GetPdfContentSha1Hash());
            }

            signatures.Add((
                signature.SigningCertificate.GetEncoded(),
                coversDoc && signature.Verify(),
                signature.IsTsp,
                // Tsp or Ltv (see https://www.mail-archive.com/itext-questions@lists.sourceforge.net/msg59504.html)
                signature.IsTsp || signature.SignDate == DateTime.MinValue
                    ? signature.TimeStampDate
                    : signature.SignDate,
                signedRevisionContentHash));
        }

        return signatures;
    }

    private static (bool coversDoc, PdfPKCS7 signature) ExtractSignature(PdfReader pdf, string signatureName)
    {
        return (
            coversDoc: pdf.AcroFields.SignatureCoversWholeDocument(signatureName),
            signature: pdf.AcroFields.VerifySignature(signatureName)
        );
    }

    public static (
        bool validAtTimeOfSigning,
        string? validationFailureChainStatus,
        string issuer,
        string subject,
        string thumbprint,
        DateTime validFrom,
        DateTime validTo) VerifyCertificate(
            byte[] signingCertificate,
            DateTime signDate)
    {
        using X509Certificate2 cert = new(signingCertificate);
        using X509Chain chain = new();
        chain.ChainPolicy.TrustMode = X509ChainTrustMode.System;
        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
        // TODO: fix this! we should check revocation status but this fails
        // in a container with this error
        // RevocationStatusUnknown: unable to get certificate CRL
        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
        chain.ChainPolicy.VerificationTime = signDate;

        bool validAtTimeOfSigning = chain.Build(cert);
        string issuer = cert.Issuer;
        string subject = cert.Subject;
        string thumbprint = cert.Thumbprint;
        DateTime validFrom = cert.NotBefore;
        DateTime validTo = cert.NotAfter;

        string? validationFailureChainStatus = null;
        if (!validAtTimeOfSigning)
        {
            validationFailureChainStatus = X509ChainToDisplayString(chain);
        }

        return (
            validAtTimeOfSigning,
            validationFailureChainStatus,
            issuer,
            subject,
            thumbprint,
            validFrom,
            validTo);
    }

    private static string X509ChainToDisplayString(X509Chain chain)
    {
        var b = new System.Text.StringBuilder();

        b.AppendLine($"[{nameof(chain.ChainPolicy.RevocationFlag)}]");
        b.AppendLine($"  {chain.ChainPolicy.RevocationFlag}");
        b.AppendLine($"[{nameof(chain.ChainPolicy.RevocationMode)}]");
        b.AppendLine($"  {chain.ChainPolicy.RevocationMode}");
        b.AppendLine($"[{nameof(chain.ChainPolicy.VerificationFlags)}]");
        b.AppendLine($"  {chain.ChainPolicy.VerificationFlags}");
        b.AppendLine($"[{nameof(chain.ChainPolicy.VerificationTime)}]");
        b.AppendLine($"  {chain.ChainPolicy.VerificationTime}");
        foreach (var (element, index) in chain.ChainElements.Cast<X509ChainElement>().Select((element, index) => (element, index)))
        {
            b.AppendLine();
            b.AppendLine($"[Element {index + 1}]");
            b.Append(element.Certificate.ToString().Replace("\n\n", "\n"));
            b.AppendLine($"[Status]");
            foreach (var status in element.ChainElementStatus)
            {
                b.AppendLine($"  {status.Status}: {status.StatusInformation}");
            }
        }

        return b.ToString();
    }

    public static byte[] GetPdfContentSha1Hash(this PdfReader pdfReader)
    {
        // This is not intended to be a secure hash, but rather a way
        // to detect changes in the PDF that may have occurred
        // due to bugs in the signing process.
        #pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms
        using SHA1 sha1 = SHA1.Create();
        #pragma warning restore CA5350 // Do Not Use Weak Cryptographic Algorithms

        for (int i = 1; i <= pdfReader.NumberOfPages; i++)
        {
            byte[] originalContent = pdfReader.GetPageContent(i);
            sha1.TransformBlock(originalContent, 0, originalContent.Length, null, 0);
        }
        sha1.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

        return sha1.Hash ?? throw new Exception("SHA1 hash should not be null");
    }

    public static string? GetPdfXmpLabelMetadata(string filepath)
    {
        using PdfReader reader = new(filepath);

        if (reader.Metadata == null || reader.Metadata.Length == 0)
        {
            return null;
        }

        string metadata = Encoding.UTF8.GetString(reader.Metadata);
        XDocument metadataXml = XDocument.Parse(metadata);
        XmlNamespaceManager xnm = new(new NameTable());
        xnm.AddNamespace("x", "adobe:ns:meta/");
        xnm.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
        xnm.AddNamespace("xmp", "http://ns.adobe.com/xap/1.0/");

        return ((IEnumerable)metadataXml.XPathEvaluate("//x:xmpmeta/rdf:RDF/rdf:Description/@xmp:Label", xnm))
            .OfType<XAttribute>()
            .FirstOrDefault()
            ?.Value;
    }

    public static bool CheckPdf(string filepath)
    {
        try
        {
            using PdfReader reader = new(filepath);

            return true;
        }
        catch (InvalidPdfException)
        {
            return false;
        }
    }

    sealed class MemoryStreamRandomAccessSource : IRandomAccessSource
    {
        private readonly MemoryStream stream;
        public MemoryStreamRandomAccessSource(MemoryStream stream)
        {
            this.stream = stream;
        }

        public long Length => this.stream.Length;

        public void Close()
        {
            this.stream.Close();
        }

        public void Dispose()
        {
            this.stream.Dispose();
        }

        public int Get(long position)
        {
            this.stream.Seek(position, SeekOrigin.Begin);
            return this.stream.ReadByte();
        }

        public int Get(long position, byte[] bytes, int off, int len)
        {
            this.stream.Seek(position, SeekOrigin.Begin);
            return this.stream.Read(bytes, off, len);
        }
    }

    private static Lazy<ConstructorInfo> PdfReaderConstructorInfo =
        new(
            () => typeof(PdfReader).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(ReaderProperties), typeof(IRandomAccessSource) },
                null)!,
            LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Creates a PdfReader without copying the memory in the stream.
    /// IMPORTANT!!! The memoryStream should be left unmodified and only disposed of
    /// after all work with this reader has been finished and disposed.
    /// </summary>
    /// <param name="memoryStream"></param>
    /// <returns></returns>
    public static PdfReader CreateFromMemoryStream(MemoryStream memoryStream)
        => (PdfReader)PdfReaderConstructorInfo.Value.Invoke(
            new object[]
            {
                new ReaderProperties(),
#pragma warning disable CA2000 // Dispose objects before losing scope
                new IndependentRandomAccessSource(
                    new MemoryStreamRandomAccessSource(memoryStream))
#pragma warning restore CA2000 // Dispose objects before losing scope
            });
}
