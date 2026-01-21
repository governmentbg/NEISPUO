namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveExamResultProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamResultProtocol> ExamResultProtocolAggregateRepository)
    : IRequestHandler<RemoveExamResultProtocolStudentCommand>
{
    public async Task Handle(RemoveExamResultProtocolStudentCommand command, CancellationToken ct)
    {
        var support = await this.ExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamResultProtocolId!.Value,
            ct);

        support.RemoveStudent(command.ClassId!.Value, command.PersonId!.Value, command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
