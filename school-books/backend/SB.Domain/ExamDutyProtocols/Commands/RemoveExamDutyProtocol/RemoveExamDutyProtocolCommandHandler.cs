namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamDutyProtocol> ExamDutyProtocolAggregateRepository)
    : IRequestHandler<RemoveExamDutyProtocolCommand>
{
    public async Task Handle(RemoveExamDutyProtocolCommand command, CancellationToken ct)
    {
        var exam = await this.ExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamDutyProtocolId!.Value,
            ct);
        this.ExamDutyProtocolAggregateRepository.Remove(exam);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
