namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateGradeChangeExamsAdmProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<GradeChangeExamsAdmProtocol> GradeChangeExamsAdmProtocolCommandAggregateRepository)
    : IRequestHandler<UpdateGradeChangeExamsAdmProtocolCommand, int>
{
    public async Task<int> Handle(UpdateGradeChangeExamsAdmProtocolCommand command, CancellationToken ct)
    {
        var gradeChangeExamsAdmProtocol = await this.GradeChangeExamsAdmProtocolCommandAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.GradeChangeExamsAdmProtocolId!.Value,
            ct);

        gradeChangeExamsAdmProtocol.UpdateData(
            command.ProtocolNum,
            command.ProtocolDate,
            command.CommissionMeetingDate!.Value,
            command.CommissionNominationOrderNumber!,
            command.CommissionNominationOrderDate!.Value,
            command.ExamSession,
            command.DirectorPersonId!.Value,
            command.CommissionChairman!.Value,
            command.CommissionMembers!,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);

        return gradeChangeExamsAdmProtocol.GradeChangeExamsAdmProtocolId;
    }
}
