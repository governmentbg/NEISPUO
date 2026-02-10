namespace SB.Domain;
public class PublicationFile
{
    // EF constructor
    private PublicationFile()
    {
        this.FileName = null!;
        this.Publication = null!;
    }

    internal PublicationFile(
        Publication publication,
        int blobId,
        string fileName)
    {
        this.PublicationId = publication.PublicationId;
        this.BlobId = blobId;
        this.FileName = fileName;
        this.Publication = publication;
    }

    public int SchoolYear { get; private set; }

    public int PublicationId { get; private set; }

    public int BlobId { get; private set; }

    public string FileName { get; private set; }

    // relations
    public Publication Publication { get; private set; }
}
