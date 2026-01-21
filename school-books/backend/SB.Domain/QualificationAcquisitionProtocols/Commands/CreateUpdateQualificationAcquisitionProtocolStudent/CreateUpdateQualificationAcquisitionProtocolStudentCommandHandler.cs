namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateUpdateQualificationAcquisitionProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<QualificationAcquisitionProtocol> QualificationAcquisitionProtocolCommandAggregateRepository)
    : IRequestHandler<CreateQualificationAcquisitionProtocolStudentCommand>, IRequestHandler<UpdateQualificationAcquisitionProtocolStudentCommand>
{
    public async Task Handle(CreateQualificationAcquisitionProtocolStudentCommand command, CancellationToken ct)
    {
        var stateExamsAdmProtocol = await this.QualificationAcquisitionProtocolCommandAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.QualificationAcquisitionProtocolId!.Value,
            ct);

        stateExamsAdmProtocol.AddStudent(
            command.ClassId!.Value,
            command.PersonId!.Value,
            command.ExamsPassed!.Value,
            command.TheoryPoints,
            command.PracticePoints,
            command.AverageDecimalGrade,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }

    public async Task Handle(UpdateQualificationAcquisitionProtocolStudentCommand command, CancellationToken ct)
    {
        var qualificationAcquisitionProtocol = await this.QualificationAcquisitionProtocolCommandAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.QualificationAcquisitionProtocolId!.Value,
            ct);

        qualificationAcquisitionProtocol.UpdateStudent(
            command.ClassId!.Value,
            command.PersonId!.Value,
            command.ExamsPassed!.Value,
            command.TheoryPoints,
            command.PracticePoints,
            command.AverageDecimalGrade,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
