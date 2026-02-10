namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateSanctionCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Sanction> SanctionAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateSanctionCommand>
{
    public async Task Handle(UpdateSanctionCommand command, CancellationToken ct)
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

        var sanction = await this.SanctionAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SanctionId!.Value,
            ct);

        if (command.ClassBookId!.Value != sanction.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        sanction.UpdateData(
            command.SanctionTypeId!.Value,
            command.OrderNumber!,
            command.OrderDate!.Value,
            command.StartDate!.Value,
            command.EndDate,
            command.Description!,
            command.CancelOrderNumber,
            command.CancelOrderDate,
            command.CancelReason,
            command.RuoOrderNumber,
            command.RuoOrderDate,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
