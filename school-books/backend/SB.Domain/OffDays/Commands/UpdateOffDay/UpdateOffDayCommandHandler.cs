namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateOffDayCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<OffDay> OffDayAggregateRepository,
    IClassBookOffDayDatesRepository ClassBookOffDayDatesRepository,
    IOffDayRebuildService OffDayRebuildService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateOffDayCommand, int>
{
    public async Task<int> Handle(UpdateOffDayCommand command, CancellationToken ct)
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

        var basicClassIds = command.BasicClassIds ?? Array.Empty<int>();
        var classBookIds = command.ClassBookIds ?? Array.Empty<int>();

        bool basicClassIdsChanged =
            offDay.Classes
            .Select(c => c.BasicClassId)
            .ToHashSet()
            .SetEquals(basicClassIds) == false;

        bool classBookIdsChanged =
            offDay.ClassBooks
            .Select(c => c.ClassBookId)
            .ToHashSet()
            .SetEquals(classBookIds) == false;

        bool datesChanged =
            offDay.From != command.From!.Value
            || offDay.To != command.To!.Value;

        offDay.UpdateData(
            command.From!.Value,
            command.To!.Value,
            command.Description!,
            command.IsForAllClasses!.Value,
            basicClassIds,
            classBookIds, // TODO verify classbooks belong to institution
            command.IsPgOffProgramDay!.Value,
            command.SysUserId!.Value);

        await using var transaction = await this.UnitOfWork.BeginTransactionAsync(ct);

        await this.UnitOfWork.SaveAsync(ct);

        if (basicClassIdsChanged || classBookIdsChanged || datesChanged)
        {
            await this.OffDayRebuildService.RebuildAndSaveAsync(
                command.SchoolYear!.Value,
                command.InstId!.Value,
                null,
                transaction,
                ct);
        }
        else
        {
            // rebuild escape path
            // classes and dates have not changed and we can go with a simple update
            await this.ClassBookOffDayDatesRepository.UpdateClassBookOffDayDatesAsync(
                command.SchoolYear!.Value,
                command.OffDayId!.Value,
                ct);
        }

        await transaction.CommitAsync(ct);

        return offDay.OffDayId;
    }
}
