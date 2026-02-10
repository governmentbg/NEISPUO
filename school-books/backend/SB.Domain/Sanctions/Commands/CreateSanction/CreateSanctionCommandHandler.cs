namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateSanctionCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Sanction> SanctionAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateSanctionCommand, int>
{
    public async Task<int> Handle(CreateSanctionCommand command, CancellationToken ct)
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

        if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.PersonId!.Value,
            ct))
        {
            throw new DomainValidationException("This person is not in the class book students list");
        }

        var sanction = new Sanction(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.PersonId!.Value,
            command.SanctionTypeId!.Value,
            command.OrderNumber!,
            command.OrderDate!.Value,
            command.StartDate!.Value,
            command.EndDate,
            command.Description,
            command.CancelOrderNumber,
            command.CancelOrderDate,
            command.CancelReason,
            command.RuoOrderNumber,
            command.RuoOrderDate,
            command.SysUserId!.Value);

        await this.SanctionAggregateRepository.AddAsync(sanction, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return sanction.SanctionId;
    }
}
