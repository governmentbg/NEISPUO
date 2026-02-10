namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateUpdateGradeChangeExamsAdmProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<GradeChangeExamsAdmProtocol> GradeChangeExamsAdmProtocolCommandAggregateRepository)
    : IRequestHandler<CreateGradeChangeExamsAdmProtocolStudentCommand, int>,
        IRequestHandler<UpdateGradeChangeExamsAdmProtocolStudentCommand, int>
{
    public async Task<int> Handle(CreateGradeChangeExamsAdmProtocolStudentCommand command, CancellationToken ct)
    {
        var gradeChangeExamsAdmProtocol = await this.GradeChangeExamsAdmProtocolCommandAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.GradeChangeExamsAdmProtocolId!.Value,
            ct);

        gradeChangeExamsAdmProtocol.AddStudent(
            command.ClassId!.Value,
            command.PersonId!.Value,
            command.Subjects!.Select(s => (subjectId: s.SubjectId!.Value, subjectTypeId: s.SubjectTypeId!.Value)).ToArray(),
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);

        return gradeChangeExamsAdmProtocol.GradeChangeExamsAdmProtocolId;
    }

    public async Task<int> Handle(UpdateGradeChangeExamsAdmProtocolStudentCommand command, CancellationToken ct)
    {
        var gradeChangeExamsAdmProtocol = await this.GradeChangeExamsAdmProtocolCommandAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.GradeChangeExamsAdmProtocolId!.Value,
            ct);

        gradeChangeExamsAdmProtocol.UpdateStudent(
            command.ClassId!.Value,
            command.PersonId!.Value,
            command.Subjects!.Select(s => (subjectId: s.SubjectId!.Value, subjectTypeId: s.SubjectTypeId!.Value)).ToArray(),
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);

        return gradeChangeExamsAdmProtocol.GradeChangeExamsAdmProtocolId;
    }
}
