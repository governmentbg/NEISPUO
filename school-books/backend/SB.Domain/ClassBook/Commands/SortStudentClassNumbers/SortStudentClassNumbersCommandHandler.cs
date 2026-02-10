namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SB.Common;

internal record SortStudentClassNumbersCommandHandler(
    IUnitOfWork UnitOfWork,
    IStudentClassQueryRepository StudentClassQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<SortStudentClassNumbersCommand>
{
    public async Task Handle(SortStudentClassNumbersCommand command, CancellationToken ct)
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
            await this.StudentClassQueryRepository.FindAllByClassBookAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct);

        var personIdWithAbsencesAttendancesAsync =
            await this.StudentClassQueryRepository.GetPersonIdsWithAbsencesAttendancesAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct);

        var currentClassNumber = 1;
        foreach (var (personId, studentClasses) in students
            .GroupBy(s => new { s.PersonId, s.FirstName, s.MiddleName, s.LastName })
            .OrderBy(g => g.Key.FirstName)
            .ThenBy(g => g.Key.MiddleName)
            .ThenBy(g => g.Key.LastName)
            .Select(g => (personId: g.Key.PersonId, studentClasses: g.Select(gi => gi.StudentClass))))
        {
            if (studentClasses.Any(sc => sc.Status == StudentClassStatus.Enrolled) ||
                personIdWithAbsencesAttendancesAsync.Contains(personId))
            {
                foreach (var sc in studentClasses)
                {
                    sc.SetClassNumber(currentClassNumber);
                }
                currentClassNumber++;
            }
            else
            {
                foreach (var sc in studentClasses)
                {
                    sc.SetClassNumber(null);
                }
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
