namespace SB.Domain;
public record CreatePublicationCommandFile
{
    public int? BlobId { get; init; }
    public string? FileName { get; init; }
}
