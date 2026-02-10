namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateOffDayCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<OffDay> OffDayAggregateRepository,
    IOffDayRebuildService OffDayRebuildService,
    ISchedulesAggregateRepository SchedulesAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateOffDayCommand, int>
{
    public async Task<int> Handle(CreateOffDayCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var offDay = new OffDay(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.From!.Value,
            command.To!.Value,
            command.Description!,
            command.IsForAllClasses!.Value,
            command.BasicClassIds ?? Array.Empty<int>(),
            command.ClassBookIds ?? Array.Empty<int>(), // TODO verify classbooks belong to institution
            command.IsPgOffProgramDay!.Value,
            command.SysUserId!.Value);

        await this.OffDayAggregateRepository.AddAsync(offDay, ct);

        await using var transaction = await this.UnitOfWork.BeginTransactionAsync(ct);

        await this.UnitOfWork.SaveAsync(ct);

        await this.OffDayRebuildService.RebuildAndSaveAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            null,
            transaction,
            ct);

        await transaction.CommitAsync(ct);

        return offDay.OffDayId;
    }
}
