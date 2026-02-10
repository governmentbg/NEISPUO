namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class ScheduleLessonNomsRepository : Repository, IScheduleLessonNomsRepository
{
    public ScheduleLessonNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<ScheduleLessonNomVO[]> GetNomsAsync(int schoolYear, int instId, int classBookId, int curriculumId, DateTime date, int? curriculumTeacherPersonId, CancellationToken ct)
    {
        var query =
            from sch in this.DbContext.Set<Schedule>()

            join cb in this.DbContext.Set<ClassBook>()
            on new { sch.SchoolYear, sch.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join c in this.DbContext.Set<Curriculum>() on sl.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join tah in this.DbContext.Set<TeacherAbsenceHour>()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
            into g1 from tah in g1.DefaultIfEmpty()

            join od in this.DbContext.Set<ClassBookOffDayDate>()
                on new { cb.SchoolYear, cb.ClassBookId, sl.Date } equals new { od.SchoolYear, od.ClassBookId , od.Date }
                into g2
            from od in g2.DefaultIfEmpty()

            join p in this.DbContext.Set<Person>()
            on sch.PersonId equals p.PersonId
            into g3 from p in g3.DefaultIfEmpty()

            where sch.SchoolYear == schoolYear &&
                sch.ClassBookId == classBookId &&
                cb.InstId == instId &&
                cb.IsValid &&
                c.CurriculumId == curriculumId &&
                sl.Date == date &&
                od == null

            select new
            {
                sl.ScheduleLessonId,
                sch.IsIndividualSchedule,
                TeacherAbsenceId = (int?)tah.TeacherAbsenceId,
                tah.ReplTeacherPersonId,
                tah.ReplTeacherIsNonSpecialist,
                tah.ExtReplTeacherName,
                sch.PersonId,
                sl.HourNumber,
                sl.Date,
                s.SubjectName,
                SubjectTypeName = st.Name,
                StudentName = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                sl.IsVerified
            };

        if (curriculumTeacherPersonId.HasValue)
        {
            query = query
                .Where(r => (
                    from ct in this.DbContext.Set<CurriculumTeacher>()

                    join sp in this.DbContext.Set<StaffPosition>() on ct.StaffPositionId equals sp.StaffPositionId
                    where
                        ct.CurriculumId == curriculumId &&
                        sp.PersonId == curriculumTeacherPersonId.Value &&
                        ct.IsValid

                    select sp
                ).Any() || (r.ReplTeacherPersonId == curriculumTeacherPersonId && r.ReplTeacherIsNonSpecialist == false));
        }
        else
        {
            query = query
                .Where(r =>
                    r.TeacherAbsenceId == null ||                                                   // no teacher absence
                    (r.ReplTeacherPersonId != null && r.ReplTeacherIsNonSpecialist == false) ||     // with teacher absence but not an empty hour and not GO/ZO/EO
                    (!string.IsNullOrEmpty(r.ExtReplTeacherName))                                   // with ext repl teacher
                 );
        }

        return await query
            .OrderBy(r => r.HourNumber)
            .Select(g =>
                new ScheduleLessonNomVO(
                    g.ScheduleLessonId,
                    g.TeacherAbsenceId,
                    g.PersonId,
                    g.IsIndividualSchedule
                        ? $"#{g.HourNumber} {g.SubjectName} / {g.SubjectTypeName} (ИУП {g.StudentName})"
                        : $"#{g.HourNumber} {g.SubjectName} / {g.SubjectTypeName}",
                    g.IsVerified))
            .ToArrayAsync(ct);
    }
}
