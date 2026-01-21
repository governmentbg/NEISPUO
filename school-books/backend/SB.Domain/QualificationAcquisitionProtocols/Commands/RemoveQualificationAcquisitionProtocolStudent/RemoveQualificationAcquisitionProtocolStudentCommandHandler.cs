namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveQualificationAcquisitionProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<QualificationAcquisitionProtocol> QualificationAcquisitionProtocolAggregateRepository)
    : IRequestHandler<RemoveQualificationAcquisitionProtocolStudentCommand>
{
    public async Task Handle(RemoveQualificationAcquisitionProtocolStudentCommand command, CancellationToken ct)
    {
        var support = await this.QualificationAcquisitionProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.QualificationAcquisitionProtocolId!.Value,
            ct);

        support.RemoveStudent(command.ClassId!.Value, command.PersonId!.Value, command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
