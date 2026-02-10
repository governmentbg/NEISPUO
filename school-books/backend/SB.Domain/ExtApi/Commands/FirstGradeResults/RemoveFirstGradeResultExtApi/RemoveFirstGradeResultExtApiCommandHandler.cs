namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveFirstGradeResultExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IFirstGradeResultsAggregateRepository FirstGradeResultAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveFirstGradeResultExtApiCommand>
{
    public async Task Handle(RemoveFirstGradeResultExtApiCommand command, CancellationToken ct)
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

        var firstGradeResult = await this.FirstGradeResultAggregateRepository
            .FindAsync(command.SchoolYear!.Value, command.FirstGradeResultId!.Value, ct);

        if (command.ClassBookId!.Value != firstGradeResult.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        this.FirstGradeResultAggregateRepository.Remove(firstGradeResult);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
