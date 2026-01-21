namespace SB.Domain;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateClassBookMainDataCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ClassBook> ClassBookAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateClassBookMainDataCommand>
{
    public async Task Handle(UpdateClassBookMainDataCommand command, CancellationToken ct)
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

        var classBook = await this.ClassBookAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct);
        classBook.UpdateData(
            command.BookName ?? "",
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
