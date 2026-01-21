namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveExamDutyProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamDutyProtocol> ExamDutyProtocolAggregateRepository)
    : IRequestHandler<RemoveExamDutyProtocolStudentCommand>
{
    public async Task Handle(RemoveExamDutyProtocolStudentCommand command, CancellationToken ct)
    {
        var support = await this.ExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamDutyProtocolId!.Value,
            ct);

        support.RemoveStudent(command.ClassId!.Value, command.PersonId!.Value, command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
