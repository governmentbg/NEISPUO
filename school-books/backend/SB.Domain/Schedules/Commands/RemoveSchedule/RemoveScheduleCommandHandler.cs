namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SB.Common;

internal record RemoveScheduleCommandHandler(
    ILogger<RemoveScheduleCommandHandler> Logger,
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Schedule> ScheduleAggregateRepository,
    IScopedAggregateRepository<Shift> ShiftAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveScheduleCommand>
{
    public async Task Handle(RemoveScheduleCommand command, CancellationToken ct)
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

        var schedule = await this.ScheduleAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ScheduleId!.Value,
            ct);

        if (command.ClassBookId!.Value != schedule.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        this.ScheduleAggregateRepository.Remove(schedule);

        var shift = await this.ShiftAggregateRepository.FindAsync(command.SchoolYear!.Value, schedule.ShiftId, ct);
        if (shift.IsAdhoc)
        {
            this.ShiftAggregateRepository.Remove(shift);
        }

        try
        {
            await this.UnitOfWork.SaveAsync(ct);
        }
        catch (DomainUpdateSqlException ex)
        {
            if (ex.SqlException.AsKnownSqlError()
                is KnownSqlError { Type: KnownSqlErrorType.ReferenceConstraintError } knownSqlError)
            {
                if (this.MapFKToObjectName(knownSqlError.ConstraintOrIndexName) is string objectName)
                {
                    throw new DomainValidationException(
                        Array.Empty<string>(),
                        new string[] { $"има свързани {objectName}" });
                }

                this.Logger.LogWarning(ex, "Unknown constraint violation exception {@ex}", ex);

                // a saving face path
                throw new DomainValidationException(
                    Array.Empty<string>(),
                    new string[] { "има свързани записи" },
                    knownSqlError.ErrorMessage);
            }
        }
    }

    private string? MapFKToObjectName(string fk)
        => fk switch
        {
            "FK_Absence_ScheduleLesson" => "отсъствия",
            "FK_Grade_ScheduleLesson" => "оценки",
            "FK_Topic_ScheduleLesson" => "теми",
            "FK_TeacherAbsenceHour_ScheduleLesson" => "учителски отсъствия",
            _ => null,
        };
}
