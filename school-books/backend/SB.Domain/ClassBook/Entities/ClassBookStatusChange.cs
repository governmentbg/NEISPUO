namespace SB.Domain;

using System;

public class ClassBookStatusChange
{
    // EF constructor
    private ClassBookStatusChange()
    {
        this.ClassBook = null!;
    }

    internal ClassBookStatusChange(
        ClassBook classBook,
        ClassBookStatusChangeType type,
        int changedBySysUserId,
        string? signees)
    {
        this.ClassBook = classBook;
        this.Type = type;
        this.ChangeDate = DateTime.Now;
        this.ChangedBySysUserId = changedBySysUserId;
        this.Signees = signees;
        this.IsLast = true;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int ClassBookStatusChangeId { get; private set; }

    public ClassBookStatusChangeType Type { get; private set; }

    public DateTime ChangeDate { get; private set; }

    public int ChangedBySysUserId { get; private set; }

    public string? Signees { get; private set; }

    public bool IsLast { get; private set; }

    // relations
    public ClassBook ClassBook { get; private set; }

    public void RemoveIsLast()
    {
        if (!this.IsLast)
        {
            throw new InvalidOperationException("This ClassBookStatusChange is not the last.");
        }

        this.IsLast = false;
    }
}
