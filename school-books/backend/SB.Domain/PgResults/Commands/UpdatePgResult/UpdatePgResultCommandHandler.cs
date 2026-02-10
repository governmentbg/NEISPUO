namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdatePgResultCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<PgResult> PgResultAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdatePgResultCommand>
{
    public async Task Handle(UpdatePgResultCommand command, CancellationToken ct)
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

        var pgResult = await this.PgResultAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.PgResultId!.Value,
            ct);

        if (command.ClassBookId!.Value != pgResult.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        pgResult.UpdateData(
            command.StartSchoolYearResult,
            command.EndSchoolYearResult,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
