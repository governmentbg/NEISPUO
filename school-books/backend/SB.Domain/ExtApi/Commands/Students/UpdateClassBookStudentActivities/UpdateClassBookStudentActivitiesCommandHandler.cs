namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateClassBookStudentActivitiesCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ClassBook> ClassBookAggregateRepository,
    IStudentClassQueryRepository StudentClassQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateClassBookStudentActivitiesCommand>
{
    public async Task Handle(UpdateClassBookStudentActivitiesCommand command, CancellationToken ct)
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

        if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.PersonId!.Value,
            ct))
        {
            throw new DomainValidationException("This person is not in the class book students list");
        }

        var classBook = await this.ClassBookAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct);

        classBook.UpdateStudentActivities(
            command.PersonId!.Value,
            command.Activities,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
