namespace SB.Domain;

using System;

public class ClassBookStudentPrint
{
    // EF constructor
    private ClassBookStudentPrint()
    {
        this.ClassBook = null!;
    }

    internal ClassBookStudentPrint(
        ClassBook classBook,
        int personId,
        int createdBySysUserId)
    {
        this.ClassBook = classBook;
        this.PersonId = personId;
        this.Status = ClassBookPrintStatus.Pending;
        this.CreatedBySysUserId = createdBySysUserId;
        this.CreateDate = DateTime.Now;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int ClassBookStudentPrintId { get; private set; }

    public int PersonId { get; private set; }

    public ClassBookPrintStatus Status { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public int? BlobId { get; private set; }

    // relations
    public ClassBook ClassBook { get; private set; }

    public void MarkProcessed(int blobId)
    {
        this.Status = ClassBookPrintStatus.Processed;
        this.BlobId = blobId;
    }

    public void MarkErrored()
    {
        this.Status = ClassBookPrintStatus.Errored;
    }
}
