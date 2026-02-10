namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IClassBookCQSQueryRepository;

internal class ClassBookCQSQueryRepository : Repository, IClassBookCQSQueryRepository
{
    public ClassBookCQSQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetVO?> GetAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()

            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId

            select new GetVO(
                cb.InstId,
                cb.ClassId,
                cb.BasicClassId,
                cb.ClassIsLvl2,
                cb.BookType,
                cb.IsFinalized,
                cb.IsValid)
        ).SingleOrDefaultAsync(ct);
    }

    public async Task<GetClassBookSchoolYearSettingsVO?> GetClassBookSchoolYearSettingsAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()
            join ep in this.DbContext.Set<ClassBookExtProvider>() on new { cb.SchoolYear, cb.InstId } equals new { ep.SchoolYear, ep.InstId }
            join sy in this.DbContext.Set<ClassBookSchoolYearSettings>() on new { ep.SchoolYear, cb.ClassBookId } equals new { sy.SchoolYear, sy.ClassBookId }

            where sy.SchoolYear == schoolYear &&
                sy.ClassBookId == classBookId

            select new GetClassBookSchoolYearSettingsVO(
                sy.SchoolYearStartDateLimit,
                sy.FirstTermEndDate,
                sy.SecondTermStartDate,
                sy.SchoolYearEndDateLimit,
                ep.ExtSystemId == null && sy.HasFutureEntryLock,
                ep.ExtSystemId == null ? sy.PastMonthLockDay : null)
        ).SingleOrDefaultAsync(ct);
    }

    public async Task<int?> GetCurriculumSubjectTypeIdAsync(int schoolYear, int curriculumId, CancellationToken ct)
    {
        return await (
            from c in this.DbContext.Set<Curriculum>()

            where c.SchoolYear == schoolYear &&
                c.CurriculumId == curriculumId

            select
                c.SubjectTypeId
        ).SingleOrDefaultAsync(ct);
    }

    public async Task<int?> GetScheduleLessonTeacherAbsenceIdAsync(int schoolYear, int scheduleLessonId, CancellationToken ct)
    {
        return await (
            from tah in this.DbContext.Set<TeacherAbsenceHour>()

            where tah.SchoolYear == schoolYear &&
                tah.ScheduleLessonId == scheduleLessonId

            select
                (int?)tah.TeacherAbsenceId
        ).FirstOrDefaultAsync(ct);
    }

    public async Task<bool> ExistsCurriculumForClassBookAsync(int schoolYear, int classId, bool classIsLvl2, int curriculumId, CancellationToken ct)
    {
        return await this.DbContext.CurriculumClassForClassBook(schoolYear, classId, classIsLvl2)
            .AnyAsync(cc => cc.CurriculumId == curriculumId && cc.IsValid, ct);
    }

    public async Task<bool> ExistsSubjectForClassBookAsync(int schoolYear, int classId, bool classIsLvl2, int subjectId, CancellationToken ct)
    {
        return await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classId, classIsLvl2)
            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId
            where c.SubjectId == subjectId && cc.IsValid && c.IsValid
            select 1
        ).AnyAsync(ct);
    }

    public async Task<bool> ExistsScheduleLessonForClassBookAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        int scheduleLessonId,
        CancellationToken ct)
    {
        DateTime dateOnly = date.Date;

        return await (
            from cb in this.DbContext.Set<ClassBook>()

            join s in this.DbContext.Set<Schedule>() on new { cb.SchoolYear, cb.ClassBookId } equals new { s.SchoolYear, s.ClassBookId }

            join sl in this.DbContext.Set<ScheduleLesson>() on new { s.SchoolYear, s.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId &&
                sl.Date == dateOnly &&
                sl.ScheduleLessonId == scheduleLessonId
            select sl.ScheduleLessonId
        ).AnyAsync(ct);
    }

    public async Task<bool> ExistsScheduleLessonForClassBookAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        int scheduleLessonId,
        int personId,
        CancellationToken ct)
    {
        DateTime dateOnly = date.Date;

        return await (
            from cb in this.DbContext.Set<ClassBook>()

            join s in this.DbContext.Set<Schedule>() on new { cb.SchoolYear, cb.ClassBookId } equals new { s.SchoolYear, s.ClassBookId }

            join sl in this.DbContext.Set<ScheduleLesson>() on new { s.SchoolYear, s.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId &&
                sl.Date == dateOnly &&
                sl.ScheduleLessonId == scheduleLessonId &&
                (s.PersonId == null || s.PersonId == personId)
            select sl.ScheduleLessonId
        ).AnyAsync(ct);
    }

    public async Task<int?> GetScheduleLessonCurriculumIdForClassBookAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        int scheduleLessonId,
        int? personId,
        CancellationToken ct)
    {
        DateTime dateOnly = date.Date;

        var predicate = PredicateBuilder.True<Schedule>();
        if (personId.HasValue)
        {
            predicate = predicate.And(s => s.PersonId == null || s.PersonId == personId.Value);
        }
        else
        {
            predicate = predicate.And(s => s.PersonId == null);
        }

        return await (
            from cb in this.DbContext.Set<ClassBook>()

            join s in this.DbContext.Set<Schedule>().Where(predicate) on new { cb.SchoolYear, cb.ClassBookId } equals new { s.SchoolYear, s.ClassBookId }

            join sl in this.DbContext.Set<ScheduleLesson>() on new { s.SchoolYear, s.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId &&
                sl.Date == dateOnly &&
                sl.ScheduleLessonId == scheduleLessonId
            select (int?)sl.CurriculumId
        ).SingleOrDefaultAsync(ct);
    }

    public async Task<bool> ExistsStudentForClassBookAsync(int schoolYear, int classBookClassId, bool classBookClassIsLvl2, int personId, CancellationToken ct)
    {
        return await this.DbContext.StudentsForClassBook(schoolYear, classBookClassId, classBookClassIsLvl2)
            .Where(cc => cc.PersonId == personId)
            .AnyAsync(ct);
    }

    public async Task<bool> PersonExistsInCurriculumStudentAsync(int curriculumId, int personId, CancellationToken ct)
    {
        return await this.DbContext.Set<CurriculumStudent>()
            .Where(cs => cs.CurriculumId == curriculumId && cs.PersonId == personId)
            .AnyAsync(ct);
    }
}
