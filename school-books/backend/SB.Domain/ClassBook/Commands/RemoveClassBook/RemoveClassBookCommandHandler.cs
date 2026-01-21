namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SB.Common;

internal record RemoveClassBookCommandHandler(
    IUnitOfWork UnitOfWork,
    ILogger<RemoveClassBookCommandHandler> Logger,
    IScopedAggregateRepository<ClassBook> ClassBookAggregateRepository,
    IClassBookOffDayDatesAggregateRepository ClassBookOffDayDatesAggregateRepository,
    IClassBookSchoolYearSettingsAggregateRepository ClassBookSchoolYearSettingsAggregateRepository,
    IOffDaysAggregateRepository OffDaysAggregateRepository,
    ISchoolYearSettingsAggregateRepository SchoolYearSettingsAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore,
    IClassBooksAdminQueryRepository ClassBooksAdminQueryRepository)
    : IRequestHandler<RemoveClassBookCommand>
{
    public async Task Handle(RemoveClassBookCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int instId = command.InstId!.Value;
        int classBookId = command.ClassBookId!.Value;

        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{schoolYear}, classBookId:{classBookId}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        var classBook = await this.ClassBookAggregateRepository.FindAsync(
            schoolYear,
            classBookId,
            ct);

        this.ClassBookAggregateRepository.Remove(classBook);

        var schoolYearSettings = await this.ClassBookSchoolYearSettingsAggregateRepository.FindAsync(
            schoolYear,
            classBookId,
            ct);
        this.ClassBookSchoolYearSettingsAggregateRepository.Remove(schoolYearSettings);

        var classBookOffDayDates = await this.ClassBookOffDayDatesAggregateRepository.FindAllByClassBookAsync(
            schoolYear,
            classBookId,
            ct);
        foreach (var cbod in classBookOffDayDates)
        {
            this.ClassBookOffDayDatesAggregateRepository.Remove(cbod);
        }

        var offDays = await this.OffDaysAggregateRepository.FindAllByClassBookAsync(
            schoolYear,
            instId,
            classBookId,
            ct);
        foreach (var od in offDays)
        {
            od.RemoveClassBookId(classBookId);
        }

        var allSchoolYearSettings = await this.SchoolYearSettingsAggregateRepository.FindAllByClassBookAsync(
            schoolYear,
            instId,
            classBookId,
            ct);
        foreach (var sys in allSchoolYearSettings)
        {
            sys.RemoveClassBookId(classBookId);
        }

        await this.ClassBooksAdminQueryRepository.RemoveRelatedPersonnelSchoolBookAccessAsync(schoolYear, classBookId, ct);

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
                        new string[] { $"има въведени {objectName}" });
                }

                this.Logger.LogWarning(ex, "Unknown constraint violation exception {@ex}", ex);

                // a saving face path
                throw new DomainValidationException(
                    Array.Empty<string>(),
                    new string[] { "има свързани записи" },
                    knownSqlError.ErrorMessage);
            }

            throw;
        }
    }

    private string? MapFKToObjectName(string fk)
        => fk switch
        {
            "FK_Schedule_ClassBook" => "разписания",
            "FK_Absence_ClassBook" => "отсъствия",
            "FK_Attendance_ClassBook" => "присъствия",
            "FK_Grade_ClassBook" => "оценки",
            "FK_AdditionalActivity_ClassBook" => "допълнителни дейности",
            "FK_Exam_ClassBook" => "контролни/класни работи",
            "FK_FirstGradeResult_ClassBook" => "общ годишен успех",
            "FK_GradeResult_ClassBook" => "резултати",
            "FK_PgResult_ClassBook" => "резултати ПГ",
            "FK_IndividualWork_ClassBook" => "индивидуални работи",
            "FK_Note_ClassBook" => "бележки",
            "FK_ParentMeeting_ClassBook" => "родителски срещи",
            "FK_Remark_ClassBook" => "отзиви",
            "FK_Sanction_ClassBook" => "санкции",
            "FK_Support_ClassBook" => "подкрепи",
            "FK_Topic_ClassBook" => "теми",
            "FK_ClassBookTopicPlanItem_ClassBook" => "тематични разпределения",
            "FK_Performance_ClassBook" => "изяви",
            "FK_ReplrParticipation_ClassBook" => "участия в РЕПЛР",

            _ => null,
        };
}
