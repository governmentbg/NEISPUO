namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateExamResultProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamResultProtocol> ExamResultProtocolAggregateRepository)
    : IRequestHandler<UpdateExamResultProtocolCommand>
{
    public async Task Handle(UpdateExamResultProtocolCommand command, CancellationToken ct)
    {
        var protocol = await this.ExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamResultProtocolId!.Value,
            ct);

        protocol.UpdateData(
            command.ProtocolNumber!,
            command.ProtocolDate,
            command.SessionType,
            command.SubjectId!.Value,
            command.SubjectTypeId!.Value,
            command.GroupNum,
            command.ClassIds!,
            command.EduFormId,
            command.ProtocolExamTypeId!.Value,
            command.ProtocolExamSubTypeId!.Value,
            command.Date!.Value,
            command.CommissionNominationOrderNumber!,
            command.CommissionNominationOrderDate!.Value,
            command.CommissionChairman!.Value,
            command.CommissionMembers!,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
