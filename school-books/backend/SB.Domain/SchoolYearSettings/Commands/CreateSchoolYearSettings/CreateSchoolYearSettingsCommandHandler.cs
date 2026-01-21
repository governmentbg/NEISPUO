namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateSchoolYearSettingsCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SchoolYearSettings> SchoolYearSettingsAggregateRepository,
    ISchoolYearSettingsRebuildService SchoolYearSettingsRebuildService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateSchoolYearSettingsCommand, int>
{
    public async Task<int> Handle(CreateSchoolYearSettingsCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var schoolYearSettings = new SchoolYearSettings(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.SchoolYearStartDate,
            command.FirstTermEndDate,
            command.SecondTermStartDate,
            command.SchoolYearEndDate,
            command.Description!,
            command.HasFutureEntryLock!.Value,
            command.PastMonthLockDay,
            command.IsForAllClasses!.Value,
            command.BasicClassIds ?? Array.Empty<int>(),
            command.ClassBookIds ?? Array.Empty<int>(), // TODO verify classbooks belong to institution
            command.SysUserId!.Value);

        await this.SchoolYearSettingsAggregateRepository.AddAsync(schoolYearSettings, ct);

        await using var transaction = await this.UnitOfWork.BeginTransactionAsync(ct);

        // save so that the SchoolYearSettings we are creating will
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
