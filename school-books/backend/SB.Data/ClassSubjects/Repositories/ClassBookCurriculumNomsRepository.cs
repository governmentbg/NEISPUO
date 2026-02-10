namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class ClassBookCurriculumNomsRepository : Repository, IClassBookCurriculumNomsRepository
{
    public ClassBookCurriculumNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<NomVO[]> GetNomsByIdAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int? individualCurriculumPersonId,
        int? gradeResultStudentPersonId,
        int[] ids,
        CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

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
        ).SingleAsync(ct);

        var curriculums = await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where this.DbContext.MakeIdsQuery(ids).Any(id => c.CurriculumId == id.Id)

            orderby c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours, s.SubjectName, st.Name descending

            select new
            {
                c.CurriculumId,
                IsIndividualLesson = (c.IsIndividualLesson ?? 0) != 0,
                IsIndividualCurriculum = c.IsIndividualCurriculum ?? false,
                s.SubjectName,
                SubjectTypeName = st.Name,
                CurriculumGroupName = (c.CurriculumGroupNum ?? 0) == 0 ? null : c.CurriculumGroupNum.ToString(),
                CurriculumTeachers = (
                    from t in this.DbContext.Set<CurriculumTeacher>()

                    join sp in this.DbContext.Set<StaffPosition>()
                    on t.StaffPositionId equals sp.StaffPositionId
                    into g4 from sp in g4.DefaultIfEmpty()

                    join p in this.DbContext.Set<Person>()
                    on sp.PersonId equals p.PersonId
                    into g5 from p in g5.DefaultIfEmpty()

                    where t.CurriculumId == c.CurriculumId &&
                        t.IsValid

                    select StringUtils.JoinNames(p.FirstName, p.LastName)
                ).ToArray(),
                IsRemoved = !cc.IsValid || !c.IsValid
            }
        ).ToArrayAsync(ct);

        HashSet<int> notEnrolledIndividualCurriculums = new ();
        if (individualCurriculumPersonId != null)
        {
            var individualCurriculumIds =
                curriculums
                .Where(c => c.IsIndividualCurriculum || c.IsIndividualLesson)
                .Select(c => c.CurriculumId)
                .ToArray();

            var enrolledIndividualCurriculums = await (
                from cs in this.DbContext.Set<CurriculumStudent>()

                where
                    this.DbContext.MakeIdsQuery(individualCurriculumIds).Any(id => cs.CurriculumId == id.Id) &&
                    cs.PersonId == individualCurriculumPersonId &&
                    cs.IsValid

                select
                    cs.CurriculumId
            ).ToListAsync(ct);

            notEnrolledIndividualCurriculums = individualCurriculumIds.ToHashSet();
            notEnrolledIndividualCurriculums.ExceptWith(enrolledIndividualCurriculums);
        }

        HashSet<int> notEnrolledStudentCurriculums = new ();
        if (gradeResultStudentPersonId != null)
        {
            var curriculumIds =
                curriculums
                .Select(c => c.CurriculumId)
                .ToArray();

            var enrolledIndividualCurriculums = await (
                from cs in this.DbContext.Set<CurriculumStudent>()

                where
                    this.DbContext.MakeIdsQuery(curriculumIds).Any(id => cs.CurriculumId == id.Id) &&
                    cs.PersonId == gradeResultStudentPersonId &&
                    cs.IsValid

                select
                    cs.CurriculumId
            ).ToListAsync(ct);

            notEnrolledStudentCurriculums = curriculumIds.ToHashSet();
            notEnrolledStudentCurriculums.ExceptWith(enrolledIndividualCurriculums);
        }

        return curriculums.Select(c =>
        {
            var individualLesson = c.IsIndividualLesson ? " (ИЧ)" : "";
            var individualCurriculum = c.IsIndividualCurriculum ? " (ИУП)" : "";
            var curriculumTeachers = c.CurriculumTeachers.Any() ? $" - {string.Join(", ", c.CurriculumTeachers)}" : "";
            var curriculumGroupName = c.CurriculumGroupName != null ? $" - {c.CurriculumGroupName}" : "";
            var badge =
                c.IsRemoved ? "ИЗТРИТ" :
                (individualCurriculumPersonId != null &&
                    (c.IsIndividualCurriculum || c.IsIndividualLesson) &&
                    notEnrolledIndividualCurriculums.Contains(c.CurriculumId)) ||
                (gradeResultStudentPersonId != null &&
                    notEnrolledStudentCurriculums.Contains(c.CurriculumId)) ? "НЕ СЕ ИЗУЧАВА ОТ УЧЕНИКА" : null;

            return new NomVO(
                c.CurriculumId,
                $"{c.SubjectName} / {c.SubjectTypeName}{individualLesson}{individualCurriculum}{curriculumTeachers}{curriculumGroupName}",
                badge);
        })
        .ToArray();
    }

    public async Task<NomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int? curriculumTeacherPersonId,
        int? individualCurriculumPersonId,
        int? gradeResultStudentPersonId,
        bool? excludeIndividualCurriculums,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var subjectPredicate = PredicateBuilder.True<Subject>();

        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                subjectPredicate = subjectPredicate.AndStringContains(s => s.SubjectName, word);
            }
        }

        var curriculumPredicate = PredicateBuilder.True<Curriculum>();

        if (excludeIndividualCurriculums == true)
        {
            curriculumPredicate = curriculumPredicate.And(c => c.IsIndividualCurriculum == null || c.IsIndividualCurriculum == false);
        }

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
        ).SingleAsync(ct);

        var curriculums =
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join c in this.DbContext.Set<Curriculum>().Where(curriculumPredicate)
            on cc.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>().Where(subjectPredicate)
            on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>()
            on c.SubjectTypeId equals st.SubjectTypeId

            where cc.IsValid &&
                c.IsValid

            select new
            {
                c.CurriculumId,
                IsIndividualLesson = (c.IsIndividualLesson ?? 0) != 0,
                IsIndividualCurriculum = c.IsIndividualCurriculum ?? false,
                c.CurriculumPartID,
                c.TotalTermHours,
                s.SubjectName,
                SubjectTypeName = st.Name,
                CurriculumGroupName = (c.CurriculumGroupNum ?? 0) == 0 ? null : c.CurriculumGroupNum.ToString(),
            };

        if (curriculumTeacherPersonId != null)
        {
            curriculums =
                (from c in curriculums

                join t in this.DbContext.Set<CurriculumTeacher>()
                on c.CurriculumId equals t.CurriculumId

                join sp in this.DbContext.Set<StaffPosition>()
                on t.StaffPositionId equals sp.StaffPositionId

                where sp.PersonId == curriculumTeacherPersonId &&
                    t.IsValid

                select c)
                .Union(
                    from sch in this.DbContext.Set<Schedule>()

                    join l in this.DbContext.Set<ScheduleLesson>()
                    on new { sch.SchoolYear, sch.ScheduleId }
                    equals new { l.SchoolYear, l.ScheduleId }

                    join c in this.DbContext.Set<Curriculum>() on l.CurriculumId equals c.CurriculumId

                    join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

                    join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

                    join tah in this.DbContext.Set<TeacherAbsenceHour>()
                    on new { l.SchoolYear, l.ScheduleLessonId }
                    equals new { tah.SchoolYear, tah.ScheduleLessonId }

                    where sch.SchoolYear == schoolYear
                        && sch.ClassBookId == classBookId
                        && tah.ReplTeacherPersonId == curriculumTeacherPersonId
                        && tah.ReplTeacherIsNonSpecialist == false

                    select new
                    {
                        c.CurriculumId,
                        IsIndividualLesson = (c.IsIndividualLesson ?? 0) != 0,
                        IsIndividualCurriculum = c.IsIndividualCurriculum ?? false,
                        c.CurriculumPartID,
                        c.TotalTermHours,
                        s.SubjectName,
                        SubjectTypeName = st.Name,
                        CurriculumGroupName = (c.CurriculumGroupNum ?? 0) == 0 ? null : c.CurriculumGroupNum.ToString(),
                    });
        }

        var result = await
            curriculums
            .OrderBy(c => c.CurriculumPartID)
            .ThenBy(c => c.IsIndividualLesson)
            .ThenByDescending(c => c.TotalTermHours)
            .ThenBy(c => c.SubjectName)
            .ThenBy(c => c.SubjectTypeName)
            .ToArrayAsync(ct);

        if (individualCurriculumPersonId != null)
        {
            var individualCurriculumIds =
                result
                .Where(c => c.IsIndividualCurriculum || c.IsIndividualLesson)
                .Select(c => c.CurriculumId)
                .ToArray();

            var enrolledIndividualCurriculums = (await (
                from cs in this.DbContext.Set<CurriculumStudent>()

                where
                    this.DbContext.MakeIdsQuery(individualCurriculumIds).Any(id => cs.CurriculumId == id.Id) &&
                    cs.PersonId == individualCurriculumPersonId &&
                    cs.IsValid

                select
                    cs.CurriculumId
            ).ToListAsync(ct)).ToHashSet();

            result = result
                .Where(c =>
                    (!c.IsIndividualCurriculum && !c.IsIndividualLesson) ||
                    enrolledIndividualCurriculums.Contains(c.CurriculumId))
                .ToArray();
        }

        result = result
            .WithOffsetAndLimit(offset, limit)
            .ToArray();

        var curriculumIds = result.Select(c => c.CurriculumId).ToArray();
        var curriculumTeacherNames =
            (await (
                from t in this.DbContext.Set<CurriculumTeacher>()

                join sp in this.DbContext.Set<StaffPosition>()
                on t.StaffPositionId equals sp.StaffPositionId
                into g4 from sp in g4.DefaultIfEmpty()

                join p in this.DbContext.Set<Person>()
                on sp.PersonId equals p.PersonId
                into g5 from p in g5.DefaultIfEmpty()

                where this.DbContext.MakeIdsQuery(curriculumIds)
                    .Any(c => t.CurriculumId == c.Id && t.IsValid)

                select new
                {
                    t.CurriculumId,
                    TeacherName = StringUtils.JoinNames(p.FirstName, p.LastName)
                }
            ).ToArrayAsync(ct))
            .ToLookup(c => c.CurriculumId, c => c.TeacherName);

        HashSet<int> notEnrolledStudentCurriculums = new ();
        if (gradeResultStudentPersonId != null)
        {
            var enrolledIndividualCurriculums = await (
                from cs in this.DbContext.Set<CurriculumStudent>()

                where
                    this.DbContext.MakeIdsQuery(curriculumIds).Any(id => cs.CurriculumId == id.Id) &&
                    cs.PersonId == gradeResultStudentPersonId &&
                    cs.IsValid

                select
                    cs.CurriculumId
            ).ToListAsync(ct);

            notEnrolledStudentCurriculums = curriculumIds.ToHashSet();
            notEnrolledStudentCurriculums.ExceptWith(enrolledIndividualCurriculums);
        }

        return result
            .Select(c =>
            {
                var individualLesson = c.IsIndividualLesson ? " (ИЧ)" : "";
                var individualCurriculum = c.IsIndividualCurriculum ? " (ИУП)" : "";
                var curriculumTeachers = curriculumTeacherNames[c.CurriculumId].Any() ? $" - {string.Join(", ", curriculumTeacherNames[c.CurriculumId])}" : "";
                var curriculumGroupName = c.CurriculumGroupName != null ? $" - {c.CurriculumGroupName}" : "";
                var badge =
                    (gradeResultStudentPersonId != null &&
                        notEnrolledStudentCurriculums.Contains(c.CurriculumId)) ? "НЕ СЕ ИЗУЧАВА ОТ УЧЕНИКА" : null;

                return new NomVO(
                    c.CurriculumId,
                    $"{c.SubjectName} / {c.SubjectTypeName}{individualLesson}{individualCurriculum}{curriculumTeachers}{curriculumGroupName}",
                    badge);
            })
            .ToArray();
    }
}
