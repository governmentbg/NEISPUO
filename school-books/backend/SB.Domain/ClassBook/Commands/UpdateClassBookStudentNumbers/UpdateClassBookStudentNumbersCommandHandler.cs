namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SB.Common;

internal record UpdateClassBookStudentNumbersCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ClassBook> ClassBookAggregateRepository,
    IStudentClassQueryRepository StudentClassQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateClassBookStudentNumbersCommand>
{
    public async Task Handle(UpdateClassBookStudentNumbersCommand command, CancellationToken ct)
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

        var students =
            (await this.StudentClassQueryRepository.FindAllByClassBookAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct
            )
        ).Select(s => s.StudentClass);

        foreach (var student in command.Students!)
        {
            var studentClasses = students.Where(s => s.PersonId == student.PersonId);

            foreach (var sc in studentClasses)
            {
                sc.SetClassNumber(student.ClassNumber);
            }
        }

        await this.UnitOfWork.SaveWithRetryingStrategyAsync(
            new []
            {
                // see https://dba.stackexchange.com/questions/211467/error-updating-temporal-tables
                SqlServerErrorCodes.DataModificationFailedOnSystemVersionedTable
            },
            ct);
    }
}
