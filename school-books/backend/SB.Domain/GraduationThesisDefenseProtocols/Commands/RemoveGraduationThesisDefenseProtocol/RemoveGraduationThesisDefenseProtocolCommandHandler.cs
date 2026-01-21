namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveGraduationThesisDefenseProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<GraduationThesisDefenseProtocol> GraduationThesisDefenseProtocolAggregateRepository)
    : IRequestHandler<RemoveGraduationThesisDefenseProtocolCommand>
{
    public async Task Handle(RemoveGraduationThesisDefenseProtocolCommand command, CancellationToken ct)
    {
        var exam = await this.GraduationThesisDefenseProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.GraduationThesisDefenseProtocolId!.Value,
            ct);
        this.GraduationThesisDefenseProtocolAggregateRepository.Remove(exam);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
