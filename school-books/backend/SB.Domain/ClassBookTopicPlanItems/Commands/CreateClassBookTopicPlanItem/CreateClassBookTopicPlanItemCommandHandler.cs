namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateClassBookTopicPlanItemCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ClassBookTopicPlanItem> ClassBookTopicPlanItemAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateClassBookTopicPlanItemCommand, int>
{
    public async Task<int> Handle(CreateClassBookTopicPlanItemCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{command.SchoolYear!.Value}, classBookId:{command.ClassBookId!.Value}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        if (!await this.ClassBookCachedQueryStore.ExistsCurriculumForClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.CurriculumId!.Value,
            ct))
        {
            throw new DomainValidationException($"This curriculum is not in the class book curriculum list");
        }

        var item = new ClassBookTopicPlanItem(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.CurriculumId!.Value,
            command.Number!.Value,
            command.Title!,
            command.Note,
            command.Taken!.Value,
            command.SysUserId!.Value);

        await this.ClassBookTopicPlanItemAggregateRepository.AddAsync(item, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return item.ClassBookTopicPlanItemId;
    }
}
