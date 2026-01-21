namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using SB.Common;

internal record CreateAbsenceCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Absence> AbsenceAggregateRepository,
    IPersonMedicalNoticeQueryRepository PersonMedicalNoticeQueryRepository,
    INotificationsQueryRepository NotificationsQueryRepository,
    INotificationsService NotificationsService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateAbsenceCommand, int[]>
{
    public async Task<int[]> Handle(CreateAbsenceCommand command, CancellationToken ct)
    {
        if (!command.Absences!.Any())
        {
            return Array.Empty<int>();
        }

        int schoolYear = command.SchoolYear!.Value;
        int instId = command.InstId!.Value;
        int classBookId = command.ClassBookId!.Value;
        int sysUserId = command.SysUserId!.Value;

        bool? isDplr;
        if (command.Absences!.All(a => a.Type == null))
        {
            isDplr = null;
        }
        else if (command.Absences!.Where(a => a.Type != null).All(a => a.Type == AbsenceType.DplrAbsence || a.Type == AbsenceType.DplrAttendance))
        {
            isDplr = true;
        }
        else if (command.Absences!.Where(a => a.Type != null).All(a => a.Type != AbsenceType.DplrAbsence && a.Type != AbsenceType.DplrAttendance))
        {
            isDplr = false;
        }
        else
        {
            throw new DomainValidationException("Absence types from DPLR and non DPLR classbooks cannot be mixed.");
        }

        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{schoolYear}, classBookId:{classBookId}) does not exist.");
        }

        if (isDplr == false &&
            !await this.ClassBookCachedQueryStore.CheckBookTypeAllowsAbsencesAsync(schoolYear, classBookId, ct))
        {
            throw new DomainValidationException($"Cannot create absence for the book type of classBookId:{classBookId}");
        }

        if (isDplr == true &&
            !await this.ClassBookCachedQueryStore.CheckBookTypeAllowsDplrAbsencesAsync(schoolYear, classBookId, ct))
        {
            throw new DomainValidationException($"Cannot create dplr absence for the book type of classBookId:{classBookId}");
        }

        var unexcusedAbsences =
            command.Absences!
            .Where(a => a.ConvertToLateId == null && a.Type == AbsenceType.Unexcused)
            .Select(a => (a.PersonId!.Value, a.Date!.Value))
            .Distinct()
            .ToArray();
        var medicalNotices =
            await this.PersonMedicalNoticeQueryRepository.GetAllByAbsencesAsync(
                schoolYear,
                unexcusedAbsences,
                ct);
        var medicalNoticesLookup =
            medicalNotices
            .ToLookup(mn => mn.PersonId);

        List<int> createdAbsenceIds = new();
        foreach (var absence in command.Absences!)
        {
            int personId = absence.PersonId!.Value;
            DateTime date = absence.Date!.Value;
            int scheduleLessonId = absence.ScheduleLessonId!.Value;

            if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                    command.SchoolYear!.Value,
                    command.ClassBookId!.Value,
                    ct))
            {
                throw new DomainValidationException($"The classbook is marked as invalid (archived).");
            }

            if (!await this.ClassBookCachedQueryStore.CheckBookAllowsAttendanceAbsenceTopicModificationsAsync(
                schoolYear,
                classBookId,
                date,
                ct))
            {
                throw new DomainValidationException($"The classbook is locked.");
            }

            if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                schoolYear,
                classBookId,
                personId,
                ct))
            {
                throw new DomainValidationException($"This person ({absence.PersonId!.Value}) is not in the class book students list");
            }

            if (!await this.ClassBookCachedQueryStore.ExistsScheduleLessonForClassBookAsync(
                schoolYear,
                classBookId,
                date,
                scheduleLessonId,
                personId,
                ct))
            {
                throw new DomainValidationException("This scheduleLesson is not in any of the classbook's schedules.");
            }

            if (absence.UndoAbsenceId.HasValue)
            {
                var undoAbsence = await this.AbsenceAggregateRepository.FindAsync(schoolYear, absence.UndoAbsenceId.Value, ct);
                this.AbsenceAggregateRepository.Remove(undoAbsence);
            }

            if (absence.ConvertToLateId.HasValue)
            {
                var convertToLate = await this.AbsenceAggregateRepository.FindAsync(schoolYear, absence.ConvertToLateId.Value, ct);
                convertToLate.ConvertToLate(sysUserId);
            }
            else if (absence.Type.HasValue)
            {
                AbsenceType type = absence.Type.Value;

                var teacherAbsenceId = await this.ClassBookCachedQueryStore.GetScheduleLessonTeacherAbsenceIdAsync(
                    command.SchoolYear!.Value,
                    absence.ScheduleLessonId!.Value,
                    ct);

                if (teacherAbsenceId != absence.TeacherAbsenceId)
                {
                    throw new DomainValidationException("teacherAbsenceId is invalid.");
                }

                var excusedReasonId = type == AbsenceType.Excused ? command.ExcusedReasonId : null;
                var excusedReasonComment = type == AbsenceType.Excused ? command.ExcusedReasonComment : null;

                Absence newAbsence =
                    new(schoolYear,
                        classBookId,
                        personId,
                        type,
                        await this.ClassBookCachedQueryStore.GetTermForDateAsync(
                            schoolYear,
                            classBookId,
                            date,
                            ct),
                        date,
                        scheduleLessonId,
                        absence.TeacherAbsenceId,
                        excusedReasonId,
                        excusedReasonComment,
                        sysUserId);
                await this.AbsenceAggregateRepository.AddAsync(newAbsence, ct);
                createdAbsenceIds.Add(newAbsence.AbsenceId);

                if (absence.Type == AbsenceType.Unexcused)
                {
                    var mn = medicalNoticesLookup[personId]
                        .FirstOrDefault(mn =>
                            mn.FromDate <= date &&
                            mn.ToDate >= date);
                    if (mn != null)
                    {
                        newAbsence.ExcuseWithHisMedicalNotice(
                            mn.HisMedicalNoticeId,
                            mn.NrnMedicalNotice,
                            mn.Pmi,
                            mn.AuthoredOn,
                            mn.FromDate,
                            mn.ToDate);
                    }
                }

                var curriculumName = await this.NotificationsQueryRepository.GetCurriculumNameForScheduleLessonAsync(schoolYear, instId, scheduleLessonId, ct);
                var jObject = JObject.FromObject(new
                {
                    absenceTypeText = absence.Type.GetEnumDescription()!.ToLower(),
                    curriculumName,
                    date = newAbsence.Date,
                    emailTag = newAbsence.EmailTag,
                    pushNotificationTag = newAbsence.PushNotificationTag
                });

                await this.NotificationsService.TryPostNotificationsAsync("NewAbsence", newAbsence.PersonId, jObject, ct);
            }
        }

        try
        {
            await this.UnitOfWork.SaveAsync(ct);
        }
        catch (DomainUpdateSqlException ex) when (isDplr == true)
        {
            if (ex.SqlException.AsKnownSqlError()
                is KnownSqlError { Type: KnownSqlErrorType.UniqueKeyConstraintError } knownSqlError &&
                knownSqlError.ConstraintOrIndexName == "UK_Absence_PersonId_SchoolYear_ClassBookId_ScheduleLessonId")
            {
                // if there is a unique key constraint error
                // we can assume there is an Absence of the opposite Dplr type
                string typeName = command.Absences![0].Type == AbsenceType.DplrAbsence ? "присъствие" : "отсъствие";
                throw new DomainValidationException(
                    Array.Empty<string>(),
                    new string[] { $"Ученикът вече има въведено {typeName}" });
            }

            throw;
        }

        return createdAbsenceIds.ToArray();
    }
}
