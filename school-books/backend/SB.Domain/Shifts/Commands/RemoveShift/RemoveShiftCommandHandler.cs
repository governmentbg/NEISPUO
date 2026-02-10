namespace SB.Domain;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveShiftCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Shift> ShiftAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveShiftCommand>
{
    public async Task Handle(RemoveShiftCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var shift = await this.ShiftAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ShiftId!.Value,
            ct);

        if (command.InstId!.Value != shift.InstId)
        {
            // the instId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.InstId)}.");
        }

        if (shift.IsAdhoc)
        {
            throw new DomainValidationException("Cannot remove adhoc shift");
        }

        this.ShiftAggregateRepository.Remove(shift);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
