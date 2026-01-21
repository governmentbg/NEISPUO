namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class Publication : IAggregateRoot
{
    // EF constructor
    private Publication()
    {
        this.Title = null!;
        this.Content = null!;
        this.Version = null!;
    }

    public Publication(
        int schoolYear,
        int instId,
        PublicationType type,
        string title,
        string content,
        DateTime date,
        (int blobId, string fileName)[] files,
        int createdBySysUserId)
        : this()
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.Type = type;
        this.Status = PublicationStatus.Draft;
        this.Title = title;
        this.Content = content;
        this.Date = date;
        this.files.AddRange(files.Select(f => new PublicationFile(this, f.blobId, f.fileName)));

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int PublicationId { get; private set; }

    public int InstId { get; private set; }

    public PublicationType Type { get; private set; }

    public PublicationStatus Status { get; private set; }

    public DateTime Date { get; private set; }

    public string Title { get; private set; }

    public string Content { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<PublicationFile> files = new();
    public IReadOnlyCollection<PublicationFile> Files => this.files.AsReadOnly();

    public void Update(
        PublicationType type,
        string title,
        string content,
        DateTime date,
        (int blobId, string fileName)[] files,
        int modifiedBySysUserId)
    {
        this.AssertIsDraft();

        this.Type = type;
        this.Title = title;
        this.Content = content;
        this.Date = date;

        this.files.Clear();
        this.files.AddRange(files.Select(f => new PublicationFile(this, f.blobId, f.fileName)));

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void ChangeStatus(PublicationStatus status, int modifiedBySysUserId)
    {
        switch (status)
        {
            case PublicationStatus.Draft:
                if (this.Status == PublicationStatus.Draft)
                    throw new DomainUpdateInconsistencyException("Status transition not allowed");
                this.Status = PublicationStatus.Draft;
                break;
            case PublicationStatus.Published:
                if (this.Status != PublicationStatus.Draft)
                    throw new DomainUpdateInconsistencyException("Status transition not allowed");
                this.Status = PublicationStatus.Published;
                break;
            case PublicationStatus.Archived:
                if (this.Status != PublicationStatus.Published)
                    throw new DomainUpdateInconsistencyException("Status transition not allowed");
                this.Status = PublicationStatus.Archived;
                break;
            default:
                throw new ArgumentException("Invalid publication status");
        }

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    private void AssertIsDraft()
    {
        if (this.Status != PublicationStatus.Draft)
        {
            throw new DomainUpdateInconsistencyException("Cannot edit publication that is not in Draft status");
        }
    }
}
