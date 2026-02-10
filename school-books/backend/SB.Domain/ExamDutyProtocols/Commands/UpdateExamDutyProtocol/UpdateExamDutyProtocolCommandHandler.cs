namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamDutyProtocol> ExamDutyProtocolAggregateRepository)
    : IRequestHandler<UpdateExamDutyProtocolCommand>
{
    public async Task Handle(UpdateExamDutyProtocolCommand command, CancellationToken ct)
    {
        var protocol = await this.ExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamDutyProtocolId!.Value,
            ct);

        protocol.UpdateData(
            command.ProtocolNumber,
            command.ProtocolDate,
            command.SessionType,
            command.SubjectId!.Value,
            command.SubjectTypeId!.Value,
            command.EduFormId,
            command.ProtocolExamTypeId!.Value,
            command.ProtocolExamSubTypeId!.Value,
            command.Date!.Value,
            command.OrderNumber!,
            command.GroupNum,
            command.OrderDate!.Value,
            command.ClassIds!,
            command.SupervisorPersonIds ?? Array.Empty<int>(),
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
