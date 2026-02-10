namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IMyHourQueryRepository;

internal class MyHourQueryRepository : Repository, IMyHourQueryRepository
{
    public MyHourQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetTeacherLessonsVO[]> GetTeacherLessonsAsync(
        int schoolYear,
        int instId,
        DateTime date,
        int teacherPersonId,
        CancellationToken ct)
    {
        var baseQuery =
            from cb in this.DbContext.Set<ClassBook>()

            join sch in this.DbContext.Set<Schedule>() on new { cb.SchoolYear, cb.ClassBookId } equals new { sch.SchoolYear, sch.ClassBookId }
            join sl in this.DbContext.Set<ScheduleLesson>() on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }
            join sh in this.DbContext.Set<ShiftHour>() on new { sch.SchoolYear, sch.ShiftId, sl.Day, sl.HourNumber } equals new { sh.SchoolYear, sh.ShiftId, sh.Day, sh.HourNumber }
            join c in this.DbContext.Set<Curriculum>() on sl.CurriculumId equals c.CurriculumId
            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join p in this.DbContext.Set<Person>() on sch.PersonId equals p.PersonId
            into g0 from p in g0.DefaultIfEmpty()

            join odd in this.DbContext.Set<ClassBookOffDayDate>() on new { cb.SchoolYear, cb.ClassBookId, sl.Date } equals new { odd.SchoolYear, odd.ClassBookId, odd.Date }
            into g1 from odd in g1.DefaultIfEmpty()

            join t in this.DbContext.Set<Topic>() on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { t.SchoolYear, t.ScheduleLessonId }
            into g2 from t in g2.DefaultIfEmpty()

            where cb.SchoolYear == schoolYear &&
                  cb.InstId == instId &&
                  sl.Date == date &&
                  odd == null &&
                  cb.IsValid

            select new
            {
                classBook = cb,
                schedule = sch,
                scheduleLesson = sl,
                shiftHour = sh,
                curriculum = c,
                subject = s,
                subjectType = st,
                studentPerson = p,
                topic = t
            };


        return await (
            from query in baseQuery

            join teacher in this.DbContext.Set<CurriculumTeacher>() on query.curriculum.CurriculumId equals teacher.CurriculumId
            into ctGroup from teacher in ctGroup.DefaultIfEmpty()

            join sp in this.DbContext.Set<StaffPosition>() on teacher.StaffPositionId equals sp.StaffPositionId
            into spGroup from sp in spGroup.DefaultIfEmpty()

            join tah in this.DbContext.Set<TeacherAbsenceHour>() on new { query.scheduleLesson.SchoolYear, query.scheduleLesson.ScheduleLessonId }
                equals new { tah.SchoolYear, tah.ScheduleLessonId } into tahGroup
            from tah in tahGroup.DefaultIfEmpty()

            join tt in this.DbContext.Set<TopicTeacher>() on new { query.scheduleLesson.SchoolYear, query.topic.TopicId } equals new { tt.SchoolYear, tt.TopicId }
            into ttGroup from tt in ttGroup.DefaultIfEmpty()

            where ((sp != null && sp.PersonId == teacherPersonId && teacher.IsValid) ||
                  (tah != null && tah.ReplTeacherPersonId == teacherPersonId) ||
                  (tt != null && tt.PersonId == teacherPersonId)) &&
                  teacher != null &&
                  ((teacher.SchoolYear <= 2022 && teacher.IsValid) ||
                   (teacher.SchoolYear > 2022 &&
                    (teacher.StaffPositionStartDate ?? teacher.ValidFrom ?? new DateTime(schoolYear, 9, 1)) <= query.scheduleLesson.Date &&
                    (teacher.StaffPositionTerminationDate == null || teacher.StaffPositionTerminationDate >= query.scheduleLesson.Date)))

            select new
            {
                ScheduleLessonId = query.scheduleLesson.ScheduleLessonId,
                HourNumber = query.shiftHour.HourNumber,
                StartTime = query.shiftHour.StartTime,
                EndTime = query.shiftHour.EndTime,
                ClassBookId = query.classBook.ClassBookId,
                FullBookName = query.classBook.FullBookName,
                IsIndividualSchedule = query.schedule.IsIndividualSchedule,
                IsIndividualLesson = (query.curriculum.IsIndividualLesson ?? 0) != 0,
                IsIndividualCurriculum = query.curriculum.IsIndividualCurriculum ?? false,
                IsTaken = query.topic != null,
                StudentFirstName = query.studentPerson.FirstName,
                StudentMiddleName = query.studentPerson.MiddleName,
                StudentLastName = query.studentPerson.LastName,
                CurriculumId = query.curriculum.CurriculumId,
                CurriculumGroupName = (query.curriculum.CurriculumGroupNum ?? 0) == 0
                    ? null
                    : query.curriculum.CurriculumGroupNum.ToString(),
                SubjectName = query.subject.SubjectName,
                SubjectNameShort = query.subject.SubjectNameShort,
                SubjectTypeName = query.subjectType.Name,
                ReplTeacherIsNonSpecialist = tah != null ? tah.ReplTeacherIsNonSpecialist : null
            })
            .GroupBy(hour => new
            {
                hour.ScheduleLessonId,
                hour.HourNumber,
                hour.StartTime,
                hour.EndTime,
                hour.ClassBookId,
                hour.FullBookName,
                hour.IsIndividualSchedule,
                hour.IsIndividualLesson,
                hour.IsIndividualCurriculum,
                hour.IsTaken,
                hour.StudentFirstName,
                hour.StudentMiddleName,
                hour.StudentLastName,
                hour.CurriculumId,
                hour.CurriculumGroupName,
                hour.SubjectName,
                hour.SubjectNameShort,
                hour.SubjectTypeName
            })
            .Select(group => new
            {
                group.Key.ScheduleLessonId,
                group.Key.HourNumber,
                group.Key.StartTime,
                group.Key.EndTime,
                group.Key.ClassBookId,
                group.Key.FullBookName,
                group.Key.IsIndividualSchedule,
                group.Key.IsIndividualLesson,
                group.Key.IsIndividualCurriculum,
                group.Key.IsTaken,
                group.Key.StudentFirstName,
                group.Key.StudentMiddleName,
                group.Key.StudentLastName,
                group.Key.CurriculumId,
                group.Key.CurriculumGroupName,
                group.Key.SubjectName,
                group.Key.SubjectNameShort,
                group.Key.SubjectTypeName,
                ReplTeacherIsNonSpecialist = group.Where(x => x.ReplTeacherIsNonSpecialist != null)
                    .Select(x => x.ReplTeacherIsNonSpecialist)
                    .FirstOrDefault()
            })
            .OrderBy(hour => hour.HourNumber)
            .ThenBy(hour => hour.StartTime)
            .ThenBy(hour => hour.EndTime)
            .Select(hour => new GetTeacherLessonsVO(
                hour.ScheduleLessonId,
                hour.HourNumber,
                hour.StartTime,
                hour.EndTime,
                hour.ClassBookId,
                hour.FullBookName,
                hour.IsIndividualSchedule,
                hour.IsIndividualLesson,
                hour.IsIndividualCurriculum,
                hour.IsTaken,
                hour.StudentFirstName,
                hour.StudentMiddleName,
                hour.StudentLastName,
                hour.CurriculumId,
                hour.CurriculumGroupName,
                hour.SubjectName,
                hour.SubjectNameShort,
                hour.SubjectTypeName,
                hour.ReplTeacherIsNonSpecialist))
            .ToArrayAsync(ct);
    }

    public async Task<GetTeacherLessonVO> GetTeacherLessonAsync(
        int schoolYear,
        int classBookId,
        int scheduleLessonId,
        CancellationToken ct)
    {
        var scheduleLesson = await (
            from sl in this.DbContext.Set<ScheduleLesson>()

            join sch in this.DbContext.Set<Schedule>()
            on new { sl.SchoolYear, sl.ScheduleId } equals new { sch.SchoolYear, sch.ScheduleId }

            join sh in this.DbContext.Set<ShiftHour>()
            on new { sch.SchoolYear, sch.ShiftId, sl.Day, sl.HourNumber } equals new { sh.SchoolYear, sh.ShiftId, sh.Day, sh.HourNumber }

            join cb in this.DbContext.Set<ClassBook>()
            on new { sch.SchoolYear, sch.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            join tah in this.DbContext.Set<TeacherAbsenceHour>()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
            into g1
            from tah in g1.DefaultIfEmpty()

            join t in this.DbContext.Set<Topic>()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { t.SchoolYear, t.ScheduleLessonId }
            into g2
            from t in g2.DefaultIfEmpty()

            where sl.SchoolYear == schoolYear &&
                sl.ScheduleLessonId == scheduleLessonId &&
                cb.ClassBookId == classBookId &&
                cb.IsValid

            select new
            {
                sl.ScheduleLessonId,
                sh.HourNumber,
                sh.StartTime,
                sh.EndTime,
                cb.ClassBookId,
                cb.ClassId,
                cb.ClassIsLvl2,
                sl.CurriculumId,
                sl.IsVerified,
                TeacherAbsenceId = (int?)tah.TeacherAbsenceId,
                IndividualCurriculumPersonId = sch.PersonId,
                TopicId = (int?)t.TopicId,
                TopicTitles = t.Titles.Select(tt => tt.Title).ToArray(),
                TopicCreateDate = (DateTime?)t.CreateDate,
                TopicCreatedBySysUserId = (int?)t.CreatedBySysUserId
            }
        ).SingleAsync(ct);

        var studentPredicate = PredicateBuilder.True<Person>();
        if (scheduleLesson.IndividualCurriculumPersonId != null)
        {
            studentPredicate = studentPredicate.And(p => p.PersonId == scheduleLesson.IndividualCurriculumPersonId);
        }

        var students = await (
            from sc in this.DbContext.StudentsForClassBook(schoolYear, scheduleLesson.ClassId, scheduleLesson.ClassIsLvl2)

            join p in this.DbContext.Set<Person>().Where(studentPredicate) on sc.PersonId equals p.PersonId

            join cs in this.DbContext.Set<CurriculumStudent>().Where(cs => cs.CurriculumId == scheduleLesson.CurriculumId && cs.IsValid)
            on p.PersonId equals cs.PersonId
            into j1
            from cs in j1.DefaultIfEmpty()

            join gs in this.DbContext.Set<ClassBookStudentGradeless>().Where(gs =>
                gs.SchoolYear == schoolYear &&
                gs.ClassBookId == scheduleLesson.ClassBookId &&
                gs.CurriculumId == scheduleLesson.CurriculumId)
            on sc.PersonId equals gs.PersonId
            into j2
            from gs in j2.DefaultIfEmpty()

            join ss in this.DbContext.Set<ClassBookStudentSpecialNeeds>().Where(ss =>
                    ss.SchoolYear == schoolYear &&
                    ss.ClassBookId == scheduleLesson.ClassBookId &&
                    ss.CurriculumId == scheduleLesson.CurriculumId)
            on sc.PersonId equals ss.PersonId
            into j3
            from ss in j3.DefaultIfEmpty()

            orderby sc.ClassNumber ?? 99999, p.FirstName, p.MiddleName, p.LastName

            select new GetTeacherLessonVOStudent(
                sc.PersonId,
                sc.ClassNumber,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                sc.IsTransferred,
                cs == null || !cs.IsValid,
                (bool?)gs.WithoutFirstTermGrade ?? false,
                (bool?)gs.WithoutSecondTermGrade ?? false,
                ss != null)
        ).ToArrayAsync(ct);

        var hasTopicPlan = await (
            from t in this.DbContext.Set<ClassBookTopicPlanItem>()
            where t.SchoolYear == schoolYear &&
                t.ClassBookId == scheduleLesson.ClassBookId &&
                t.CurriculumId == scheduleLesson.CurriculumId
            select t.CurriculumId
        ).AnyAsync(ct);

        return new GetTeacherLessonVO(
            scheduleLesson.ClassBookId,
            scheduleLesson.ScheduleLessonId,
            scheduleLesson.HourNumber,
            scheduleLesson.StartTime,
            scheduleLesson.EndTime,
            scheduleLesson.TeacherAbsenceId,
            scheduleLesson.CurriculumId,
            scheduleLesson.IndividualCurriculumPersonId,
            hasTopicPlan,
            scheduleLesson.TopicId,
            scheduleLesson.TopicTitles,
            scheduleLesson.TopicCreateDate,
            scheduleLesson.TopicCreatedBySysUserId,
            scheduleLesson.IsVerified,
            students);
    }

    public async Task<GetTeacherLessonAbsencesVO[]> GetTeacherLessonAbsencesAsync(
        int schoolYear,
        int classBookId,
        int scheduleLessonId,
        CancellationToken ct)
    {
        return await (
            from a in this.DbContext.Set<Absence>()

            join csu in this.DbContext.Set<SysUser>() on a.CreatedBySysUserId equals csu.SysUserId

            join cp in this.DbContext.Set<Person>() on csu.PersonId equals cp.PersonId

            join msu in this.DbContext.Set<SysUser>() on a.ModifiedBySysUserId equals msu.SysUserId

            join mp in this.DbContext.Set<Person>() on msu.PersonId equals mp.PersonId

            join ar in this.DbContext.Set<AbsenceReason>() on a.ExcusedReasonId equals ar.Id
            into g2
            from ar in g2.DefaultIfEmpty()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.ScheduleLessonId == scheduleLessonId

            select new GetTeacherLessonAbsencesVO(
                a.AbsenceId,
                a.PersonId,
                a.Type,
                EnumUtils.GetEnumDescription(a.Type),
                a.ExcusedReasonId,
                ar.Name,
                a.ExcusedReasonComment,
                a.CreateDate,
                a.CreatedBySysUserId,
                cp.FirstName,
                cp.MiddleName,
                cp.LastName,
                a.ModifyDate,
                a.ModifiedBySysUserId,
                mp.FirstName,
                mp.MiddleName,
                mp.LastName)
        ).ToArrayAsync(ct);
    }

    public async Task<GetTeacherLessonGradesVO[]> GetTeacherLessonGradesAsync(
        int schoolYear,
        int classBookId,
        int scheduleLessonId,
        CancellationToken ct)
    {
        var sql = """
            SELECT
                g.GradeId,
                g.PersonId,
                g.Date,
                g.Category,
                g.DecimalGrade,
                g.QualitativeGrade,
                g.SpecialGrade
            FROM
                [school_books].[Grade] g
            WHERE
                g.SchoolYear = @schoolYear AND
                g.ClassBookId = @classBookId AND
                g.ScheduleLessonId = @scheduleLessonId
            ORDER BY
                g.Date,
                g.CreateDate
            """;

        return await this.DbContext.Set<GetTeacherLessonGradesVO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId),
                new SqlParameter("scheduleLessonId", scheduleLessonId))
            .ToArrayAsync(ct);
    }
}
