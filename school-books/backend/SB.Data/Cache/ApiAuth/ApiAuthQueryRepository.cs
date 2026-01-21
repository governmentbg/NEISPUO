namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;

internal class ApiAuthQueryRepository : Repository, IApiAuthQueryRepository
{
    public ApiAuthQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<bool> GetInstitutionBelongsToRegionAsync(
        int instId,
        int regionId,
        CancellationToken ct)
    {
        return await (
            from id in this.DbContext.Set<InstitutionDepartment>()

            join t in this.DbContext.Set<Town>() on id.TownId equals t.TownId

            join m in this.DbContext.Set<Municipality>() on t.MunicipalityId equals m.MunicipalityId

            where id.InstitutionId == instId &&
                m.RegionId == regionId

            select id.InstitutionDepartmentId
        ).AnyAsync(ct);
    }

    public async Task<bool> GetClassBookBelongsToInstitutionAsync(
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassBookId == classBookId
            select cb.ClassBookId
        ).AnyAsync(ct);
    }

    public async Task<bool> GetPersonIsClassBookLeadTeacherAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.BasicClassId,
                cb.ClassId,
                cb.ClassIsLvl2,
                cb.BookType
            }
        ).SingleOrDefaultAsync(ct);

        if (classBook == null)
        {
            return false;
        }

        var curriculumPredicate = PredicateBuilder.True<Curriculum>();
        var psbaPredicate = PredicateBuilder.True<PersonnelSchoolBookAccess>();
        if (classBook.BookType != ClassBookType.Book_PG &&
            classBook.BookType != ClassBookType.Book_CDO &&
            classBook.BookType != ClassBookType.Book_DPLR &&
            classBook.BookType != ClassBookType.Book_CSOP)
        {
            curriculumPredicate = curriculumPredicate
                .And(c => c.SubjectId == Subject.SchoolLeadTeacherHour);
            psbaPredicate = psbaPredicate
                .And(psba =>
                    psba.SchoolYear == schoolYear &&
                    psba.ClassBookId == classBookId &&
                    psba.PersonId == personId &&
                    psba.HasAdminAccess);
        }
        else
        {
            // HasAdminAccess is not used for PG, CDO, DPLR, CSOP
            psbaPredicate = psbaPredicate
                .And(psba =>
                    psba.SchoolYear == schoolYear &&
                    psba.ClassBookId == classBookId &&
                    psba.PersonId == personId);
        }

        var hasLеadTeacherHours =
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join c in this.DbContext.Set<Curriculum>().Where(curriculumPredicate) on cc.CurriculumId equals c.CurriculumId

            join t in this.DbContext.Set<CurriculumTeacher>() on cc.CurriculumId equals t.CurriculumId

            join sp in this.DbContext.Set<StaffPosition>() on t.StaffPositionId equals sp.StaffPositionId

            where sp.PersonId == personId &&
                cc.IsValid &&
                c.IsValid &&
                t.IsValid

            select 1;

        var hasPersonnelSchoolBookAccess =
            from psba in this.DbContext.Set<PersonnelSchoolBookAccess>().Where(psbaPredicate)

            select 1;

        return await hasLеadTeacherHours
            .Concat(hasPersonnelSchoolBookAccess)
            .AnyAsync(ct);
    }

    public async Task<bool> GetPersonIsClassBookTeacherAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct)
    {
        var teacherCurriculumClassBooks =
            from tcb in this.DbContext.Set<TeacherCurriculumClassBooksView>()

            where
                tcb.PersonId == personId &&
                tcb.SchoolYear == schoolYear &&
                tcb.InstId == instId &&
                tcb.ClassBookId == classBookId

            select tcb.ClassBookId;

        var teacherSupportClassBooks =
            from st in this.DbContext.Set<SupportTeacher>()

            join s in this.DbContext.Set<Support>()
            on new { st.SchoolYear, st.SupportId } equals new { s.SchoolYear, s.SupportId }

            join cb in this.DbContext.Set<ClassBook>()
            on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            where
                st.SchoolYear == schoolYear &&
                st.PersonId == personId &&
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId

            select cb.ClassBookId;

        var teacherAbsenceHourClassBooks =
            from tah in this.DbContext.Set<TeacherAbsenceHour>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { tah.SchoolYear, tah.ScheduleLessonId } equals new { sl.SchoolYear, sl.ScheduleLessonId }

            join s in this.DbContext.Set<Schedule>()
            on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }

            join cb in this.DbContext.Set<ClassBook>()
            on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            where
                tah.SchoolYear == schoolYear &&
                tah.ReplTeacherPersonId != null &&
                tah.ReplTeacherPersonId == personId &&
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassBookId == classBookId

            select cb.ClassBookId;

        var teacherPersonnelSchoolBookAccessClassBooks =
            from psba in this.DbContext.Set<PersonnelSchoolBookAccess>()

            join cb in this.DbContext.Set<ClassBook>()
            on new { psba.SchoolYear, psba.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            where
                psba.PersonId == personId &&
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId

            select cb.ClassBookId;

        return await
            teacherCurriculumClassBooks
            .Concat(teacherSupportClassBooks)
            .Concat(teacherAbsenceHourClassBooks)
            .Concat(teacherPersonnelSchoolBookAccessClassBooks)
            .AnyAsync(ct);
    }

    public async Task<bool> GetPersonIsClassBookCurriculumTeacherAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int curriculumId,
        CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleOrDefaultAsync(ct);

        if (classBook == null)
        {
            return false;
        }

        return await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join t in this.DbContext.Set<CurriculumTeacher>() on cc.CurriculumId equals t.CurriculumId

            join sp in this.DbContext.Set<StaffPosition>() on t.StaffPositionId equals sp.StaffPositionId

            where sp.PersonId == personId &&
                t.CurriculumId == curriculumId &&
                cc.IsValid &&
                t.IsValid
            select sp.StaffPositionId
        ).AnyAsync(ct);
    }

    public async Task<bool> GetPersonIsClassBookLessonTeacherAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleLessonId,
        CancellationToken ct)
    {
        var isCurriculumTeacher =
            from cb in this.DbContext.Set<ClassBook>()

            join s in this.DbContext.Set<Schedule>() on new { cb.SchoolYear, cb.ClassBookId } equals new { s.SchoolYear, s.ClassBookId }

            join sl in this.DbContext.Set<ScheduleLesson>() on new { s.SchoolYear, s.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join t in this.DbContext.Set<CurriculumTeacher>() on sl.CurriculumId equals t.CurriculumId

            join sp in this.DbContext.Set<StaffPosition>() on t.StaffPositionId equals sp.StaffPositionId

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassBookId == classBookId &&
                sl.ScheduleLessonId == scheduleLessonId &&
                sp.PersonId == personId &&
                t.IsValid

            select sl.ScheduleLessonId;

        var isReplTeacher =
            from cb in this.DbContext.Set<ClassBook>()

            join s in this.DbContext.Set<Schedule>() on new { cb.SchoolYear, cb.ClassBookId } equals new { s.SchoolYear, s.ClassBookId }

            join sl in this.DbContext.Set<ScheduleLesson>() on new { s.SchoolYear, s.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join tah in this.DbContext.Set<TeacherAbsenceHour>() on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassBookId == classBookId &&
                sl.ScheduleLessonId == scheduleLessonId &&
                tah.ReplTeacherPersonId == personId

            select sl.ScheduleLessonId;

        var isTopicTeacher =
            from cb in this.DbContext.Set<ClassBook>()

            join s in this.DbContext.Set<Schedule>() on new { cb.SchoolYear, cb.ClassBookId } equals new { s.SchoolYear, s.ClassBookId }

            join sl in this.DbContext.Set<ScheduleLesson>() on new { s.SchoolYear, s.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join t in this.DbContext.Set<Topic>()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { t.SchoolYear, t.ScheduleLessonId }

            join tt in this.DbContext.Set<TopicTeacher>()
            on new { sl.SchoolYear, t.TopicId } equals new { tt.SchoolYear, tt.TopicId }

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassBookId == classBookId &&
                sl.ScheduleLessonId == scheduleLessonId &&
                tt.PersonId == personId

            select sl.ScheduleLessonId;

        return await isCurriculumTeacher.Concat(isReplTeacher).Concat(isTopicTeacher).AnyAsync(ct);
    }

    public async Task<bool> GetPersonIsInSupportTeamAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int supportId,
        CancellationToken ct)
    {
        return await (
            from st in this.DbContext.Set<SupportTeacher>()
            join s in this.DbContext.Set<Support>() on new { st.SchoolYear, st.SupportId } equals new { s.SchoolYear, s.SupportId }
            where st.SchoolYear == schoolYear &&
                st.SupportId == supportId &&
                st.PersonId == personId &&
                s.ClassBookId == classBookId
            select st
        ).AnyAsync(ct);
    }

    public async Task<bool> GetPersonIsReplTeacherForDateAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        DateTime date,
        CancellationToken ct)
    {
        return await (
            from sch in this.DbContext.Set<Schedule>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join tah in this.DbContext.Set<TeacherAbsenceHour>()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }

            where sch.SchoolYear == schoolYear &&
                sch.ClassBookId == classBookId &&
                !sch.IsIndividualSchedule &&
                sl.Date == date &&
                tah.ReplTeacherPersonId == personId
            select
                tah.TeacherAbsenceId
        ).AnyAsync(ct);
    }

    public async Task<bool> GetPersonIsReplTeacherForDateAndCurriculumAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int curriculumId,
        DateTime date,
        CancellationToken ct)
    {
        return await (
            from sch in this.DbContext.Set<Schedule>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join tah in this.DbContext.Set<TeacherAbsenceHour>()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }

            where sch.SchoolYear == schoolYear &&
                sch.ClassBookId == classBookId &&
                !sch.IsIndividualSchedule &&
                sl.CurriculumId == curriculumId &&
                sl.Date == date &&
                tah.ReplTeacherPersonId == personId
            select
                tah.TeacherAbsenceId
        ).AnyAsync(ct);
    }

    public async Task<bool> GetHisMedicalNoticeBelongsToRegionAsync(
        int hisMedicalNoticeId,
        int regionId,
        CancellationToken ct)
    {
        return await (
            from hmn in this.DbContext.Set<HisMedicalNotice>()
            join hmnsy in this.DbContext.Set<HisMedicalNoticeSchoolYear>() on hmn.HisMedicalNoticeId equals hmnsy.HisMedicalNoticeId

            join p in this.DbContext.Set<Person>()
            on new { hmn.Identifier, hmn.PersonalIdTypeId }
            equals new { Identifier = p.PersonalId, PersonalIdTypeId = p.PersonalIdType }

            join sc in this.DbContext.Set<StudentClass>()
            on new { hmnsy.SchoolYear, p.PersonId }
            equals new { sc.SchoolYear, sc.PersonId }

            join id in this.DbContext.Set<InstitutionDepartment>()
            on sc.InstitutionId equals id.InstitutionId

            join t in this.DbContext.Set<Town>()
            on id.TownId equals t.TownId

            join m in this.DbContext.Set<Municipality>()
            on t.MunicipalityId equals m.MunicipalityId

            where hmn.HisMedicalNoticeId == hisMedicalNoticeId &&
                m.RegionId == regionId

            select id.InstitutionDepartmentId
        ).AnyAsync(ct);
    }

    public async Task<bool> GetStudentBelongsToClassBookAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int personId,
        CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleOrDefaultAsync(ct);

        if (classBook == null)
        {
            return false;
        }

        return await (
            from s in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
            where s.PersonId == personId && !s.IsTransferred
            select s.PersonId
        ).AnyAsync(ct);
    }

    public async Task<bool> GetStudentsBelongsToInstitutionAsync(
        int schoolYear,
        int instId,
        int[] studentsPersonId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<StudentClass>()
            .AnyAsync(
                sc => sc.SchoolYear == schoolYear
                      && sc.InstitutionId == instId
                      && this.DbContext.MakeIdsQuery(studentsPersonId).Any((id) => sc.PersonId == id.Id)
                      && (sc.Status == StudentClassStatus.Enrolled
                          || (sc.Status == StudentClassStatus.Transferred
                              && sc.DischargeDate >= new DateTime(schoolYear, 9, 1))),
                ct);
    }
}
