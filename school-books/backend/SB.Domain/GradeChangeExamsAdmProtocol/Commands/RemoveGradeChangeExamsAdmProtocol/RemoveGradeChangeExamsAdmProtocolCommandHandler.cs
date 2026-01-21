namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveGradeChangeExamsAdmProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<GradeChangeExamsAdmProtocol> GradeChangeExamsAdmProtocolAggregateRepository)
    : IRequestHandler<RemoveGradeChangeExamsAdmProtocolCommand>
{
    public async Task Handle(RemoveGradeChangeExamsAdmProtocolCommand command, CancellationToken ct)
    {
        var gradeChangeExamsAdmProtocol = await this.GradeChangeExamsAdmProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.GradeChangeExamsAdmProtocolId!.Value,
            ct);

        this.GradeChangeExamsAdmProtocolAggregateRepository.Remove(gradeChangeExamsAdmProtocol);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
