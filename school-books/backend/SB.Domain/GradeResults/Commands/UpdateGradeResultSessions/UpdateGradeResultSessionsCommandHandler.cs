namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateGradeResultSessionsCommandHandler(
    IUnitOfWork UnitOfWork,
    IGradeResultsAggregateRepository GradeResultAggregateRepository,
    IGradeResultsQueryRepository GradeResultsQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateGradeResultSessionsCommand>
{
    public async Task Handle(UpdateGradeResultSessionsCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{command.SchoolYear!.Value}, classBookId:{command.ClassBookId!.Value}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        var classBookGradeResults = (
            await this.GradeResultAggregateRepository.FindAllByClassBookIdAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct)
        ).ToDictionary(gr => gr.PersonId, gr => gr);

        foreach (var session in command.Sessions!)
        {
            var gr = classBookGradeResults[session.PersonId!.Value];

            gr.SetSubjectResult(
                session.CurriculumId!.Value,
                session.Session1Grade,
                session.Session1NoShow,
                session.Session2Grade,
                session.Session2NoShow,
                session.Session3Grade,
                session.Session3NoShow,
                command.SysUserId!.Value);
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
