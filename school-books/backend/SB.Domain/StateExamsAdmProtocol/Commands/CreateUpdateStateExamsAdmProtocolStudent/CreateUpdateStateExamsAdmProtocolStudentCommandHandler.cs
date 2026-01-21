namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateUpdateStateExamsAdmProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<StateExamsAdmProtocol> StateExamsAdmProtocolCommandAggregateRepository)
    : IRequestHandler<CreateStateExamsAdmProtocolStudentCommand>, IRequestHandler<UpdateStateExamsAdmProtocolStudentCommand>
{
    public async Task Handle(CreateStateExamsAdmProtocolStudentCommand command, CancellationToken ct)
    {
        var stateExamsAdmProtocol = await this.StateExamsAdmProtocolCommandAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.StateExamsAdmProtocolId!.Value,
            ct);

        stateExamsAdmProtocol.AddStudent(
            command.ClassId!.Value,
            command.PersonId!.Value,
            command.HasFirstMandatorySubject!.Value,
            command.SecondMandatorySubject?.SubjectId,
            command.SecondMandatorySubject?.SubjectTypeId,
            command.AdditionalSubjects!.Select(s => (subjectId: s.SubjectId!.Value, subjectTypeId: s.SubjectTypeId!.Value)).ToArray(),
            command.QualificationSubjects!.Select(s => (subjectId: s.SubjectId!.Value, subjectTypeId: s.SubjectTypeId!.Value)).ToArray(),
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }

    public async Task Handle(UpdateStateExamsAdmProtocolStudentCommand command, CancellationToken ct)
    {
        var stateExamsAdmProtocol = await this.StateExamsAdmProtocolCommandAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.StateExamsAdmProtocolId!.Value,
            ct);

        stateExamsAdmProtocol.UpdateStudent(
            command.ClassId!.Value,
            command.PersonId!.Value,
            command.HasFirstMandatorySubject!.Value,
            command.SecondMandatorySubject?.SubjectId,
            command.SecondMandatorySubject?.SubjectTypeId,
            command.AdditionalSubjects!.Select(s => (subjectId: s.SubjectId!.Value, subjectTypeId: s.SubjectTypeId!.Value)).ToArray(),
            command.QualificationSubjects!.Select(s => (subjectId: s.SubjectId!.Value, subjectTypeId: s.SubjectTypeId!.Value)).ToArray(),
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
