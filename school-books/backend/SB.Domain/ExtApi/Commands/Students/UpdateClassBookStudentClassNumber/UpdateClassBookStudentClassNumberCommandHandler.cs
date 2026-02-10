namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SB.Common;

internal record UpdateClassBookStudentClassNumberCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ClassBook> ClassBookAggregateRepository,
    IStudentClassQueryRepository StudentClassQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateClassBookStudentClassNumberCommand>
{
    public async Task Handle(UpdateClassBookStudentClassNumberCommand command, CancellationToken ct)
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

        var studentClasses = await this.StudentClassQueryRepository.FindAllByClassBookAndPersonIdAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.PersonId!.Value,
            ct);

        if (studentClasses.Length == 0)
        {
            throw new DomainValidationException($"Student with personId:{command.PersonId!.Value} does not exists in any of the ClassBook's ClassGroups.");
        }

        foreach (var sc in studentClasses)
        {
            sc.SetClassNumber(command.ClassNumber);
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
