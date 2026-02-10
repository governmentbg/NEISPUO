namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using SB.Common;

internal record CreateAttendancesCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Attendance> AttendanceAggregateRepository,
    IPersonMedicalNoticeQueryRepository PersonMedicalNoticeQueryRepository,
    IStudentSettingsAggregateRepository StudentSettingsAggregateRepository,
    INotificationsQueryRepository NotificationsQueryRepository,
    INotificationsService NotificationsService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateAttendancesCommand, int[]>
{
    public async Task<int[]> Handle(CreateAttendancesCommand command, CancellationToken ct)
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

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsAttendanceAbsenceTopicModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.Date!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookTypeAllowsAttendancesAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value, ct))
        {
            throw new DomainValidationException($"Cannot create attendance for the book type of classBookId:{command.ClassBookId!.Value}");
        }

        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;
        int sysUserId = command.SysUserId!.Value;

        var unexcusedAbsences =
            command.Attendances!
            .Where(a => a.Type == AttendanceType.UnexcusedAbsence)
            .Select(a => (a.PersonId!.Value, command.Date!.Value))
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

        List<int> createdAttendanceIds = new();
        foreach (var attendance in command.Attendances!)
        {
            if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                    schoolYear,
                    classBookId,
                    attendance.PersonId!.Value,
                    ct))
            {
                throw new DomainValidationException($"This person ({attendance.PersonId!.Value}) is not in the class book students list");
            }

            int personId = attendance.PersonId!.Value;
            DateTime date = command.Date!.Value;

            var excusedReasonId = attendance.Type == AttendanceType.ExcusedAbsence ? command.ExcusedReasonId : null;
            var excusedReasonComment = attendance.Type == AttendanceType.ExcusedAbsence ? command.ExcusedReasonComment : null;

            Attendance newAttendance =
                new(schoolYear,
                    classBookId,
                    attendance.PersonId!.Value,
                    attendance.Type!.Value,
                    date,
                    excusedReasonId,
                    excusedReasonComment,
                    sysUserId);
            await this.AttendanceAggregateRepository.AddAsync(newAttendance, ct);
            createdAttendanceIds.Add(newAttendance.AttendanceId);

            if (attendance.Type == AttendanceType.UnexcusedAbsence)
            {
                var mn = medicalNoticesLookup[personId]
                    .FirstOrDefault(mn =>
                        mn.FromDate <= date &&
                        mn.ToDate >= date);
                if (mn != null)
                {
                    newAttendance.ExcuseWithHisMedicalNotice(
                        mn.HisMedicalNoticeId,
                        mn.NrnMedicalNotice,
                        mn.Pmi,
                        mn.AuthoredOn,
                        mn.FromDate,
                        mn.ToDate);
                }
            }

            if (newAttendance.Type != AttendanceType.Presence)
            {
                var jObject = JObject.FromObject(new
                {
                    attendanceTypeText = newAttendance.Type.GetEnumDescription()!.ToLower(),
                    date = newAttendance.Date
                });
                await this.NotificationsService.TryPostNotificationsAsync("NewAttendanceAbsence", newAttendance.PersonId, jObject, ct);
            }
        }

        await this.UnitOfWork.SaveAsync(ct);

        return createdAttendanceIds.ToArray();
    }
}
