namespace SB.Domain;

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record PrintClassBookStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IClassBooksAggregateRepository ClassBookAggregateRepository,
    IQueueMessagesService QueueMessagesService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<PrintClassBookStudentCommand>
{
    public async Task Handle(PrintClassBookStudentCommand command, CancellationToken ct)
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

        var classBookStudentPrint = classBook.CreateClassBookStudentPrint(command.PersonId!.Value, command.SysUserId!.Value);
        await this.ClassBookAggregateRepository.AddAsync(
            classBookStudentPrint,
            ct);

        await this.UnitOfWork.SaveAsync(ct);

        await this.QueueMessagesService.PostMessageAndSaveAsync(
            new PrintHtmlQueueMessage(
                PrintType.StudentBook,
                JsonSerializer.Serialize(
                    new ClassBookStudentPrintParams(
                        command.SchoolYear!.Value,
                        command.ClassBookId!.Value,
                        command.PersonId!.Value)),
                classBookStudentPrint.ClassBookStudentPrintId),
            ct);
    }
}
