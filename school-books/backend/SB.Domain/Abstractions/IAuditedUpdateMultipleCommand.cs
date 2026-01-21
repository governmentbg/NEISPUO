namespace SB.Domain;

using MediatR;

public interface IAuditedUpdateMultipleCommand : IRequest, IAuditedCommand
{
    int[] ObjectIds { get; }
}
