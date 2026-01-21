namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateQualificationExamResultProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<QualificationExamResultProtocol> QualificationExamResultProtocolAggregateRepository)
    : IRequestHandler<CreateQualificationExamResultProtocolCommand, int>
{
    public async Task<int> Handle(CreateQualificationExamResultProtocolCommand command, CancellationToken ct)
    {
        var qualificationExamResultProtocol = new QualificationExamResultProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ProtocolNumber,
            command.ProtocolDate,
            command.SessionType,
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

        await this.QualificationExamResultProtocolAggregateRepository.AddAsync(qualificationExamResultProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return qualificationExamResultProtocol.QualificationExamResultProtocolId;
    }
}
