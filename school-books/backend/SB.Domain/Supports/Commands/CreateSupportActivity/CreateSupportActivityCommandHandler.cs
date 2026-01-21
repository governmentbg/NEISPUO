namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateSupportActivityCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Support> SupportAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateSupportActivityCommand>
{
    public async Task Handle(CreateSupportActivityCommand command, CancellationToken ct)
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

        var support = await this.SupportAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SupportId!.Value,
            ct);

        if (command.ClassBookId!.Value != support.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        support.AddActivity(
            command.SupportActivityTypeId!.Value,
            command.Target,
            command.Result,
            command.Date,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
