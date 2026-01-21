namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveQualificationExamResultProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<QualificationExamResultProtocol> QualificationExamResultProtocolAggregateRepository)
    : IRequestHandler<RemoveQualificationExamResultProtocolStudentCommand>
{
    public async Task Handle(RemoveQualificationExamResultProtocolStudentCommand command, CancellationToken ct)
    {
        var support = await this.QualificationExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.QualificationExamResultProtocolId!.Value,
            ct);

        support.RemoveStudent(command.ClassId!.Value, command.PersonId!.Value, command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
