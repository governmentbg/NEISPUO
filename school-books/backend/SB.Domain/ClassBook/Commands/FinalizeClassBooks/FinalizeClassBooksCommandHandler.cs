namespace SB.Domain;

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record FinalizeClassBooksCommandHandler(
    IUnitOfWork UnitOfWork,
    IClassBooksAggregateRepository ClassBookAggregateRepository,
    IQueueMessagesService QueueMessagesService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<FinalizeClassBooksCommand>
{
    public async Task Handle(FinalizeClassBooksCommand command, CancellationToken ct)
    {
        var classBooks = await this.ClassBookAggregateRepository.FindAllByIdsAsync(
            command.SchoolYear!.Value,
            command.ClassBookIds!,
            ct);

        var queueMessages = new PrintHtmlQueueMessage[classBooks.Length];
        for (int i = 0; i < classBooks.Length; i++)
        {
            var classBook = classBooks[i];

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

            queueMessages[i] =
                new PrintHtmlQueueMessage(
                    PrintType.ClassBook,
                    JsonSerializer.Serialize(
                        new ClassBookPrintParams(
                            classBook.SchoolYear,
                            classBook.ClassBookId)),
                    classBookPrint.ClassBookPrintId);
        }

        await this.UnitOfWork.SaveAsync(ct);

        await this.QueueMessagesService.PostMessagesAndSaveAsync(
            queueMessages,
            ct);

        await this.ClassBookCachedQueryStore.ClearClassBooksAsync(
            command.SchoolYear!.Value,
            command.ClassBookIds!);
    }
}
