namespace SB.Domain;

public interface IAggregateRoot
{
    byte[] Version { get; }
}
