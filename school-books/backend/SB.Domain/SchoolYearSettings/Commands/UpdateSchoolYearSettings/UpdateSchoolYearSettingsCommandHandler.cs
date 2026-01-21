namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateSchoolYearSettingsCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SchoolYearSettings> SchoolYearSettingsAggregateRepository,
    IClassBookSchoolYearSettingsRepository ClassBookSchoolYearSettingsRepository,
    ISchoolYearSettingsRebuildService SchoolYearSettingsRebuildService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateSchoolYearSettingsCommand, int>
{
    public async Task<int> Handle(UpdateSchoolYearSettingsCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var schoolYearSettings = await this.SchoolYearSettingsAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SchoolYearSettingsId!.Value,
            ct);

        if (command.InstId!.Value != schoolYearSettings.InstId)
        {
            // the instId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.InstId)}.");
        }

        var basicClassIds = command.BasicClassIds ?? Array.Empty<int>();
        var classBookIds = command.ClassBookIds ?? Array.Empty<int>();

        bool basicClassIdsChanged =
            schoolYearSettings.Classes
            .Select(c => c.BasicClassId)
            .ToHashSet()
            .SetEquals(basicClassIds) == false;

        bool classBookIdsChanged =
            schoolYearSettings.ClassBooks
            .Select(c => c.ClassBookId)
            .ToHashSet()
            .SetEquals(classBookIds) == false;

        schoolYearSettings.UpdateData(
            command.SchoolYearStartDate,
            command.FirstTermEndDate,
            command.SecondTermStartDate,
            command.SchoolYearEndDate,
            command.Description!,
            command.HasFutureEntryLock!.Value,
            command.PastMonthLockDay,
            command.IsForAllClasses!.Value,
            basicClassIds,
            classBookIds, // TODO verify classbooks belong to institution
            command.SysUserId!.Value);

        await using var transaction = await this.UnitOfWork.BeginTransactionAsync(ct);

        // save so that the SchoolYearSettings we are updating will
        // be picked up by the GetAllForRebuildAsync query
        await this.UnitOfWork.SaveAsync(ct);

        await this.SchoolYearSettingsRebuildService.RebuildAndSaveAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            transaction,
            ct);

        await transaction.CommitAsync(ct);

        return schoolYearSettings.SchoolYearSettingsId;
    }
}
