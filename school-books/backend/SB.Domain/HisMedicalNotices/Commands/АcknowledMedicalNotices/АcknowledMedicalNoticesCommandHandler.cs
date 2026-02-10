namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record АcknowledMedicalNoticesCommandHandler(
    IHisMedicalNoticesQueryRepository HisMedicalNoticesQueryRepository)
    : IRequestHandler<АcknowledMedicalNoticesCommand>
{
    public async Task Handle(АcknowledMedicalNoticesCommand command, CancellationToken ct)
    {
        await this.HisMedicalNoticesQueryRepository.ExecuteAcknowledgeAsync(
            command.ExtSystemId,
            command.HisMedicalNoticeIds,
            ct);
    }
}
