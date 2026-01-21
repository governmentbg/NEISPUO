namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateGradeChangeExamsAdmProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<GradeChangeExamsAdmProtocol> GradeChangeExamsAdmProtocolCommandAggregateRepository)
    : IRequestHandler<CreateGradeChangeExamsAdmProtocolCommand, int>
{
    public async Task<int> Handle(CreateGradeChangeExamsAdmProtocolCommand command, CancellationToken ct)
    {
        var gradeChangeExamsAdmProtocol = new GradeChangeExamsAdmProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
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

        await this.GradeChangeExamsAdmProtocolCommandAggregateRepository.AddAsync(gradeChangeExamsAdmProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return gradeChangeExamsAdmProtocol.GradeChangeExamsAdmProtocolId;
    }
}
