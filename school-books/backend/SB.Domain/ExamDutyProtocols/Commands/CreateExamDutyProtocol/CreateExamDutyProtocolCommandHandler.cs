namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamDutyProtocol> ExamDutyProtocolAggregateRepository)
    : IRequestHandler<CreateExamDutyProtocolCommand, int>
{
    public async Task<int> Handle(CreateExamDutyProtocolCommand command, CancellationToken ct)
    {
        var examDutyProtocol = new ExamDutyProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
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
            command.OrderDate!.Value,
            command.GroupNum,
            command.ClassIds ?? Array.Empty<int>(),
            command.SupervisorPersonIds ?? Array.Empty<int>(),
            command.SysUserId!.Value);

        await this.ExamDutyProtocolAggregateRepository.AddAsync(examDutyProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return examDutyProtocol.ExamDutyProtocolId;
    }
}
