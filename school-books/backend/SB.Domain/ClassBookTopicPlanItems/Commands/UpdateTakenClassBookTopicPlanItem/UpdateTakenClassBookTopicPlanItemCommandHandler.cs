namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateTakenClassBookTopicPlanItemCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ClassBookTopicPlanItem> ClassBookTopicPlanItemAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateTakenClassBookTopicPlanItemCommand>
{
    public async Task Handle(UpdateTakenClassBookTopicPlanItemCommand command, CancellationToken ct)
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

        var item = await this.ClassBookTopicPlanItemAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ClassBookTopicPlanItemId!.Value,
            ct);

        if (command.ClassBookId!.Value != item.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        if (command.CurriculumId!.Value != item.CurriculumId)
        {
            // the curriculum check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.CurriculumId)}.");
        }

        item.UpdateTaken(
            command.Taken!.Value,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
