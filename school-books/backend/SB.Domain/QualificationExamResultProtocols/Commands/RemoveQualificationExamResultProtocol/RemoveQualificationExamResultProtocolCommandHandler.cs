namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveQualificationExamResultProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<QualificationExamResultProtocol> QualificationExamResultProtocolAggregateRepository)
    : IRequestHandler<RemoveQualificationExamResultProtocolCommand>
{
    public async Task Handle(RemoveQualificationExamResultProtocolCommand command, CancellationToken ct)
    {
        var exam = await this.QualificationExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.QualificationExamResultProtocolId!.Value,
            ct);
        this.QualificationExamResultProtocolAggregateRepository.Remove(exam);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
