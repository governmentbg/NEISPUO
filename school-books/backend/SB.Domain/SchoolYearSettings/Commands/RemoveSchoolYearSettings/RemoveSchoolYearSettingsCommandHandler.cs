namespace SB.Domain;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveSchoolYearSettingsCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SchoolYearSettings> SchoolYearSettingsAggregateRepository,
    IClassBookSchoolYearSettingsRepository ClassBookSchoolYearSettingsRepository,
    ISchoolYearSettingsQueryRepository SchoolYearSettingsQueryRepository,
    ISchoolYearSettingsRebuildService SchoolYearSettingsRebuildService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveSchoolYearSettingsCommand>
{
    public async Task Handle(RemoveSchoolYearSettingsCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int instId = command.InstId!.Value;
        int schoolYearSettingsId = command.SchoolYearSettingsId!.Value;

        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            schoolYear,
            instId,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var schoolYearSettings = await this.SchoolYearSettingsAggregateRepository.FindAsync(
            schoolYear,
            schoolYearSettingsId,
            ct);

        if (command.InstId!.Value != schoolYearSettings.InstId)
        {
            // the instId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.InstId)}.");
        }

        this.SchoolYearSettingsAggregateRepository.Remove(schoolYearSettings);

        if (await this.ClassBookSchoolYearSettingsRepository.ExistsClassBookSchoolYearSettingsAsync(
            schoolYear,
            schoolYearSettingsId,
            ct))
        {
            await using var transaction = await this.UnitOfWork.BeginTransactionAsync(ct);

            // unlink ClassBookSchoolYearSettings so that we can remove the SchoolYearSettings
            await this.SchoolYearSettingsQueryRepository.RemoveSchoolYearSettingsLinkAsync(
                schoolYear,
                instId,
                schoolYearSettingsId,
                ct);

            // save so that the SchoolYearSettings we are removing will
            // not be picked up by the GetAllForRebuildAsync query
            await this.UnitOfWork.SaveAsync(ct);

            await this.SchoolYearSettingsRebuildService.RebuildAndSaveAsync(
                schoolYear,
                instId,
                transaction,
                ct);

            await transaction.CommitAsync(ct);
        }
        else
        {
            // rebuild escape path
            // there are no ClassBookSchoolYearSettings depending on us
            await this.UnitOfWork.SaveAsync(ct);
        }
    }
}
