namespace SB.Domain;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveOffDayCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<OffDay> OffDayAggregateRepository,
    IClassBookOffDayDatesRepository ClassBookOffDayDatesRepository,
    IOffDayRebuildService OffDayRebuildService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveOffDayCommand>
{
    public async Task Handle(RemoveOffDayCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var offDay = await this.OffDayAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.OffDayId!.Value,
            ct);

        if (command.InstId!.Value != offDay.InstId)
        {
            // the instId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.InstId)}.");
        }

        this.OffDayAggregateRepository.Remove(offDay);

        if (await this.ClassBookOffDayDatesRepository.ExistsClassBookOffDayDateAsync(
            command.SchoolYear!.Value,
            command.OffDayId!.Value,
            ct))
        {
            await using var transaction = await this.UnitOfWork.BeginTransactionAsync(ct);

            await this.OffDayRebuildService.RebuildAndSaveAsync(
                command.SchoolYear!.Value,
                command.InstId!.Value,
                command.OffDayId!.Value,
                transaction,
                ct);

            await this.UnitOfWork.SaveAsync(ct);

            await transaction.CommitAsync(ct);
        }
        else
        {
            // rebuild escape path
            // there are no ClassBookOffDayDates depending on us
            await this.UnitOfWork.SaveAsync(ct);
        }
    }
}
