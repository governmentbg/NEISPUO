namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveClassBookTopicPlanItemCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ClassBookTopicPlanItem> ClassBookTopicPlanItemAggregateRepository,
    ITopicsAggregateRepository TopicsAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveClassBookTopicPlanItemCommand>
{
    public async Task Handle(RemoveClassBookTopicPlanItemCommand command, CancellationToken ct)
    {
        var schoolYear = command.SchoolYear!.Value;
        var classBookId = command.ClassBookId!.Value;

        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{schoolYear}, classBookId:{classBookId}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        var item = await this.ClassBookTopicPlanItemAggregateRepository.FindAsync(
            schoolYear,
            command.ClassBookTopicPlanItemId!.Value,
            ct);

        if (classBookId != item.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        if (command.CurriculumId!.Value != item.CurriculumId)
        {
            // the curriculum check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.CurriculumId)}.");
        }

        this.ClassBookTopicPlanItemAggregateRepository.Remove(item);

        var classBookTopicPlanItemIds = new[] { item.ClassBookTopicPlanItemId };
        var usedTopics =
            await this.TopicsAggregateRepository.FindUsedTopicsAsync(
                schoolYear,
                classBookId,
                classBookTopicPlanItemIds,
                ct);

        foreach (var topic in usedTopics)
        {
            topic.ClearClassBookTopicPlanItemId(classBookTopicPlanItemIds);
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
