namespace SB.Domain;

using MediatR;

public interface IAuditedCreateMultipleCommand : IRequest<int[]>, IAuditedCommand
{
}
