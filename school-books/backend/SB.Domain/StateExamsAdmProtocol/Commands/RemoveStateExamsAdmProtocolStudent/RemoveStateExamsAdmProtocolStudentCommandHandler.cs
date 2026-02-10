namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveStateExamsAdmProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<StateExamsAdmProtocol> StateExamsAdmProtocolAggregateRepository)
    : IRequestHandler<RemoveStateExamsAdmProtocolStudentCommand>
{
    public async Task Handle(RemoveStateExamsAdmProtocolStudentCommand command, CancellationToken ct)
    {
        var stateExamsAdmProtocol = await this.StateExamsAdmProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.StateExamsAdmProtocolId!.Value,
            ct);

        stateExamsAdmProtocol.RemoveStudent(
            command.ClassId!.Value,
            command.PersonId!.Value,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
