namespace SB.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.ILectureSchedulesQueryRepository;

internal class LectureSchedulesQueryRepository : Repository, ILectureSchedulesQueryRepository
{
    public LectureSchedulesQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        string? teacherName,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var personPredicate = PredicateBuilder.True<Person>();

        if (!string.IsNullOrWhiteSpace(teacherName))
        {
            string[] words = teacherName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                personPredicate = personPredicate.AndAnyStringContains(p => p.FirstName, p => p.MiddleName, p => p.LastName, word);
            }
        }

        return await (
            from lc in this.DbContext.Set<LectureSchedule>()

            join t in this.DbContext.Set<Person>().Where(personPredicate) on lc.TeacherPersonId equals t.PersonId

            where lc.SchoolYear == schoolYear &&
                lc.InstId == instId

            orderby lc.StartDate descending

            select new GetAllVO(
                lc.LectureScheduleId,
                lc.TeacherPersonId,
                StringUtils.JoinNames(t.FirstName, t.LastName),
                lc.OrderNumber,
                lc.OrderDate,
                lc.StartDate,
                lc.EndDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<TableResultVO<GetAllVO>> GetAllForTeacherPersonAsync(
        int schoolYear,
        int instId,
        int teacherPersonId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from lc in this.DbContext.Set<LectureSchedule>()

            join t in this.DbContext.Set<Person>() on lc.TeacherPersonId equals t.PersonId

            where lc.SchoolYear == schoolYear &&
                lc.InstId == instId &&
                lc.TeacherPersonId == teacherPersonId

            orderby lc.StartDate descending

            select new GetAllVO(
                lc.LectureScheduleId,
                lc.TeacherPersonId,
                StringUtils.JoinNames(t.FirstName, t.LastName),
                lc.OrderNumber,
                lc.OrderDate,
                lc.StartDate,
                lc.EndDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int lectureScheduleId,
        CancellationToken ct)
    {
        var lectureScheduleHours = await (
            from lsh in this.DbContext.Set<LectureScheduleHour>()
            join sl in this.DbContext.Set<ScheduleLesson>() on new { lsh.SchoolYear, lsh.ScheduleLessonId } equals new { sl.SchoolYear, sl.ScheduleLessonId }
            where lsh.SchoolYear == schoolYear &&
                lsh.LectureSchedule.InstId == instId &&
                lsh.LectureScheduleId == lectureScheduleId
            orderby sl.Date
            select new GetVOHour(sl.ScheduleLessonId, sl.Date)
        ).ToArrayAsync(ct);

        return await (
            from lc in this.DbContext.Set<LectureSchedule>()

            where lc.SchoolYear == schoolYear &&
                lc.InstId == instId &&
                lc.LectureScheduleId == lectureScheduleId

            select new GetVO(
                lc.TeacherPersonId,
                lc.OrderNumber,
                lc.OrderDate,
                lc.StartDate,
                lc.EndDate,
                lectureScheduleHours)
        ).SingleAsync(ct);
    }

    public async Task<int> GetTeacherPersonIdAsync(
        int schoolYear,
        int lectureScheduleId,
        CancellationToken ct)
    {
        return await
            this.DbContext
            .Set<LectureSchedule>()
            .Where(ta =>
                ta.SchoolYear == schoolYear &&
                ta.LectureScheduleId == lectureScheduleId)
            .Select(ta => ta.TeacherPersonId)
            .SingleAsync(ct);
    }

    public async Task<GetLessonsVO[]> GetLessonsAsync(
        int schoolYear,
        int instId,
        int[] scheduleLessonIds,
        CancellationToken ct)
    {
        var replLessons = await (
            from sl in this.DbContext.Set<ScheduleLesson>()

            join s in this.DbContext.Set<Schedule>() on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }

            join cb in this.DbContext.Set<ClassBook>() on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            join tah in this.DbContext.Set<TeacherAbsenceHour>() on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
            into j1 from tah in j1.DefaultIfEmpty()

            join lsh in this.DbContext.Set<LectureScheduleHour>() on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { lsh.SchoolYear, lsh.ScheduleLessonId }
            into j2 from lsh in j2.DefaultIfEmpty()

            where sl.SchoolYear == schoolYear &&
                cb.IsValid &&
                this.DbContext
                .MakeIdsQuery(scheduleLessonIds)
                .Any(id => sl.ScheduleLessonId == id.Id)

            select new
            {
                sl.ScheduleLessonId,
                sl.Date,
                tah.ReplTeacherPersonId,
                CurriculumTeacherPersonIds = (
                    from t in this.DbContext.Set<CurriculumTeacher>()
                    join sp in this.DbContext.Set<StaffPosition>() on t.StaffPositionId equals sp.StaffPositionId

                    // Getting only valid teachers for the date of the lesson
                    where t.CurriculumId == sl.CurriculumId &&
                          ((t.SchoolYear <= 2022 && t.IsValid) ||
                           (t.SchoolYear > 2022 &&
                            (t.StaffPositionStartDate ?? t.ValidFrom ?? new DateTime(schoolYear, 9, 1)) <= sl.Date &&
                            (t.StaffPositionTerminationDate == null || t.StaffPositionTerminationDate >= sl.Date || t.NoReplacement)))

                    select
                        sp.PersonId
                ).ToArray(),
                LectureScheduleId = (int?)lsh.LectureScheduleId
            }
        ).ToArrayAsync(ct);

        return replLessons
            .Select(sl =>
                new GetLessonsVO
                {
                    ScheduleLessonId = sl.ScheduleLessonId,
                    Date = sl.Date,
                    CurriculumTeacherPersonIds = sl.ReplTeacherPersonId == null ?
                        sl.CurriculumTeacherPersonIds :
                        sl.CurriculumTeacherPersonIds.Concat(new int[] { sl.ReplTeacherPersonId.Value }).ToArray(),
                    LectureScheduleId = sl.LectureScheduleId
                }
            ).ToArray();
    }

    public async Task<GetLessonsInUseVO[]> GetLessonsInUseAsync(
        int schoolYear,
        int instId,
        int[] scheduleLessonIds,
        int? exceptLectureScheduleId,
        CancellationToken ct)
    {
        var lectureScheduleHourPredicate = PredicateBuilder.True<LectureScheduleHour>();

        lectureScheduleHourPredicate = lectureScheduleHourPredicate.And(usl => usl.SchoolYear == schoolYear &&
                this.DbContext
                .MakeIdsQuery(scheduleLessonIds)
                .Any(id => usl.ScheduleLessonId == id.Id));

        if (exceptLectureScheduleId.HasValue)
        {
            lectureScheduleHourPredicate = lectureScheduleHourPredicate.And(a => a.LectureScheduleId != exceptLectureScheduleId.Value);
        }

        return await (
            from usl in this.DbContext.Set<LectureScheduleHour>().Where(lectureScheduleHourPredicate)
            join sl in this.DbContext.Set<ScheduleLesson>() on new { usl.SchoolYear, usl.ScheduleLessonId } equals new { sl.SchoolYear, sl.ScheduleLessonId }
            join s in this.DbContext.Set<Schedule>() on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }
            join cb in this.DbContext.Set<ClassBook>() on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
            join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, cb.ClassId } equals new { cg.SchoolYear, cg.ClassId }

            select new GetLessonsInUseVO(
                sl.ScheduleLessonId,
                sl.Date,
                sl.Day,
                sl.HourNumber,
                cg.ClassName)
        ).ToArrayAsync(ct);
    }

    public async Task<GetScheduleVO> GetScheduleAsync(
        int schoolYear,
        int instId,
        int lectureScheduleId,
        CancellationToken ct)
    {
        return await this.GetScheduleInternalAsync(
            schoolYear,
            instId,
            h => h.LectureScheduleId == lectureScheduleId,
            h => true,
            ct);
    }

    public async Task<GetScheduleVO> GetTeacherScheduleForPeriodAsync(
        int schoolYear,
        int instId,
        int teacherPersonId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct)
    {
        var offDays = (await (
            from odd in this.DbContext.Set<ClassBookOffDayDate>()
            join od in this.DbContext.Set<OffDay>() on new { odd.SchoolYear, odd.OffDayId } equals new { od.SchoolYear, od.OffDayId }
            where
                od.SchoolYear == schoolYear &&
                od.InstId == instId &&
                odd.Date >= startDate &&
                odd.Date <= endDate

            select new
            {
                odd.ClassBookId,
                odd.Date
            }
        )
        .ToArrayAsync(ct))
        .Select(a => (a.ClassBookId, a.Date))
        .ToHashSet();

        return await this.GetScheduleInternalAsync(
            schoolYear,
            instId,
            h =>
                h.LectureScheduleId == null &&
                string.IsNullOrEmpty(h.ExtTeacherName) &&
                ((h.TeacherAbsenceId == null && h.CurriculumTeachers.Any(t => t.TeacherPersonId == teacherPersonId)) ||
                 (h.TeacherAbsenceId != null && h.ReplTeacher!.TeacherPersonId == teacherPersonId)) &&
                h.Date >= startDate.Date &&
                h.Date <= endDate.Date,
            h => !offDays.Contains((h.ClassBookId, h.Date)),
            ct);
    }

    public async Task<GetScheduleVO> GetTeacherScheduleForLectureScheduleAsync(
        int schoolYear,
        int instId,
        int lectureScheduleId,
        CancellationToken ct)
    {
        var lectureSchedule = await this.DbContext.Set<LectureSchedule>()
            .SingleAsync(lc => lc.SchoolYear == schoolYear && lc.InstId == instId && lc.LectureScheduleId == lectureScheduleId, ct);

        var offDays = (await (
            from odd in this.DbContext.Set<ClassBookOffDayDate>()
            join od in this.DbContext.Set<OffDay>() on new { odd.SchoolYear, odd.OffDayId } equals new { od.SchoolYear, od.OffDayId }
            where
                od.SchoolYear == schoolYear &&
                od.InstId == instId &&
                odd.Date >= lectureSchedule.StartDate &&
                odd.Date <= lectureSchedule.EndDate

            select new
            {
                odd.ClassBookId,
                odd.Date
            }
        )
        .ToArrayAsync(ct))
        .Select(a => (a.ClassBookId, a.Date))
        .ToHashSet();

        return await this.GetScheduleInternalAsync(
            schoolYear,
            instId,
            h =>
                h.LectureScheduleId == lectureScheduleId ||
                (h.LectureScheduleId == null &&
                    (h.CurriculumTeachers.Any(t => t.TeacherPersonId == lectureSchedule.TeacherPersonId) ||
                        (h.IsEmptyHour == false && h.ReplTeacher!.TeacherPersonId == lectureSchedule.TeacherPersonId)) &&
                    h.Date >= lectureSchedule.StartDate &&
                    h.Date <= lectureSchedule.EndDate),
            h => h.LectureScheduleId == lectureScheduleId || !offDays.Contains((h.ClassBookId, h.Date)),
            ct);
    }

    public async Task<bool> HasInvalidClassBooksForLectureScheduleAsync(
        int schoolYear,
        int instId,
        int lectureScheduleId,
        CancellationToken ct)
        => await (
            from lsh in this.DbContext.Set<LectureScheduleHour>()

            join sl in this.DbContext.Set<ScheduleLesson>()
                on new { lsh.SchoolYear, lsh.ScheduleLessonId }
                equals new { sl.SchoolYear, sl.ScheduleLessonId }

            join s in this.DbContext.Set<Schedule>()
                on new { sl.SchoolYear, sl.ScheduleId }
                equals new { s.SchoolYear, s.ScheduleId }

            join cb in this.DbContext.Set<ClassBook>()
                on new { s.SchoolYear, s.ClassBookId }
                equals new { cb.SchoolYear, cb.ClassBookId }

            where lsh.SchoolYear == schoolYear &&
                lsh.LectureScheduleId == lectureScheduleId &&
                cb.InstId == instId &&
                !cb.IsValid

            select lsh.LectureScheduleId
        ).AnyAsync(ct);

    private async Task<GetScheduleVO> GetScheduleInternalAsync(
        int schoolYear,
        int instId,
        Expression<Func<SchedulesQueryHelper.GetScheduleHoursVO, bool>> scheduleHourPredicate,
        Func<SchedulesQueryHelper.GetScheduleTableVOSlotHour, bool> scheduleTableSlotHourPredicate,
        CancellationToken ct)
    {
        var scheduleTable =
            await this.DbContext.GetScheduleTableAsync(
                includeUsedHoursInfo: true,
                schoolYear,
                cb => cb.InstId == instId,
                s => true,
                scheduleHourPredicate,
                ct: ct);

        return new GetScheduleVO(
            ShiftHours:
                scheduleTable.ShiftHours
                .Select(h =>
                    new GetScheduleVOShiftHour(
                        SlotNumber: h.SlotNumber,
                        StartTime: h.StartTime,
                        EndTime: h.EndTime))
                .ToArray(),
            Slots:
                scheduleTable.Slots
                .Select(sl =>
                    new GetScheduleVOSlot(
                        Day: sl.Day,
                        SlotNumber: sl.SlotNumber,
                        Hours: sl.Hours.Where(scheduleTableSlotHourPredicate)
                            .Select(h => new GetScheduleVOSlotHour(
                                ScheduleLessonId: h.ScheduleLessonId,
                                ClassBookId: h.ClassBookId,
                                ClassBookFullName: h.ClassBookFullName,
                                IsClassBookValid: h.IsClassBookValid,
                                Date: h.Date,
                                CurriculumId: h.CurriculumId,
                                CurriculumGroupName: h.CurriculumGroupName,
                                SubjectName: h.SubjectName,
                                SubjectNameShort: h.SubjectNameShort,
                                SubjectTypeName: h.SubjectTypeName,
                                IsIndividualLesson: h.IsIndividualLesson,
                                IsIndividualCurriculum:h.IsIndividualCurriculum,
                                IsIndividualSchedule:h.IsIndividualSchedule,
                                StudentPersonId: h.StudentPersonId,
                                StudentFirstName: h.StudentFirstName,
                                StudentMiddleName: h.StudentMiddleName,
                                StudentLastName: h.StudentLastName,
                                LectureScheduleId: h.LectureScheduleId,
                                IsReplTeacher: h.ReplTeacher != null,
                                ReplTeacherIsNonSpecialist: h.ReplTeacherIsNonSpecialist))
                            .ToArray()
                    ))
                .ToArray()
        );
    }
}
