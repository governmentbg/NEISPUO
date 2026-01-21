namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateAdditionalActivityCommandHandler(
    IUnitOfWork UnitOfWork,
    IClassBookCachedQueryStore ClassBookCachedQueryStore,
    IScopedAggregateRepository<AdditionalActivity> AdditionalActivityAggregateRepository)
    : IRequestHandler<CreateAdditionalActivityCommand, int>
{
    public async Task<int> Handle(CreateAdditionalActivityCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsAdditionalActivityModificationsAsync(
            schoolYear,
            classBookId,
            command.Year!.Value,
            command.WeekNumber!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookTypeAllowsAdditionalActivitiesAsync(schoolYear, classBookId, ct))
        {
            throw new DomainValidationException($"Cannot create additionalActivity for the book type of classBookId:{classBookId}");
        }

        var additionalActivity = new AdditionalActivity(
            schoolYear,
            classBookId,
            command.Year!.Value,
            command.WeekNumber!.Value,
            command.Activity!,
            command.SysUserId!.Value);

        await this.AdditionalActivityAggregateRepository.AddAsync(additionalActivity, ct);

        await this.UnitOfWork.SaveAsync(ct);

        return additionalActivity.AdditionalActivityId;
    }
}
