namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class ClassBookPrint
{
    // EF constructor
    private ClassBookPrint()
    {
        this.ClassBook = null!;
    }

    internal ClassBookPrint(
        ClassBook classBook,
        bool isFinal,
        int createdBySysUserId)
    {
        this.ClassBook = classBook;
        this.IsFinal = isFinal;
        this.IsSigned = false;
        this.IsExternal = false;
        this.Status = ClassBookPrintStatus.Pending;
        this.CreatedBySysUserId = createdBySysUserId;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.StatusDate = now;
    }

    internal ClassBookPrint(
        ClassBook classBook,
        int blobId,
        (string issuer,
        string subject,
        string thumbprint,
        DateTime validFrom,
        DateTime validTo)[] signatures,
        int createdBySysUserId)
    {
        this.ClassBook = classBook;
        this.IsFinal = true;
        this.IsExternal = true;
        this.IsSigned = true;
        this.Status = ClassBookPrintStatus.Processed;

        this.BlobId = blobId;
        this.signatures.AddRange(
            signatures.Select(
                (s, i) =>
                    new ClassBookPrintSignature(
                        this,
                        i,
                        s.issuer,
                        s.subject,
                        s.thumbprint,
                        s.validFrom,
                        s.validTo)));

        this.CreatedBySysUserId = createdBySysUserId;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.StatusDate = now;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int ClassBookPrintId { get; private set; }

    public ClassBookPrintStatus Status { get; private set; }

    public DateTime CreateDate { get; private set; }

    public DateTime StatusDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public int? BlobId { get; private set; }

    public string? ContentHash { get; private set; }

    public bool IsFinal { get; private set; }

    public bool IsExternal { get; private set; }

    public bool IsSigned { get; private set; }

    // relations
    public ClassBook ClassBook { get; private set; }

    private readonly List<ClassBookPrintSignature> signatures = new();
    public IReadOnlyCollection<ClassBookPrintSignature> Signatures => this.signatures.AsReadOnly();

    public void MarkProcessed(int blobId, string? contentHash)
    {
        this.Status = ClassBookPrintStatus.Processed;
        this.StatusDate = DateTime.Now;
        this.BlobId = blobId;
        this.ContentHash = contentHash;
    }

    public void MarkErrored()
    {
        this.Status = ClassBookPrintStatus.Errored;
        this.StatusDate = DateTime.Now;
    }

    public void RemoveIsFinal()
    {
        this.IsFinal = false;
    }

    public void Sign(
        int blobId,
        (string issuer,
        string subject,
        string thumbprint,
        DateTime validFrom,
        DateTime validTo)[] signatures)
    {
        if (!this.IsFinal)
        {
            throw new InvalidOperationException("Cannot sign a non-final ClassBookPrint.");
        }

        if (this.IsSigned)
        {
            throw new InvalidOperationException("Cannot sign an already signed ClassBookPrint.");
        }

        this.IsSigned = true;
        this.BlobId = blobId;
        this.signatures.AddRange(
            signatures.Select(
                (s, i) =>
                    new ClassBookPrintSignature(
                        this,
                        i,
                        s.issuer,
                        s.subject,
                        s.thumbprint,
                        s.validFrom,
                        s.validTo)));
    }
}
