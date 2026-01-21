namespace SB.Data;

using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.IExamDutyProtocolsQueryRepository;
using SB.Common;
using System.Threading;
using System;

internal class ExamDutyProtocolsQueryRepository : Repository, IExamDutyProtocolsQueryRepository
{
    public ExamDutyProtocolsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int instId,
        int? schoolYear,
        ExamDutyProtocolType? protocolType,
        string? orderNumber,
        DateTime? orderDate,
        DateTime? protocolDate,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<ExamDutyProtocol>();
        predicate = predicate.AndEquals(p => p.SchoolYear, schoolYear);
        predicate = predicate.AndStringContains(p => p.OrderNumber, orderNumber);
        predicate = predicate.AndEquals(p => p.Date, protocolDate);
        predicate = predicate.AndEquals(p => p.OrderDate, orderDate);

        var sePredicate = PredicateBuilder.True<StateExamDutyProtocol>();
        sePredicate = sePredicate.AndEquals(p => p.SchoolYear, schoolYear);
        sePredicate = sePredicate.AndStringContains(p => p.OrderNumber, orderNumber);
        sePredicate = sePredicate.AndEquals(p => p.Date, protocolDate);
        sePredicate = sePredicate.AndEquals(p => p.OrderDate, orderDate);

        var scPredicate = PredicateBuilder.True<SkillsCheckExamDutyProtocol>();
        scPredicate = scPredicate.AndEquals(p => p.SchoolYear, schoolYear);
        scPredicate = scPredicate.AndEquals(p => p.Date, protocolDate);

        var nvoPredicate = PredicateBuilder.True<NvoExamDutyProtocol>();
        nvoPredicate = nvoPredicate.AndEquals(p => p.SchoolYear, schoolYear);
        nvoPredicate = nvoPredicate.AndEquals(p => p.Date, protocolDate);

        var examDutyProtocols =
            from edp in this.DbContext.Set<ExamDutyProtocol>().Where(predicate)

            where edp.InstId == instId

            select new
            {
                edp.ExamDutyProtocolId,
                edp.SchoolYear,
                OrderNumber = (string?)edp.OrderNumber,
                OrderDate = (DateTime?)edp.OrderDate,
                ProtocolDate = edp.Date,
                SessionType = (string?)edp.SessionType,
                ProtocolType = ExamDutyProtocolType.Exam
            };

        var stateExamDutyProtocols =
            from edp in this.DbContext.Set<StateExamDutyProtocol>().Where(sePredicate)

            where edp.InstId == instId

            select new
            {
                ExamDutyProtocolId = edp.StateExamDutyProtocolId,
                edp.SchoolYear,
                OrderNumber = (string?)edp.OrderNumber,
                OrderDate = (DateTime?)edp.OrderDate,
                ProtocolDate = edp.Date,
                SessionType = (string?)edp.SessionType,
                ProtocolType = ExamDutyProtocolType.State
            };

        var skillsCheckExamDutyProtocols =
            from edp in this.DbContext.Set<SkillsCheckExamDutyProtocol>().Where(scPredicate)

            where edp.InstId == instId

            select new
            {
                ExamDutyProtocolId = edp.SkillsCheckExamDutyProtocolId,
                edp.SchoolYear,
                OrderNumber = (string?)null,
                OrderDate = (DateTime?)null,
                ProtocolDate = edp.Date,
                SessionType = (string?)null,
                ProtocolType = ExamDutyProtocolType.SkillsCheck
            };

        var nvoExamDutyProtocols =
            from edp in this.DbContext.Set<NvoExamDutyProtocol>().Where(nvoPredicate)

            where edp.InstId == instId

            select new
            {
                ExamDutyProtocolId = edp.NvoExamDutyProtocolId,
                edp.SchoolYear,
                OrderNumber = (string?)null,
                OrderDate = (DateTime?)null,
                ProtocolDate = edp.Date,
                SessionType = (string?)null,
                ProtocolType = ExamDutyProtocolType.Nvo
            };

        var typePredicate = PredicateBuilder.True(examDutyProtocols);
        typePredicate = typePredicate.AndEquals(p => p.ProtocolType, protocolType);

        return await
            examDutyProtocols
            .Concat(stateExamDutyProtocols)
            .Concat(skillsCheckExamDutyProtocols)
            .Concat(nvoExamDutyProtocols)
            .Where(typePredicate)
            .OrderByDescending(edp => edp.ExamDutyProtocolId)
            .Select(edp => new GetAllVO(
                edp.ExamDutyProtocolId,
                edp.SchoolYear,
                edp.OrderNumber,
                edp.OrderDate,
                edp.ProtocolDate,
                edp.SessionType,
                edp.ProtocolType,
                EnumUtils.GetEnumDescription(edp.ProtocolType)))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int examDutyProtocolId,
        CancellationToken ct)
    {
        var classIds = await (
            from edpc in this.DbContext.Set<ExamDutyProtocolClass>()
            where edpc.SchoolYear == schoolYear && edpc.ExamDutyProtocolId == examDutyProtocolId
            select edpc.ClassId
         ).ToArrayAsync(ct);

        var supervisorIds = await (
            from edps in this.DbContext.Set<ExamDutyProtocolSupervisor>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            where edps.SchoolYear == schoolYear && edps.ExamDutyProtocolId == examDutyProtocolId
            orderby p.FirstName, p.MiddleName, p.LastName
            select edps.PersonId
         ).ToArrayAsync(ct);

        return await (
            from edp in this.DbContext.Set<ExamDutyProtocol>()

            where edp.SchoolYear == schoolYear && edp.ExamDutyProtocolId == examDutyProtocolId

            select new GetVO(
                edp.SchoolYear,
                edp.ExamDutyProtocolId,
                edp.ProtocolNumber,
                edp.ProtocolDate,
                edp.SessionType,
                edp.SubjectId,
                edp.SubjectTypeId,
                edp.EduFormId,
                edp.ProtocolExamTypeId,
                edp.ProtocolExamSubTypeId,
                edp.OrderNumber,
                edp.OrderDate,
                edp.Date,
                edp.GroupNum,
                classIds,
                supervisorIds)
         ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int examDutyProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<ExamDutyProtocolStudent>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            join cg in this.DbContext.Set<ClassGroup>()
                on new { edps.SchoolYear, edps.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            join cgp in this.DbContext.Set<ClassGroup>()
                on new { cg.SchoolYear, cg.ParentClassId }
                equals new { cgp.SchoolYear, ParentClassId = (int?)cgp.ClassId }
                into j1
            from cgp in j1.DefaultIfEmpty()

            where edps.SchoolYear == schoolYear &&
                edps.ExamDutyProtocolId == examDutyProtocolId
            orderby cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            select new GetStudentAllVO(
                cg.ClassId,
                p.PersonId,
                cgp.ClassName ?? cg.ClassName,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName))
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<bool> HasDuplicatedStudentsAsync(
        int schoolYear,
        int examDutyProtocolId,
        int[] personIds,
        CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<ExamDutyProtocolStudent>()

            where edps.SchoolYear == schoolYear &&
                edps.ExamDutyProtocolId == examDutyProtocolId &&
                this.DbContext.MakeIdsQuery(personIds).Any(id => edps.PersonId == id.Id)

            select edps.PersonId
        ).AnyAsync(ct);
    }

    public async Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int examDutyProtocolId,
       CancellationToken ct)
    {
        var classNames = await (
            from edpc in this.DbContext.Set<ExamDutyProtocolClass>()
            join cg in this.DbContext.Set<ClassGroup>()
                on new { edpc.SchoolYear, edpc.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            where edpc.SchoolYear == schoolYear && edpc.ExamDutyProtocolId == examDutyProtocolId
            select cg.ClassName
        ).ToArrayAsync(ct);

        var supervisors = await (
            from edps in this.DbContext.Set<ExamDutyProtocolSupervisor>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            where edps.SchoolYear == schoolYear && edps.ExamDutyProtocolId == examDutyProtocolId
            orderby p.FirstName, p.MiddleName, p.LastName
            select new GetWordDataVOSupervisor(StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName))
        ).ToArrayAsync(ct);

        var students = await (
            from edps in this.DbContext.Set<ExamDutyProtocolStudent>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            join cg in this.DbContext.Set<ClassGroup>()
                on new { edps.SchoolYear, edps.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            join cgp in this.DbContext.Set<ClassGroup>()
                on new { cg.SchoolYear, cg.ParentClassId }
                equals new { cgp.SchoolYear, ParentClassId = (int?)cgp.ClassId }
            into j1
            from cgp in j1.DefaultIfEmpty()

            where edps.SchoolYear == schoolYear &&
                edps.ExamDutyProtocolId == examDutyProtocolId
            orderby cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            select new GetWordDataVOStudent(StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName), cgp.ClassName ?? cg.ClassName)
        ).ToArrayAsync(ct);

        return await (
            from edp in this.DbContext.Set<ExamDutyProtocol>()
            join et in this.DbContext.Set<ProtocolExamType>() on edp.ProtocolExamTypeId equals et.Id
            join est in this.DbContext.Set<ProtocolExamSubType>() on edp.ProtocolExamSubTypeId equals est.Id
            join s in this.DbContext.Set<Subject>() on edp.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on edp.SubjectTypeId equals st.SubjectTypeId
            join i in this.DbContext.Set<InstitutionSchoolYear>() on new { edp.InstId, edp.SchoolYear } equals new { InstId = i.InstitutionId, i.SchoolYear }

            join ef in this.DbContext.Set<EduForm>() on edp.EduFormId equals ef.ClassEduFormId
            into g0
            from ef in g0.DefaultIfEmpty()

            join t in this.DbContext.Set<Town>() on i.TownId equals t.TownId
            into g2
            from t in g2.DefaultIfEmpty()

            join m in this.DbContext.Set<Municipality>() on t.MunicipalityId equals m.MunicipalityId
            into g3
            from m in g3.DefaultIfEmpty()

            join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId
            into g4
            from r in g4.DefaultIfEmpty()

            where edp.SchoolYear == schoolYear && edp.ExamDutyProtocolId == examDutyProtocolId

            select new GetWordDataVO(
                edp.SchoolYear + " / " + (edp.SchoolYear + 1),
                i.Name,
                !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                edp.ProtocolNumber,
                edp.ProtocolDate != null ? edp.ProtocolDate!.Value.ToString("dd.MM.yyyy") : null,
                edp.SessionType,
                s.SubjectName + ' ' + st.Name,
                ef.Name,
                et.Name,
                est.Name,
                edp.OrderNumber,
                edp.OrderDate.ToString("dd.MM.yyyy"),
                edp.Date.ToString("dd.MM.yyyy"),
                edp.GroupNum,
                classNames,
                supervisors,
                students)
         ).SingleAsync(ct);
    }

    public async Task<int[]> GetStudentPersonIdsByClassIdAsync(int schoolYear, int classId, int[] excludedPersonIds, CancellationToken ct)
    {
        return await (
            from cg in this.DbContext.Set<ClassGroup>()
            join sc in this.DbContext.Set<StudentClass>()
                on new { cg.SchoolYear, cg.ClassId }
                equals new { sc.SchoolYear, sc.ClassId }
            let currentClassId = cg.IsNotPresentForm == true ? cg.ClassId : cg.ParentClassId
            where currentClassId == classId
                && !this.DbContext.MakeIdsQuery(excludedPersonIds).Any(id => sc.PersonId == id.Id)
                && sc.Status == StudentClassStatus.Enrolled
            select sc.PersonId
        ).ToArrayAsync(ct);
    }
}
