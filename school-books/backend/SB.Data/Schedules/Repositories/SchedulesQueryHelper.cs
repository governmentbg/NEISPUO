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

internal static partial class SchedulesQueryHelper
{
    public static IQueryable<GetScheduleHoursVO> GetScheduleHours(
        this DbContext dbContext,
        bool includeUsedHoursInfo,
        int schoolYear,
        Expression<Func<ClassBook, bool>> classBookPredicate,
        Expression<Func<Schedule, bool>> schedulePredicate)
    {
        IQueryable<int?> scheduleLessonRelatedData;
        if(!includeUsedHoursInfo)
        {
            scheduleLessonRelatedData =
                dbContext.Set<Grade>()
                .Where(g => false)
                .Select(g => g.ScheduleLessonId);
        }
        else
        {
            // Use Union instead of multiple Concat for better query plan
            scheduleLessonRelatedData =
                dbContext.Set<Grade>()
                .Where(g => g.SchoolYear == schoolYear)
                .Select(g => g.ScheduleLessonId)
                .Union(
                    dbContext.Set<Absence>()
                    .Where(a => a.SchoolYear == schoolYear)
                    .Select(a => (int?)a.ScheduleLessonId))
                .Union(
                    dbContext.Set<Topic>()
                    .Where(t => t.SchoolYear == schoolYear)
                    .Select(t => (int?)t.ScheduleLessonId));
        }

        return
            from cb in dbContext.Set<ClassBook>().AsNoTracking().Where(classBookPredicate)

            join sch in dbContext.Set<Schedule>().AsNoTracking().Where(schedulePredicate)
            on new { cb.SchoolYear, cb.ClassBookId } equals new { sch.SchoolYear, sch.ClassBookId }

            join stud in dbContext.Set<Person>().AsNoTracking()
            on new { PersonId = sch.PersonId } equals new { PersonId = (int?)stud.PersonId }
            into g1 from student in g1.DefaultIfEmpty()

            join sl in dbContext.Set<ScheduleLesson>().AsNoTracking()
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join sh in dbContext.Set<ScheduleHour>().AsNoTracking()
            on new { sl.SchoolYear, sl.ScheduleId, sl.Day, sl.HourNumber, sl.CurriculumId } equals new { sh.SchoolYear, sh.ScheduleId, sh.Day, sh.HourNumber, sh.CurriculumId }

            join c in dbContext.Set<Curriculum>().AsNoTracking() on sl.CurriculumId equals c.CurriculumId

            join s in dbContext.Set<Subject>().AsNoTracking() on c.SubjectId equals s.SubjectId

            join st in dbContext.Set<SubjectType>().AsNoTracking() on c.SubjectTypeId equals st.SubjectTypeId

            join lsh in dbContext.Set<LectureScheduleHour>().AsNoTracking()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { lsh.SchoolYear, lsh.ScheduleLessonId }
            into g2 from lsh in g2.DefaultIfEmpty()

            join tah in dbContext.Set<TeacherAbsenceHour>().AsNoTracking()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
            into g3 from tah in g3.DefaultIfEmpty()

            join repl in dbContext.Set<Person>().AsNoTracking()
            on new { PersonId = tah.ReplTeacherPersonId } equals new { PersonId = (int?)repl.PersonId }
            into g4 from repl in g4.DefaultIfEmpty()

            where cb.SchoolYear == schoolYear

            orderby sl.Date, sl.HourNumber

            select new GetScheduleHoursVO
            {
                BookType = cb.BookType,
                BasicClassId = cb.BasicClassId,
                ClassBookId = cb.ClassBookId,
                ClassBookFullName = cb.FullBookName,
                IsClassBookValid = cb.IsValid,
                ScheduleLessonId = sl.ScheduleLessonId,
                Date = sl.Date,
                Day = sl.Day,
                HourNumber = sl.HourNumber,
                ShiftId = sch.ShiftId,
                CurriculumId = c.CurriculumId,
                CurriculumGroupName = (c.CurriculumGroupNum ?? 0) == 0 ? null : c.CurriculumGroupNum.ToString(),
                SubjectName = s.SubjectName,
                SubjectNameShort = s.SubjectNameShort,
                SubjectTypeName = st.Name,
                IsIndividualLesson = (c.IsIndividualLesson ?? 0) != 0,
                IsIndividualCurriculum = c.IsIndividualCurriculum ?? false,
                IsIndividualSchedule = sch.IsIndividualSchedule,
                StudentPersonId = sch.PersonId,
                StudentFirstName = student.FirstName,
                StudentMiddleName = student.MiddleName,
                StudentLastName = student.LastName,

                CurriculumTeachers = (
                    from t in dbContext.Set<CurriculumTeacher>().AsNoTracking()
                        // Getting only valid teachers for the date of the lesson or is marked as no replacement
                        .Where(t =>
                            (t.SchoolYear <= 2022 && t.IsValid) ||
                            (t.SchoolYear > 2022 &&
                             (t.StaffPositionStartDate ?? t.ValidFrom ?? new DateTime(schoolYear, 9, 1)) <= sl.Date &&
                             (t.StaffPositionTerminationDate == null || t.StaffPositionTerminationDate >= sl.Date || t.NoReplacement)))

                    join sp in dbContext.Set<StaffPosition>().AsNoTracking()
                    on t.StaffPositionId equals sp.StaffPositionId

                    join p in dbContext.Set<Person>().AsNoTracking()
                    on sp.PersonId equals p.PersonId

                    where t.CurriculumId == c.CurriculumId

                    select new GetScheduleHoursVOTeacher
                    {
                        TeacherPersonId = p.PersonId,
                        TeacherFirstName = p.FirstName,
                        TeacherLastName = p.LastName,
                        ActiveAtLessonTime =
                            (t.StaffPositionStartDate ?? t.ValidFrom ?? new DateTime(schoolYear, 9, 1)) <= sl.Date &&
                            (t.StaffPositionTerminationDate == null || t.StaffPositionTerminationDate >= sl.Date),
                        MarkedAsNoReplacement = t.NoReplacement
                    }
                ).ToArray(),

                TeacherAbsenceId = tah.TeacherAbsenceId,
                ReplTeacher = tah.ReplTeacherPersonId != null ?
                    new GetScheduleHoursVOTeacher
                    {
                        TeacherPersonId = tah.ReplTeacherPersonId.Value,
                        TeacherFirstName = repl.FirstName,
                        TeacherLastName = repl.LastName
                    }
                    : null,
                ReplTeacherIsNonSpecialist = tah.ReplTeacherIsNonSpecialist,
                ExtTeacherName = tah.ExtReplTeacherName,
                IsEmptyHour = tah != null ? string.IsNullOrEmpty(tah.ExtReplTeacherName) && tah.ReplTeacherPersonId == null : null,
                IsInUse = scheduleLessonRelatedData.Any(scheduleLessonId => scheduleLessonId == sl.ScheduleLessonId) || sl.ScheduleLessonId == lsh.ScheduleLessonId,
                LectureScheduleId = lsh.LectureScheduleId,
                Location = sh.Location
            };
    }

    public static async Task<GetScheduleTableVO> GetScheduleTableAsync(
        this DbContext dbContext,
        bool includeUsedHoursInfo,
        int schoolYear,
        Expression<Func<ClassBook, bool>> classBookPredicate,
        Expression<Func<Schedule, bool>> schedulePredicate,
        Expression<Func<GetScheduleHoursVO, bool>> scheduleHourPredicate,
        Expression<Func<CurriculumStudent, bool>>? curriculumStudentPredicate = null,
        CancellationToken ct = default)
    {
        var scheduleQuery = dbContext
            .GetScheduleHours(
                includeUsedHoursInfo,
                schoolYear,
                classBookPredicate,
                schedulePredicate)
            .Where(scheduleHourPredicate);

        // Get only hours for specific student in student info schedule or parent profile
        if (curriculumStudentPredicate != null)
        {
            scheduleQuery = scheduleQuery
                .Where(q =>
                    dbContext.Set<CurriculumStudent>()
                        .AsNoTracking()
                        .Where(curriculumStudentPredicate)
                        .Any(cs => cs.CurriculumId == q.CurriculumId && cs.IsValid)
                );
        }

        // Use AsNoTracking and ToListAsync for better performance
        var scheduleHours = await scheduleQuery.AsNoTracking().ToArrayAsync(ct);

        var shiftIds = scheduleHours.Select(h => h.ShiftId).Distinct().ToArray();

        var shiftHours = await (
            from s in dbContext.Set<Shift>().AsNoTracking()

            join sh in dbContext.Set<ShiftHour>().AsNoTracking()
            on new { s.SchoolYear, s.ShiftId } equals new { sh.SchoolYear, sh.ShiftId }

            where s.SchoolYear == schoolYear &&
                dbContext
                    .MakeIdsQuery(shiftIds)
                    .Any(id => s.ShiftId == id.Id)

            select new
            {
                s.ShiftId,
                sh.Day,
                sh.HourNumber,
                sh.StartTime,
                sh.EndTime
            }
        ).ToListAsync(ct);

        // Create dictionary for O(1) lookups
        var shiftHourToSlot = new Dictionary<(int ShiftId, int Day, int HourNumber), int>();
        
        var combinedShiftHours = shiftHours
            .Select(h =>
                new
                {
                    h.ShiftId,
                    h.Day,
                    h.HourNumber,
                    StartTime = h.StartTime.ToString(@"hh\:mm"),
                    EndTime = h.EndTime.ToString(@"hh\:mm"),
                })
            .GroupBy(h => new { h.StartTime, h.EndTime })
            .OrderBy(g => g.Key.StartTime)
            .ThenBy(g => g.Key.EndTime)
            .Select((g, i) =>
            {
                var slotNumber = i;
                // Populate lookup dictionary
                foreach (var h in g)
                {
                    shiftHourToSlot[(h.ShiftId, h.Day, h.HourNumber)] = slotNumber;
                }
                return new
                {
                    SlotNumber = slotNumber,
                    g.Key.StartTime,
                    g.Key.EndTime
                };
            })
            .ToList();

        var scheduleHoursBySlot =
            scheduleHours.GroupBy(h => new
            {
                h.Day,
                SlotNumber = shiftHourToSlot[(h.ShiftId, h.Day, h.HourNumber)]
            })
            .ToDictionary(
                g => g.Key,
                g => g
                    .Select(h => new GetScheduleTableVOSlotHour(
                        BookType: h.BookType,
                        BasicClassId: h.BasicClassId,
                        ClassBookId: h.ClassBookId,
                        ScheduleLessonId: h.ScheduleLessonId,
                        ClassBookFullName: h.ClassBookFullName,
                        IsClassBookValid: h.IsClassBookValid,
                        Date: h.Date,
                        CurriculumId: h.CurriculumId,
                        CurriculumGroupName: h.CurriculumGroupName,
                        SubjectName: h.SubjectName,
                        SubjectNameShort: h.SubjectNameShort,
                        SubjectTypeName: h.SubjectTypeName,
                        IsIndividualLesson: h.IsIndividualLesson,
                        IsIndividualCurriculum: h.IsIndividualCurriculum,
                        IsIndividualSchedule: h.IsIndividualSchedule,
                        StudentPersonId: h.StudentPersonId,
                        StudentFirstName: h.StudentFirstName,
                        StudentMiddleName: h.StudentMiddleName,
                        StudentLastName: h.StudentLastName,
                        CurriculumTeachers: h.CurriculumTeachers
                            .Select(t => new GetScheduleTableVOSlotHourTeacher(
                                t.TeacherPersonId,
                                t.TeacherFirstName,
                                t.TeacherLastName,
                                t.ActiveAtLessonTime,
                                t.MarkedAsNoReplacement))
                            .ToArray(),
                        TeacherAbsenceId: h.TeacherAbsenceId,
                        LectureScheduleId: h.LectureScheduleId,
                        ReplTeacher: h.ReplTeacher == null
                            ? null
                            : new GetScheduleTableVOSlotHourTeacher(
                                h.ReplTeacher.TeacherPersonId,
                                h.ReplTeacher.TeacherFirstName,
                                h.ReplTeacher.TeacherLastName,
                                true,
                                false),
                        ReplTeacherIsNonSpecialist: h.ReplTeacherIsNonSpecialist,
                        ExtTeacherName: h.ExtTeacherName,
                        IsEmptyHour: h.IsEmptyHour,
                        IsInUse: h.IsInUse,
                        Location: h.Location
                    ))
                    .ToArray()
            );

        var scheduleIncludesWeekend = scheduleHours.Any(sh => sh.Day > 5);
        var dayCount = scheduleIncludesWeekend ? 7 : 5;
        var slotCount = combinedShiftHours.Count;

        // Optimize slot generation - build array directly
        var slots = new GetScheduleTableVOSlot[dayCount * slotCount];
        var index = 0;
        
        for (int day = 1; day <= dayCount; day++)
        {
            for (int slot = 0; slot < slotCount; slot++)
            {
                var key = new { Day = day, SlotNumber = slot };
                slots[index++] = new GetScheduleTableVOSlot(
                    Day: day,
                    SlotNumber: slot,
                    Hours: scheduleHoursBySlot.GetValueOrDefault(
                        key,
                        Array.Empty<GetScheduleTableVOSlotHour>())
                );
            }
        }

        return new GetScheduleTableVO(
            ShiftHours:
                combinedShiftHours
                .Select(h =>
                    new GetScheduleTableVOShiftHour(
                        SlotNumber: h.SlotNumber,
                        StartTime: h.StartTime,
                        EndTime: h.EndTime))
                .ToArray(),
            Slots: slots
        );
    }
}
