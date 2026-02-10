namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateQualificationExamResultProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<QualificationExamResultProtocol> QualificationExamResultProtocolAggregateRepository)
    : IRequestHandler<UpdateQualificationExamResultProtocolCommand>
{
    public async Task Handle(UpdateQualificationExamResultProtocolCommand command, CancellationToken ct)
    {
        var protocol = await this.QualificationExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.QualificationExamResultProtocolId!.Value,
            ct);

        protocol.UpdateData(
            command.ProtocolNumber,
            command.ProtocolDate,
            command.SessionType!,
            command.Profession!,
            command.Speciality!,
            command.QualificationDegreeId!.Value,
            command.GroupNum,
            command.ClassIds!,
            command.EduFormId,
            command.QualificationExamTypeId!.Value,
            command.Date!.Value,
            command.CommissionNominationOrderNumber!,
            command.CommissionNominationOrderDate!.Value,
            command.CommissionChairman!.Value,
            command.CommissionMembers!,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
