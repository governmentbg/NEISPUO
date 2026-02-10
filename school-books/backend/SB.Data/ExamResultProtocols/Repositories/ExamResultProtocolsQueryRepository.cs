namespace SB.Data;

using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.IExamResultProtocolsQueryRepository;
using SB.Common;
using System.Threading;
using System;

internal class ExamResultProtocolsQueryRepository : Repository, IExamResultProtocolsQueryRepository
{
    public ExamResultProtocolsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int instId,
        int? schoolYear,
        string? orderNumber,
        ExamResultProtocolType? protocolType,
        string? protocolNumber,
        DateTime? protocolDate,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<ExamResultProtocol>();
        predicate = predicate.AndEquals(p => p.SchoolYear, schoolYear);
        predicate = predicate.AndStringContains(p => p.CommissionNominationOrderNumber, orderNumber);
        predicate = predicate.AndStringContains(p => p.ProtocolNumber, protocolNumber);
        predicate = predicate.AndEquals(p => p.ProtocolDate, protocolDate);

        var qPredicate = PredicateBuilder.True<QualificationExamResultProtocol>();
        qPredicate = qPredicate.AndEquals(p => p.SchoolYear, schoolYear);
        qPredicate = qPredicate.AndStringContains(p => p.CommissionNominationOrderNumber, orderNumber);
        qPredicate = qPredicate.AndStringContains(p => p.ProtocolNumber, protocolNumber);
        qPredicate = qPredicate.AndEquals(p => p.ProtocolDate, protocolDate);

        var skPredicate = PredicateBuilder.True<SkillsCheckExamResultProtocol>();
        skPredicate = skPredicate.AndEquals(p => p.SchoolYear, schoolYear);
        skPredicate = skPredicate.AndEquals(p => p.Date, protocolDate);

        var qaPredicate = PredicateBuilder.True<QualificationAcquisitionProtocol>();
        qaPredicate = qaPredicate.AndEquals(p => p.SchoolYear, schoolYear);
        qaPredicate = qaPredicate.AndStringContains(p => p.CommissionNominationOrderNumber, orderNumber);
        qaPredicate = qaPredicate.AndEquals(p => p.Date, protocolDate);

        var hsPredicate = PredicateBuilder.True<HighSchoolCertificateProtocol>();
        hsPredicate = hsPredicate.AndEquals(p => p.SchoolYear, schoolYear);
        hsPredicate = hsPredicate.AndStringContains(p => p.CommissionNominationOrderNumber, orderNumber);
        hsPredicate = hsPredicate.AndEquals(p => p.ProtocolDate, protocolDate);

        var gtPredicate = PredicateBuilder.True<GraduationThesisDefenseProtocol>();
        gtPredicate = gtPredicate.AndEquals(p => p.SchoolYear, schoolYear);
        gtPredicate = gtPredicate.AndStringContains(p => p.DirectorOrderNumber, orderNumber);
        gtPredicate = gtPredicate.AndStringContains(p => p.ProtocolNumber, protocolNumber);
        gtPredicate = gtPredicate.AndEquals(p => p.ProtocolDate, protocolDate);

        var examResultProtocols =
            from edp in this.DbContext.Set<ExamResultProtocol>().Where(predicate)

            where edp.InstId == instId

            select new
            {
                edp.ExamResultProtocolId,
                edp.SchoolYear,
                OrderNumber = (string?)edp.CommissionNominationOrderNumber,
                edp.ProtocolNumber,
                edp.ProtocolDate,
                edp.SessionType,
                ProtocolType = ExamResultProtocolType.Exam
            };

        var qualificationExamResultProtocols =
            from edp in this.DbContext.Set<QualificationExamResultProtocol>().Where(qPredicate)

            where edp.InstId == instId

            select new
            {
                ExamResultProtocolId = edp.QualificationExamResultProtocolId,
                edp.SchoolYear,
                OrderNumber = (string?)edp.CommissionNominationOrderNumber,
                edp.ProtocolNumber,
                edp.ProtocolDate,
                edp.SessionType,
                ProtocolType = ExamResultProtocolType.Qualification
            };

        var skillsCheckExamResultProtocols =
            from edp in this.DbContext.Set<SkillsCheckExamResultProtocol>().Where(skPredicate)

            where edp.InstId == instId

            select new
            {
                ExamResultProtocolId = edp.SkillsCheckExamResultProtocolId,
                edp.SchoolYear,
                OrderNumber = (string?)null,
                ProtocolNumber = (string?)null,
                ProtocolDate = edp.Date,
                SessionType = (string?)null,
                ProtocolType = ExamResultProtocolType.SkillsCheck
            };

        var qualificationAcquisitionProtocols =
            from edp in this.DbContext.Set<QualificationAcquisitionProtocol>().Where(qaPredicate)

            where edp.InstId == instId

            select new
            {
                ExamResultProtocolId = edp.QualificationAcquisitionProtocolId,
                edp.SchoolYear,
                OrderNumber = (string?)edp.CommissionNominationOrderNumber,
                edp.ProtocolNumber,
                edp.ProtocolDate,
                SessionType = (string?)null,
                ProtocolType =
                    edp.ProtocolType == QualificationAcquisitionProtocolType.QualificationAcquisition ? ExamResultProtocolType.QualificationAcquisition :
                    edp.ProtocolType == QualificationAcquisitionProtocolType.QualificationAcquisitionExamGrades ? ExamResultProtocolType.QualificationAcquisitionExamGrades :
                    ExamResultProtocolType.QualificationAcquisitionStateExamGrades
            };

        var highSchoolCertificateProtocols =
            from edp in this.DbContext.Set<HighSchoolCertificateProtocol>().Where(hsPredicate)

            where edp.InstId == instId

            select new
            {
                ExamResultProtocolId = edp.HighSchoolCertificateProtocolId,
                edp.SchoolYear,
                OrderNumber = (string?)edp.CommissionNominationOrderNumber,
                ProtocolNumber = edp.ProtocolNum,
                edp.ProtocolDate,
                SessionType = edp.ExamSession,
                ProtocolType = ExamResultProtocolType.HighSchoolCertificate
            };

        var graduationThesisDefenseProtocols =
            from edp in this.DbContext.Set<GraduationThesisDefenseProtocol>().Where(gtPredicate)

            where edp.InstId == instId

            select new
            {
                ExamResultProtocolId = edp.GraduationThesisDefenseProtocolId,
                edp.SchoolYear,
                OrderNumber = (string?)edp.DirectorOrderNumber,
                edp.ProtocolNumber,
                edp.ProtocolDate,
                edp.SessionType,
                ProtocolType = ExamResultProtocolType.GraduationThesisDefense
            };

        var typePredicate = PredicateBuilder.True(examResultProtocols);
        typePredicate = typePredicate.AndEquals(p => p.ProtocolType, protocolType);

        return await
            examResultProtocols
            .Concat(qualificationExamResultProtocols)
            .Concat(skillsCheckExamResultProtocols)
            .Concat(qualificationAcquisitionProtocols)
            .Concat(highSchoolCertificateProtocols)
            .Concat(graduationThesisDefenseProtocols)
            .Where(typePredicate)
            .OrderByDescending(edp => edp.ExamResultProtocolId)
            .Select(edp => new GetAllVO(
                edp.ExamResultProtocolId,
                edp.OrderNumber,
                edp.ProtocolType,
                EnumUtils.GetEnumDescription(edp.ProtocolType),
                edp.ProtocolNumber,
                edp.ProtocolDate,
                edp.SessionType))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int examResultProtocolId,
        CancellationToken ct)
    {
        var classIds = await (
            from edpc in this.DbContext.Set<ExamResultProtocolClass>()
            where edpc.SchoolYear == schoolYear && edpc.ExamResultProtocolId == examResultProtocolId
            select edpc.ClassId
         ).ToArrayAsync(ct);

        return (await (
            from ap in this.DbContext.Set<ExamResultProtocol>()
            join apc in this.DbContext.Set<ExamResultProtocolCommissioner>()
                on new { ap.SchoolYear, ap.ExamResultProtocolId }
                equals new { apc.SchoolYear, apc.ExamResultProtocolId }

            where ap.SchoolYear == schoolYear &&
                ap.ExamResultProtocolId == examResultProtocolId

            select new
            {
                ap.SchoolYear,
                ap.ExamResultProtocolId,
                ap.ProtocolNumber,
                ap.ProtocolDate,
                ap.SessionType,
                ap.SubjectId,
                ap.SubjectTypeId,
                ap.GroupNum,
                ap.EduFormId,
                ap.ProtocolExamTypeId,
                ap.ProtocolExamSubTypeId,
                ap.Date,
                ap.CommissionNominationOrderNumber,
                ap.CommissionNominationOrderDate,
                apc.PersonId,
                apc.IsChairman,
                apc.OrderNum
            })
            .ToArrayAsync(ct))
            .GroupBy(ap => new
            {
                ap.SchoolYear,
                ap.ExamResultProtocolId,
                ap.ProtocolNumber,
                ap.ProtocolDate,
                ap.SessionType,
                ap.SubjectId,
                ap.SubjectTypeId,
                ap.GroupNum,
                ap.EduFormId,
                ap.ProtocolExamTypeId,
                ap.ProtocolExamSubTypeId,
                ap.Date,
                ap.CommissionNominationOrderNumber,
                ap.CommissionNominationOrderDate
            })
            .Select(g => new GetVO(
                g.Key.SchoolYear,
                g.Key.ExamResultProtocolId,
                g.Key.ProtocolNumber,
                g.Key.ProtocolDate,
                g.Key.SessionType,
                g.Key.SubjectId,
                g.Key.SubjectTypeId,
                g.Key.GroupNum,
                classIds,
                g.Key.EduFormId,
                g.Key.ProtocolExamTypeId,
                g.Key.ProtocolExamSubTypeId,
                g.Key.Date,
                g.Key.CommissionNominationOrderNumber,
                g.Key.CommissionNominationOrderDate,
                g.Where(c => c.IsChairman == true).Select(c => c.PersonId).Single(),
                g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => c.PersonId).ToArray()
            ))
            .Single();
    }

    public async Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int examResultProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<ExamResultProtocolStudent>()
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
                edps.ExamResultProtocolId == examResultProtocolId
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
        int examResultProtocolId,
        int[] personIds,
    CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<ExamResultProtocolStudent>()

            where edps.SchoolYear == schoolYear &&
                edps.ExamResultProtocolId == examResultProtocolId &&
                this.DbContext.MakeIdsQuery(personIds).Any(id => edps.PersonId == id.Id)

            select edps.PersonId
        ).AnyAsync(ct);
    }

    public async Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int examResultProtocolId,
       CancellationToken ct)
    {
        var classNames = await (
            from edpc in this.DbContext.Set<ExamResultProtocolClass>()
            join cg in this.DbContext.Set<ClassGroup>()
                on new { edpc.SchoolYear, edpc.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            where edpc.SchoolYear == schoolYear && edpc.ExamResultProtocolId == examResultProtocolId
            select cg.ClassName
        ).ToArrayAsync(ct);

        var students = await (
            from edps in this.DbContext.Set<ExamResultProtocolStudent>()
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
                edps.ExamResultProtocolId == examResultProtocolId
            orderby cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            select new GetWordDataVOStudent(StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName), cgp.ClassName ?? cg.ClassName)
        ).ToArrayAsync(ct);

        return (await (
            from erp in this.DbContext.Set<ExamResultProtocol>()
            join et in this.DbContext.Set<ProtocolExamType>() on erp.ProtocolExamTypeId equals et.Id
            join est in this.DbContext.Set<ProtocolExamSubType>() on erp.ProtocolExamSubTypeId equals est.Id
            join s in this.DbContext.Set<Subject>() on erp.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on erp.SubjectTypeId equals st.SubjectTypeId
            join erpc in this.DbContext.Set<ExamResultProtocolCommissioner>()
                on new { erp.SchoolYear, erp.ExamResultProtocolId }
                equals new { erpc.SchoolYear, erpc.ExamResultProtocolId }
            join i in this.DbContext.Set<InstitutionSchoolYear>() on new { erp.InstId, erp.SchoolYear } equals new { InstId = i.InstitutionId, i.SchoolYear }

            join ef in this.DbContext.Set<EduForm>() on erp.EduFormId equals ef.ClassEduFormId
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
            join pc in this.DbContext.Set<Person>() on erpc.PersonId equals pc.PersonId

            where erp.SchoolYear == schoolYear &&
                erp.ExamResultProtocolId == examResultProtocolId

            select new
            {
                SchoolYear = erp.SchoolYear + " / " + (erp.SchoolYear + 1),
                InstitutionName = i.Name,
                InstitutionTownName = !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                InstitutionMunicipalityName = !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                InstitutionRegionName = !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                erp.ProtocolNumber,
                erp.ProtocolDate,
                erp.SessionType,
                SubjectName = s.SubjectName + ' ' + st.Name,
                erp.GroupNum,
                EduFormName = ef.Name,
                ProtocolExamType = et.Name,
                ProtocolExamSubType = est.Name,
                erp.Date,
                erp.CommissionNominationOrderNumber,
                erp.CommissionNominationOrderDate,
                CommissionerName = StringUtils.JoinNames(pc.FirstName, pc.MiddleName, pc.LastName),
                erpc.IsChairman,
                erpc.OrderNum
            })
            .ToArrayAsync(ct))
            .GroupBy(erp => new
            {
                erp.SchoolYear,
                erp.InstitutionName,
                erp.InstitutionTownName,
                erp.InstitutionMunicipalityName,
                erp.InstitutionRegionName,
                erp.ProtocolNumber,
                erp.ProtocolDate,
                erp.SessionType,
                erp.SubjectName,
                erp.GroupNum,
                erp.EduFormName,
                erp.ProtocolExamType,
                erp.ProtocolExamSubType,
                erp.Date,
                erp.CommissionNominationOrderNumber,
                erp.CommissionNominationOrderDate
            })
            .Select(g => new GetWordDataVO(
                g.Key.SchoolYear,
                g.Key.InstitutionName,
                g.Key.InstitutionTownName,
                g.Key.InstitutionMunicipalityName,
                g.Key.InstitutionRegionName,
                g.Key.ProtocolNumber,
                g.Key.ProtocolDate?.ToString("dd.MM.yyyy"),
                g.Key.SessionType,
                g.Key.SubjectName,
                g.Key.GroupNum,
                classNames,
                g.Key.EduFormName,
                g.Key.ProtocolExamType,
                g.Key.ProtocolExamSubType,
                g.Key.Date.ToString("dd.MM.yyyy"),
                g.Key.CommissionNominationOrderNumber,
                g.Key.CommissionNominationOrderDate.ToString("dd.MM.yyyy"),
                g.Where(c => c.IsChairman == true).Select(c => c.CommissionerName).Single(),
                g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => new GetWordDataVOCommissioner(c.CommissionerName)).ToArray(),
                this.FormatCommissionMembersDivided(g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => c.CommissionerName).ToArray()),
                students
            ))
            .Single();
    }

    private GetWordDataVOCommissionerLeftRight[] FormatCommissionMembersDivided(string[] commissionerNames)
    {
        var leftColumnLength = (int)Math.Ceiling((decimal)commissionerNames.Length / 2);

        // We don't use OrderNum to prevent printing just "." at last right position
        return Enumerable.Range(0, leftColumnLength)
            .Select(i => new GetWordDataVOCommissionerLeftRight(
                $"{i + 1}. {commissionerNames[i]}",
                leftColumnLength + i < commissionerNames.Length ?
                    $"{leftColumnLength + i + 1}. {commissionerNames[leftColumnLength + i]}" :
                    string.Empty))
            .ToArray();
    }
}
