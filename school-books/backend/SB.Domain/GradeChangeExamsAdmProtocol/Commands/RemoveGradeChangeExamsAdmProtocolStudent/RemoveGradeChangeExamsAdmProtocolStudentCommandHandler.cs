namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveGradeChangeExamsAdmProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<GradeChangeExamsAdmProtocol> GradeChangeExamsAdmProtocolAggregateRepository)
    : IRequestHandler<RemoveGradeChangeExamsAdmProtocolStudentCommand>
{
    public async Task Handle(RemoveGradeChangeExamsAdmProtocolStudentCommand command, CancellationToken ct)
    {
        var gradeChangeExamsAdmProtocol = await this.GradeChangeExamsAdmProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.GradeChangeExamsAdmProtocolId!.Value,
            ct);

        gradeChangeExamsAdmProtocol.RemoveStudent(
            command.ClassId!.Value,
            command.PersonId!.Value,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
