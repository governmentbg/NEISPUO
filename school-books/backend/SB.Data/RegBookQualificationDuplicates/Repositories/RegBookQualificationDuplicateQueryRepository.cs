namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IRegBookQualificationDuplicateQueryRepository;

internal class RegBookQualificationDuplicateQueryRepository : Repository, IRegBookQualificationDuplicateQueryRepository
{
    public RegBookQualificationDuplicateQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        string? registrationNumberTotal,
        string? registrationNumberYear,
        DateTime? registrationDate,
        string? fullName,
        string? identifier,
        int? basicDocumentId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<RegBookQualificationDuplicate>();
        predicate = predicate.AndEquals(mbr => mbr.RegistrationNumberTotal, registrationNumberTotal);
        predicate = predicate.AndEquals(mbr => mbr.RegistrationNumberYear, registrationNumberYear);
        predicate = predicate.AndEquals(mbr => mbr.RegistrationDate, registrationDate);
        predicate = predicate.AndEquals(mbr => mbr.BasicDocumentId, basicDocumentId);

        var pPredicate = PredicateBuilder.True<Person>();
        pPredicate = pPredicate.AndStringContains(mbr => mbr.PersonalId, identifier);

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            string[] words = fullName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                pPredicate = pPredicate.AndAnyStringContains(mbr => mbr.FirstName, mbr => mbr.MiddleName, mbr => mbr.LastName, word);
            }
        }

        return await (
            from rbq in this.DbContext.Set<RegBookQualificationDuplicate>().Where(predicate)
            join p in this.DbContext.Set<Person>().Where(pPredicate) on rbq.PersonId equals p.PersonId
            join bd in this.DbContext.Set<BasicDocument>() on rbq.BasicDocumentId equals bd.Id

            where rbq.SchoolYear == schoolYear && rbq.InstitutionId == instId

            orderby rbq.RegistrationNumberTotal.Length, rbq.RegistrationNumberTotal

            select new GetAllVO(
                rbq.Id,
                rbq.RegistrationNumberTotal,
                rbq.RegistrationNumberYear,
                rbq.RegistrationDate,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                p.PersonalId,
                bd.Name,
                rbq.IsCancelled
                ))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int id,
        CancellationToken ct)
    {
        return await
            (from rbq in this.DbContext.Set<RegBookQualificationDuplicate>()
             join p in this.DbContext.Set<Person>() on rbq.PersonId equals p.PersonId
             join bd in this.DbContext.Set<BasicDocument>() on rbq.BasicDocumentId equals bd.Id

             join ctype in this.DbContext.Set<ClassType>() on rbq.ClassTypeId equals ctype.ClassTypeId
             into j0
             from ctype in j0.DefaultIfEmpty()

             join prof in this.DbContext.Set<SPPOOProfession>() on rbq.SPPOOProfessionId equals prof.SPPOOProfessionId
             into j1
             from prof in j1.DefaultIfEmpty()

             join spec in this.DbContext.Set<SPPOOSpeciality>() on rbq.SPPOOSpecialityId equals spec.SPPOOSpecialityId
             into j2
             from spec in j2.DefaultIfEmpty()

             join ef in this.DbContext.Set<EduForm>() on rbq.EduFormId equals ef.ClassEduFormId
             into j3
             from ef in j3.DefaultIfEmpty()

             where rbq.SchoolYear == schoolYear && rbq.Id == id
             select new GetVO(
                 rbq.RegistrationNumberTotal,
                 rbq.RegistrationNumberYear,
                 rbq.RegistrationDate,
                 p.FirstName,
                 p.MiddleName,
                 p.LastName,
                 p.PersonalId,
                 bd.Name,
                 rbq.Series,
                 rbq.FactoryNumber,
                 ctype.Name,
                 prof.Name,
                 spec.Name,
                 ef.Name,
                 rbq.OrigSeries,
                 rbq.OrigFactoryNumber,
                 rbq.OrigRegistrationNumber,
                 rbq.OrigRegistrationNumberYear,
                 rbq.OrigRegistrationDate,
                 rbq.IsCancelled
             )).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO[]> GetExcelDataAsync(int schoolYear, int instId, int basicDocumentId, CancellationToken ct)
    {
        return await
            (from rbq in this.DbContext.Set<RegBookQualificationDuplicate>()
             join p in this.DbContext.Set<Person>() on rbq.PersonId equals p.PersonId
             join bd in this.DbContext.Set<BasicDocument>() on rbq.BasicDocumentId equals bd.Id

             join ctype in this.DbContext.Set<ClassType>() on rbq.ClassTypeId equals ctype.ClassTypeId
             into j0
             from ctype in j0.DefaultIfEmpty()

             join prof in this.DbContext.Set<SPPOOProfession>() on rbq.SPPOOProfessionId equals prof.SPPOOProfessionId
             into j1
             from prof in j1.DefaultIfEmpty()

             join spec in this.DbContext.Set<SPPOOSpeciality>() on rbq.SPPOOSpecialityId equals spec.SPPOOSpecialityId
             into j2
             from spec in j2.DefaultIfEmpty()

             join ef in this.DbContext.Set<EduForm>() on rbq.EduFormId equals ef.ClassEduFormId
             into j3
             from ef in j3.DefaultIfEmpty()

             where rbq.SchoolYear == schoolYear && rbq.InstitutionId == instId && rbq.BasicDocumentId == basicDocumentId

             orderby rbq.RegistrationNumberTotal.Length, rbq.RegistrationNumberTotal

             select new GetExcelDataVO(
                 rbq.Id,
                 rbq.RegistrationNumberTotal,
                 rbq.RegistrationNumberYear,
                 rbq.RegistrationDate,
                 p.FirstName,
                 p.MiddleName,
                 p.LastName,
                 p.PersonalId,
                 bd.Name,
                 rbq.Series,
                 rbq.FactoryNumber,
                 ctype.Name,
                 prof.Name,
                 spec.Name,
                 ef.Name,
                 rbq.OrigSeries,
                 rbq.OrigFactoryNumber,
                 rbq.OrigRegistrationNumber,
                 rbq.OrigRegistrationNumberYear,
                 rbq.OrigRegistrationDate,
                 rbq.IsCancelled
             )).ToArrayAsync(ct);
    }

    public async Task<GetWordDataVO> GetWordDataAsync(int schoolYear, int id, CancellationToken ct)
    {
        return await
            (from rbq in this.DbContext.Set<RegBookQualificationDuplicate>()
             join p in this.DbContext.Set<Person>() on rbq.PersonId equals p.PersonId
             join bd in this.DbContext.Set<BasicDocument>() on rbq.BasicDocumentId equals bd.Id
             join i in this.DbContext.Set<InstitutionSchoolYear>() on new { rbq.InstitutionId, rbq.SchoolYear } equals new { i.InstitutionId, i.SchoolYear }

             join la in this.DbContext.Set<LocalArea>() on i.LocalAreaId equals la.LocalAreaId
             into g1
             from la in g1.DefaultIfEmpty()

             join t in this.DbContext.Set<Town>() on i.TownId equals t.TownId
             into g2
             from t in g2.DefaultIfEmpty()

             join m in this.DbContext.Set<Municipality>() on t.MunicipalityId equals m.MunicipalityId
             into g3
             from m in g3.DefaultIfEmpty()

             join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId
             into g4
             from r in g4.DefaultIfEmpty()

             where rbq.SchoolYear == schoolYear && rbq.Id == id
             select new GetWordDataVO(
                 i.Name,
                 !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                 !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                 !string.IsNullOrEmpty(la.Name) ? $"район {la.Name}," : string.Empty,
                 !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                 rbq.RegistrationNumberTotal,
                 bd.Name,
                 rbq.RegistrationNumberYear,
                 rbq.RegistrationDate,
                 StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName)
             )).SingleAsync(ct);
    }
}
