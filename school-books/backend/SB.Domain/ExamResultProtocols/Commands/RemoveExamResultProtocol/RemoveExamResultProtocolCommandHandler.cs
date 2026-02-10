namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveExamResultProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamResultProtocol> ExamResultProtocolAggregateRepository)
    : IRequestHandler<RemoveExamResultProtocolCommand>
{
    public async Task Handle(RemoveExamResultProtocolCommand command, CancellationToken ct)
    {
        var exam = await this.ExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamResultProtocolId!.Value,
            ct);
        this.ExamResultProtocolAggregateRepository.Remove(exam);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
