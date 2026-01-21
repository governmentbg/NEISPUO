namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateGraduationThesisDefenseProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<GraduationThesisDefenseProtocol> GraduationThesisDefenseProtocolAggregateRepository)
    : IRequestHandler<CreateGraduationThesisDefenseProtocolCommand, int>
{
    public async Task<int> Handle(CreateGraduationThesisDefenseProtocolCommand command, CancellationToken ct)
    {
        var examDutyProtocol = new GraduationThesisDefenseProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
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

        await this.GraduationThesisDefenseProtocolAggregateRepository.AddAsync(examDutyProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return examDutyProtocol.GraduationThesisDefenseProtocolId;
    }
}
