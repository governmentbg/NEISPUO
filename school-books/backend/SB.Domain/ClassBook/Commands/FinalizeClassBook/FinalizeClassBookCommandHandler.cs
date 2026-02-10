namespace SB.Domain;

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record FinalizeClassBookCommandHandler(
    IUnitOfWork UnitOfWork,
    IClassBooksAggregateRepository ClassBookAggregateRepository,
    IQueueMessagesService QueueMessagesService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<FinalizeClassBookCommand>
{
    public async Task Handle(FinalizeClassBookCommand command, CancellationToken ct)
    {
        var classBook = await this.ClassBookAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct);

        if (classBook.IsFinalized)
        {
            throw new DomainValidationException(
                new [] { "class_book_already_finalized" },
                new [] { $"Дневникът '{classBook.FullBookName}' вече е приключен." });
        }

        var classBookPrint = classBook.Finalize(command.SysUserId!.Value);
        await this.ClassBookAggregateRepository.AddAsync(
            classBookPrint,
            ct);

        await this.UnitOfWork.SaveAsync(ct);

        await this.QueueMessagesService.PostMessageAndSaveAsync(
            new PrintHtmlQueueMessage(
                PrintType.ClassBook,
                JsonSerializer.Serialize(
                    new ClassBookPrintParams(
                        command.SchoolYear!.Value,
                        command.ClassBookId!.Value)),
                classBookPrint.ClassBookPrintId),
            ct);

        await this.ClassBookCachedQueryStore.ClearClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            // no cancellation
            default(CancellationToken));
    }
}
