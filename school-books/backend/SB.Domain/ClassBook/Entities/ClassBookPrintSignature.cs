namespace SB.Domain;

using System;

public class ClassBookPrintSignature
{
    // EF constructor
    private ClassBookPrintSignature()
    {
        this.Print = null!;
        this.Issuer = null!;
        this.Subject = null!;
        this.Thumbprint = null!;
    }

    internal ClassBookPrintSignature(
        ClassBookPrint print,
        int index,
        string issuer,
        string subject,
        string thumbprint,
        DateTime validFrom,
        DateTime validTo)
    {
        this.Print = print;
        this.Index = index;
        this.Issuer = issuer;
        this.Subject = subject;
        this.Thumbprint = thumbprint;
        this.ValidFrom = validFrom;
        this.ValidTo = validTo;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int ClassBookPrintId { get; private set; }

    public int Index { get; private set; }

    public string Issuer { get; private set; }

    public string Subject { get; private set; }

    public string Thumbprint { get; private set; }

    public DateTime ValidFrom { get; private set; }

    public DateTime ValidTo { get; private set; }

    // relations
    public ClassBookPrint Print { get; private set; }
}
