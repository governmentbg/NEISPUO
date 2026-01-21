namespace SB.Domain;

public class DomainUpdateInconsistencyException : DomainException
{
    public DomainUpdateInconsistencyException(string message)
        : base(message)
    {
    }
}
