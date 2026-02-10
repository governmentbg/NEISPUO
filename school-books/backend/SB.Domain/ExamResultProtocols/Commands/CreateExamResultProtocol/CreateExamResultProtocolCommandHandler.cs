namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateExamResultProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamResultProtocol> ExamResultProtocolAggregateRepository)
    : IRequestHandler<CreateExamResultProtocolCommand, int>
{
    public async Task<int> Handle(CreateExamResultProtocolCommand command, CancellationToken ct)
    {
        var examResultProtocol = new ExamResultProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ProtocolNumber,
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

        await this.ExamResultProtocolAggregateRepository.AddAsync(examResultProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return examResultProtocol.ExamResultProtocolId;
    }
}
