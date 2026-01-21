namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateGraduationThesisDefenseProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<GraduationThesisDefenseProtocol> GraduationThesisDefenseProtocolAggregateRepository)
    : IRequestHandler<UpdateGraduationThesisDefenseProtocolCommand>
{
    public async Task Handle(UpdateGraduationThesisDefenseProtocolCommand command, CancellationToken ct)
    {
        var protocol = await this.GraduationThesisDefenseProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.GraduationThesisDefenseProtocolId!.Value,
            ct);

        protocol.UpdateData(
            command.ProtocolNumber,
            command.ProtocolDate,
            command.SessionType,
            command.EduFormId,
            command.CommissionMeetingDate!.Value,
            command.DirectorOrderNumber!,
            command.DirectorOrderDate!.Value,
            command.DirectorPersonId!.Value,
            command.CommissionChairman!.Value,
            command.CommissionMembers!,
            command.Section1StudentsCapacity!.Value!,
            command.Section2StudentsCapacity!.Value!,
            command.Section3StudentsCapacity!.Value!,
            command.Section4StudentsCapacity!.Value!,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
