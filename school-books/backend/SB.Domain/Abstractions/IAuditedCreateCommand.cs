namespace SB.Domain;

using MediatR;

public interface IAuditedCreateCommand : IRequest<int>, IAuditedCommand
{
}
