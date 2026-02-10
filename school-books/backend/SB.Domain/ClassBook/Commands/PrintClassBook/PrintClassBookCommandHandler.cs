namespace SB.Domain;

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record PrintClassBookCommandHandler(
    IUnitOfWork UnitOfWork,
    IClassBooksAggregateRepository ClassBookAggregateRepository,
    IQueueMessagesService QueueMessagesService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<PrintClassBookCommand>
{
    public async Task Handle(PrintClassBookCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{command.SchoolYear!.Value}, classBookId:{command.ClassBookId!.Value}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        var classBook = await this.ClassBookAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct);

        var classBookPrint = classBook.CreateClassBookPrint(command.SysUserId!.Value);
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
    }
}
