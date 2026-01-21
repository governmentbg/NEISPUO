namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UnfinalizeClassBookCommandHandler(
    IUnitOfWork UnitOfWork,
    IClassBooksAggregateRepository ClassBookAggregateRepository,
    IQueueMessagesService QueueMessagesService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UnfinalizeClassBookCommand>
{
    public async Task Handle(UnfinalizeClassBookCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var classBook = await this.ClassBookAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct);

        if (!classBook.IsFinalized)
        {
            throw new DomainException("The class book is not finalized.");
        }

        classBook.Unfinalize(command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);

        await this.ClassBookCachedQueryStore.ClearClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            // no cancellation
            default(CancellationToken));
    }
}
