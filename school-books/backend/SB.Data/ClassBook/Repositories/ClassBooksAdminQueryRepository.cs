namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Common;
using SB.Domain;
using static SB.Domain.IClassBooksAdminQueryRepository;

internal class ClassBooksAdminQueryRepository : Repository, IClassBooksAdminQueryRepository
{
    private readonly string blobServiceHmacKey;
    private readonly string blobServicePublicWebUrl;

    public ClassBooksAdminQueryRepository(
        UnitOfWork unitOfWork,
        IOptions<DomainOptions> domainOptions)
        : base(unitOfWork)
    {
        var opts = domainOptions.Value;

        if (string.IsNullOrEmpty(opts.BlobServiceHMACKey))
        {
            throw new Exception($"{nameof(DomainOptions)}.{nameof(DomainOptions.BlobServiceHMACKey)} should have a configured value");
        }

        if (string.IsNullOrEmpty(opts.BlobServicePublicUrl))
        {
            throw new Exception($"{nameof(DomainOptions)}.{nameof(DomainOptions.BlobServicePublicUrl)} should have a configured value");
        }

        this.blobServiceHmacKey = opts.BlobServiceHMACKey;
        this.blobServicePublicWebUrl = opts.BlobServicePublicUrl;
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        ClassKind classKind,
        string? bookName,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True(new
        {
            ClassBookId = default(int),
            ClassName = default(string)!,
            BasicClassName = default(string?),
            BookName = default(string)!,
            BookType = default(ClassBookType),
            FullBookName = default(string)!,
            SortOrd = default(int?),
            IsValid = default(bool)
        });

        if (!string.IsNullOrEmpty(bookName))
        {
            predicate = predicate.AndAnyStringContains(i => i.FullBookName, i => i.ClassName, bookName);
        }

        return await (
            from cb in this.DbContext.Set<ClassBook>()

            join bc in this.DbContext.Set<BasicClass>() on cb.BasicClassId equals bc.BasicClassId
            into j1 from bc in j1.DefaultIfEmpty()

            join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId

            join ctype in this.DbContext.Set<ClassType>() on cg.ClassTypeId equals ctype.ClassTypeId
            into j2 from ctype in j2.DefaultIfEmpty()

            where cb.SchoolYear == schoolYear &&
                  cb.InstId == instId &&
                  ((ClassKind?)ctype.ClassKind ?? ClassKind.Class) == classKind

            select new
            {
                ClassBookId = cb.ClassBookId,
                ClassName = cg.ClassName,
                BasicClassName = bc.Name,
                BookName = cb.BookName,
                BookType = cb.BookType,
                FullBookName = cb.FullBookName,
                SortOrd = bc.SortOrd,
                IsValid = cb.IsValid
            })
            .Where(predicate)
            .OrderBy(i => i.IsValid ? 0 : 1)
            .ThenBy(i => i.SortOrd)
            .ThenBy(i => i.BookName)
            .Select(i => new GetAllVO(
                i.ClassBookId,
                i.ClassName,
                i.BasicClassName,
                i.BookName,
                i.BookType,
                i.IsValid))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<TableResultVO<GetAllVO>> GetAllForTeacherAsync(
        int schoolYear,
        int instId,
        ClassKind classKind,
        string? bookName,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True(new
        {
            ClassBookId = default(int),
            ClassName = default(string)!,
            BasicClassName = default(string?),
            BookName = default(string)!,
            BookType = default(ClassBookType),
            FullBookName = default(string)!,
            SortOrd = default(int?),
            IsValid = default(bool)
        });

        if (!string.IsNullOrEmpty(bookName))
        {
            predicate = predicate.AndAnyStringContains(i => i.FullBookName, i => i.ClassName, bookName);
        }

        var teacherClassBooksForLvl1 =
            from cb in this.DbContext.Set<ClassBook>()

            join bc in this.DbContext.Set<BasicClass>() on cb.BasicClassId equals bc.BasicClassId
            into j1 from bc in j1.DefaultIfEmpty()

            join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId

            join ctype in this.DbContext.Set<ClassType>() on cg.ClassTypeId equals ctype.ClassTypeId
            into j2 from ctype in j2.DefaultIfEmpty()

            where
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassIsLvl2 == false &&
                ((ClassKind?)ctype.ClassKind ?? ClassKind.Class) == classKind &&
                (
                    from cc in this.DbContext.Set<CurriculumClass>()

                    join t in this.DbContext.Set<CurriculumTeacher>() on cc.CurriculumId equals t.CurriculumId

                    join sp in this.DbContext.Set<StaffPosition>() on t.StaffPositionId equals sp.StaffPositionId

                    where
                        cc.ClassId == cb.ClassId &&
                        sp.PersonId == personId &&
                        cc.IsValid &&
                        t.IsValid

                    select sp
                ).Any()

            select new
            {
                ClassBookId = cb.ClassBookId,
                ClassName = cg.ClassName,
                BasicClassName = bc.Name,
                BookName = cb.BookName,
                BookType = cb.BookType,
                FullBookName = cb.FullBookName,
                SortOrd = bc.SortOrd,
                IsValid = cb.IsValid
            };

        var teacherClassBooksForLvl2 =
            from cb in this.DbContext.Set<ClassBook>()

            join bc in this.DbContext.Set<BasicClass>() on cb.BasicClassId equals bc.BasicClassId
            into j1 from bc in j1.DefaultIfEmpty()

            join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId

            join ctype in this.DbContext.Set<ClassType>() on cg.ClassTypeId equals ctype.ClassTypeId
            into j2 from ctype in j2.DefaultIfEmpty()

            where
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassIsLvl2 == true &&
                ((ClassKind?)ctype.ClassKind ?? ClassKind.Class) == classKind &&
                (
                    from cg in this.DbContext.Set<ClassGroup>()

                    join cc in this.DbContext.Set<CurriculumClass>() on cg.ParentClassId equals cc.ClassId

                    join t in this.DbContext.Set<CurriculumTeacher>() on cc.CurriculumId equals t.CurriculumId

                    join sp in this.DbContext.Set<StaffPosition>() on t.StaffPositionId equals sp.StaffPositionId

                    where
                        cg.ClassId == cb.ClassId &&
                        sp.PersonId == personId &&
                        cc.IsValid &&
                        t.IsValid

                    select sp
                ).Any()

            select new
            {
                ClassBookId = cb.ClassBookId,
                ClassName = cg.ClassName,
                BasicClassName = bc.Name,
                BookName = cb.BookName,
                BookType = cb.BookType,
                FullBookName = cb.FullBookName,
                SortOrd = bc.SortOrd,
                IsValid = cb.IsValid
            };

        return await teacherClassBooksForLvl1
            .Union(teacherClassBooksForLvl2)
            .Where(predicate)
            .OrderBy(cb => cb.SortOrd)
            .ThenBy(cb => cb.BookName)
            .Select(cb => new GetAllVO(
                cb.ClassBookId,
                cb.ClassName,
                cb.BasicClassName,
                cb.BookName,
                cb.BookType,
                cb.IsValid))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()
            join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, cb.ClassId } equals new { cg.SchoolYear, cg.ClassId }

            join bc in this.DbContext.Set<BasicClass>() on cg.BasicClassId equals bc.BasicClassId
            into j1 from bc in j1.DefaultIfEmpty()

            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId

            select new GetVO(
                cb.ClassBookId,
                cg.ClassName,
                bc.Name,
                cb.BookName,
                cb.BookType,
                cb.BasicClassId,
                cb.IsFinalized,
                cb.IsValid)
        ).SingleAsync(ct);
    }

    public async Task<GetInfoVO> GetInfoAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new GetInfoVO(
                cb.BookType,
                cb.FullBookName,
                cb.BasicClassId,
                cb.IsFinalized,
                cb.IsValid,
                cb.ModifyDate)
        ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetCurriculumVO>> GetCurriculumAsync(int schoolYear, int classBookId, int? offset, int? limit, CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var curriculumTeachers = (await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join t in this.DbContext.Set<CurriculumTeacher>() on cc.CurriculumId equals t.CurriculumId

            join sp in this.DbContext.Set<StaffPosition>() on t.StaffPositionId equals sp.StaffPositionId

            join p in this.DbContext.Set<Person>() on sp.PersonId equals p.PersonId

            where cc.IsValid &&
                t.IsValid

            select new
            {
                cc.CurriculumId,
                Name = StringUtils.JoinNames(p.FirstName, p.LastName)
            })
            .ToArrayAsync(ct))
            .ToLookup(t => t.CurriculumId, t => t.Name);

        return await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join cg in this.DbContext.Set<ClassBookCurriculumGradeless>().Where(cg => cg.SchoolYear == schoolYear && cg.ClassBookId == classBookId)
            on c.CurriculumId equals cg.CurriculumId
            into j1 from cg in j1.DefaultIfEmpty()

            where cc.IsValid &&
                c.IsValid

            orderby c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

            select new GetCurriculumVO(
                c.CurriculumId,
                s.SubjectName,
                st.Name,
                (c.CurriculumGroupNum ?? 0) == 0 ? null : c.CurriculumGroupNum.ToString(),
                (c.IsIndividualLesson ?? 0) != 0,
                c.IsIndividualCurriculum ?? false,
                string.Join(", ", curriculumTeachers[c.CurriculumId]),
                cg != null)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<TableResultVO<GetStudentsVO>> GetStudentsAsync(int schoolYear, int classBookId, int? offset, int? limit, CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var registeredRelatives = await (
            from sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
            join pa in this.DbContext.Set<ParentChildSchoolBookAccess>() on sc.PersonId equals pa.ChildId
            join su in this.DbContext.Set<SysUser>().Where(s => s.DeletedOn == null) on pa.ParentId equals su.PersonId
            join p in this.DbContext.Set<Person>() on pa.ParentId equals p.PersonId
            where pa.HasAccess && p.FirstName != null && p.LastName != null
            select new
            {
                IsRegistered = true,
                StudentPersonId = sc.PersonId,
                p.FirstName,
                MiddleName = p.MiddleName ?? string.Empty,
                p.LastName,
                Email = su.Username
            })
            .ToListAsync(ct);

        var unregisteredRelatives = await (
            from sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join r in this.DbContext.Set<Relative>() on sc.PersonId equals r.PersonId

            where r.FirstName != null && r.LastName != null

            select new
            {
                IsRegistered = false,
                StudentPersonId = sc.PersonId,
                FirstName = r.FirstName ?? string.Empty,
                MiddleName = r.MiddleName ?? string.Empty,
                LastName = r.LastName ?? string.Empty,
                Email = r.Email ??  string.Empty
            })
            .ToListAsync(ct);

        // We have duplicate records in unregistered and registered relatives,
        // and we need to get registered users as priority and remove duplicate or invalid records in unregisteredRelatives
        foreach (var registeredRelative in registeredRelatives)
        {
            var recordForDeletion =
                unregisteredRelatives
                    .FirstOrDefault(ur =>
                        ur.StudentPersonId == registeredRelative.StudentPersonId &&
                        (registeredRelative.Email.Trim().Equals(ur.Email.Trim(), StringComparison.OrdinalIgnoreCase) ||
                         (registeredRelative.FirstName.Trim().Equals(ur.FirstName.Trim(), StringComparison.OrdinalIgnoreCase) &&
                          registeredRelative.LastName.Trim().Equals(ur.LastName.Trim(), StringComparison.OrdinalIgnoreCase))));


            if (recordForDeletion != null)
            {
                unregisteredRelatives.Remove(recordForDeletion);
            }
        }

        var allRelatives = registeredRelatives
            .Union(unregisteredRelatives)
            .GroupBy(t => new
            {
                t.StudentPersonId,
                Name = StringUtils.JoinNames(t.FirstName, t.MiddleName, t.LastName),
                t.IsRegistered
            })
            .ToLookup(
                g => g.Key.StudentPersonId,
                g => new GetStudentsVOParent(
                g.Key.Name,
                g.Where(x => !string.IsNullOrWhiteSpace(x.Email))
                 .Select(x => new GetStudentsVOParentEmail(
                     x.Email.Trim(),
                     x.IsRegistered
                 ))
                 .DistinctBy(e => new { e.Name, e.IsRegistered })
                 .ToArray()
                )
            );

        return await (
            from s in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join p in this.DbContext.Set<Person>() on s.PersonId equals p.PersonId

            join sp in this.DbContext.Set<SPPOOSpeciality>() on s.StudentSpecialityId equals sp.SPPOOSpecialityId
            into j0 from sp in j0.DefaultIfEmpty()

            join a in this.DbContext.Set<ClassBookStudentActivity>().Where(a => a.SchoolYear == schoolYear && a.ClassBookId == classBookId)
            on s.PersonId equals a.PersonId
            into j1 from a in j1.DefaultIfEmpty()

            orderby (s.ClassNumber == null ? 99999 : s.ClassNumber), p.FirstName, p.MiddleName, p.LastName

            select new GetStudentsVO(
                s.PersonId,
                s.ClassNumber,
                s.IsTransferred,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                this.DbContext.Set<ClassBookStudentSpecialNeeds>()
                    .Any(cbssn =>
                        cbssn.SchoolYear == schoolYear &&
                        cbssn.ClassBookId == classBookId &&
                        cbssn.PersonId == s.PersonId)
                    ||
                this.DbContext.Set<ClassBookStudentFirstGradeResultSpecialNeeds>()
                    .Any(cbssn =>
                        cbssn.SchoolYear == schoolYear &&
                        cbssn.ClassBookId == classBookId &&
                        cbssn.PersonId == s.PersonId),
                this.DbContext.Set<ClassBookStudentGradeless>()
                    .Any(cbsg =>
                        cbsg.SchoolYear == schoolYear &&
                        cbsg.ClassBookId == classBookId &&
                        cbsg.PersonId == s.PersonId),
                a.Activities,
                sp.Name,
                allRelatives[s.PersonId].ToArray())
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetStudentVO> GetStudentAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var student = await (
            from sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId

            join s in this.DbContext.Set<SPPOOSpeciality>()
            on sc.StudentSpecialityId equals s.SPPOOSpecialityId
            into j1 from s in j1.DefaultIfEmpty()

            join a in this.DbContext.Set<ClassBookStudentActivity>().Where(a => a.SchoolYear == schoolYear && a.ClassBookId == classBookId)
            on sc.PersonId equals a.PersonId
            into j2 from a in j2.DefaultIfEmpty()

            join fgrsn in this.DbContext.Set<ClassBookStudentFirstGradeResultSpecialNeeds>().Where(a => a.SchoolYear == schoolYear && a.ClassBookId == classBookId)
            on sc.PersonId equals fgrsn.PersonId
            into j3 from fgrsn in j3.DefaultIfEmpty()

            where sc.PersonId == personId

            select new
            {
                sc.ClassNumber,
                FullName = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                Speciality = s.Name,
                Activities = a.Activities,
                HasSpecialNeedFirstGradeResult = fgrsn != null,
            }
        ).SingleAsync(ct);

        var specialNeeds = await (
            from cbssn in this.DbContext.Set<ClassBookStudentSpecialNeeds>()
            where cbssn.SchoolYear == schoolYear &&
                cbssn.ClassBookId == classBookId &&
                cbssn.PersonId == personId
            select cbssn.CurriculumId
        ).ToArrayAsync(ct);

        var studentCurriculums = await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

            join cs in this.DbContext.Set<CurriculumStudent>() on c.CurriculumId equals cs.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where cs.PersonId == personId &&
                cc.IsValid &&
                c.IsValid

            orderby c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

            select new GetStudentVOCurriculums(
                c.CurriculumId,
                s.SubjectName + " " + st.Name,
                (c.CurriculumGroupNum ?? 0) == 0 ? null : c.CurriculumGroupNum.ToString())
        ).ToArrayAsync(ct);

        studentCurriculums = studentCurriculums.Distinct().ToArray();

        var gradelessCurriculums = await (
            from cbsg in this.DbContext.Set<ClassBookStudentGradeless>()
            where cbsg.SchoolYear == schoolYear &&
                cbsg.ClassBookId == classBookId &&
                cbsg.PersonId == personId
            select new GetStudentVOGradelessCurriculum(
                cbsg.CurriculumId,
                cbsg.WithoutFirstTermGrade,
                cbsg.WithoutSecondTermGrade,
                cbsg.WithoutFinalGrade)
        ).ToArrayAsync(ct);

        var carriedAbsences = await (
            from ca in this.DbContext.Set<ClassBookStudentCarriedAbsence>()

            where ca.SchoolYear == schoolYear &&
                ca.ClassBookId == classBookId &&
                ca.PersonId == personId

            select new GetStudentVOCarriedAbsences(
                ca.FirstTermExcusedCount,
                ca.FirstTermUnexcusedCount,
                ca.FirstTermLateCount,
                ca.SecondTermExcusedCount,
                ca.SecondTermUnexcusedCount,
                ca.SecondTermLateCount)
        ).SingleOrDefaultAsync(ct);

        return new GetStudentVO(
            personId,
            student.ClassNumber,
            student.FullName,
            specialNeeds
                .Where(curriculumId =>
                    studentCurriculums.Any(studentCurriculum => studentCurriculum.CurriculumId == curriculumId))
                .ToArray(),
            student.HasSpecialNeedFirstGradeResult,
            gradelessCurriculums,
            studentCurriculums,
            student.Activities,
            student.Speciality,
            carriedAbsences
            );
    }

    public async Task<GetStudentNumbersVO[]> GetStudentNumbersAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        return await (
            from s in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join p in this.DbContext.Set<Person>() on s.PersonId equals p.PersonId

            orderby (s.ClassNumber == null ? 99999 : s.ClassNumber), p.FirstName, p.MiddleName, p.LastName

            select new GetStudentNumbersVO(
                s.PersonId,
                s.ClassNumber,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName)
            )
        ).ToArrayAsync(ct);
    }

    public async Task<GetSchoolYearProgramVO> GetSchoolYearProgramAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()

            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId

            select new GetSchoolYearProgramVO(
                cb.ClassBookId,
                cb.SchoolYearProgram)
        ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetPrintsVO>> GetClassBookPrintsAsync(int schoolYear, int classBookId, int? offset, int? limit, CancellationToken ct)
    {
        return await (
            from p in this.DbContext.Set<ClassBookPrint>()

            join csu in this.DbContext.Set<SysUser>()
            on p.CreatedBySysUserId equals csu.SysUserId

            join cp in this.DbContext.Set<Person>()
            on csu.PersonId equals cp.PersonId

            where p.SchoolYear == schoolYear
                && p.ClassBookId == classBookId

            orderby p.CreateDate descending

            select new GetPrintsVO(
                p.CreateDate,
                cp.FirstName,
                cp.LastName,
                p.Status,
                this.blobServiceHmacKey,
                this.blobServicePublicWebUrl,
                p.BlobId)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<TableResultVO<GetStudentPrintsVO>> GetClassBookStudentPrintsAsync(int schoolYear, int classBookId, int? offset, int? limit, CancellationToken ct)
    {
        return await (
            from p in this.DbContext.Set<ClassBookStudentPrint>()

            join sp in this.DbContext.Set<Person>()
            on p.PersonId equals sp.PersonId

            join csu in this.DbContext.Set<SysUser>()
            on p.CreatedBySysUserId equals csu.SysUserId

            join cp in this.DbContext.Set<Person>()
            on csu.PersonId equals cp.PersonId

            where p.SchoolYear == schoolYear
                && p.ClassBookId == classBookId

            orderby p.CreateDate descending

            select new GetStudentPrintsVO(
                p.CreateDate,
                cp.FirstName,
                cp.LastName,
                StringUtils.JoinNames(sp.FirstName, sp.MiddleName, sp.LastName),
                p.Status,
                this.blobServiceHmacKey,
                this.blobServicePublicWebUrl,
                p.BlobId)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<bool> HasPendingPrintAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<ClassBookPrint>()
            .AnyAsync(
                cp =>
                    cp.SchoolYear == schoolYear
                    && cp.ClassBookId == classBookId
                    && cp.Status == ClassBookPrintStatus.Pending,
                ct);
    }

    public async Task<bool> HasPendingStudentPrintAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<ClassBookStudentPrint>()
            .AnyAsync(
                cp =>
                    cp.SchoolYear == schoolYear
                    && cp.ClassBookId == classBookId
                    && cp.PersonId == personId
                    && cp.Status == ClassBookPrintStatus.Pending,
                ct);
    }

    public async Task RemoveRelatedPersonnelSchoolBookAccessAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var relatedData = await this.DbContext.Set<PersonnelSchoolBookAccess>().Where(pd => pd.SchoolYear == schoolYear && pd.ClassBookId == classBookId).ToArrayAsync(ct);
        this.DbContext.RemoveRange(relatedData);
    }
}
