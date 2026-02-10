namespace SB.Domain;

using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveAllClassBookTopicPlanItemsCommandHandler(
    IUnitOfWork UnitOfWork,
    IClassBookTopicPlanItemsAggregateRepository ClassBookTopicPlanItemsAggregateRepository,
    ITopicsAggregateRepository TopicsAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveAllClassBookTopicPlanItemsCommand>
{
    public async Task Handle(RemoveAllClassBookTopicPlanItemsCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;
        int curriculumId = command.CurriculumId!.Value;

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

        var items =
            await this.ClassBookTopicPlanItemsAggregateRepository.FindAllByCurriculumIdAsync(
                schoolYear,
                classBookId,
                curriculumId,
                ct);

        var classBookTopicPlanItemIds = items.Select(i => i.ClassBookTopicPlanItemId).ToArray();

        foreach (var item in items)
        {
            this.ClassBookTopicPlanItemsAggregateRepository.Remove(item);
        }

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
