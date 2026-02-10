namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IRegBookCertificateQueryRepository;

internal class RegBookCertificateQueryRepository : Repository, IRegBookCertificateQueryRepository
{
    public RegBookCertificateQueryRepository(UnitOfWork unitOfWork)
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
        var predicate = PredicateBuilder.True<RegBookCertificate>();
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
            from rbq in this.DbContext.Set<RegBookCertificate>().Where(predicate)
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
                rbq.IsCancelled))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int id,
        CancellationToken ct)
    {
        var duplicates = await
            (from d in this.DbContext.Set<RegBookCertificateDuplicate>()
             where d.SchoolYear == schoolYear && d.Id == id
             select new GetVODuplicate(
                 d.Id,
                 d.RegistrationNumberTotal + (d.RegistrationDate != null ? $"/{d.RegistrationDate.Value:dd.MM.yyyy}" : string.Empty)
             )).ToArrayAsync(ct);

        return await
            (from rbc in this.DbContext.Set<RegBookCertificate>()
             join p in this.DbContext.Set<Person>() on rbc.PersonId equals p.PersonId
             join bd in this.DbContext.Set<BasicDocument>() on rbc.BasicDocumentId equals bd.Id

             join ef in this.DbContext.Set<EduForm>() on rbc.EduFormId equals ef.ClassEduFormId
             into j1
             from ef in j1.DefaultIfEmpty()

             where rbc.SchoolYear == schoolYear && rbc.Id == id
             select new GetVO(
                 rbc.Id,
                 rbc.RegistrationNumberTotal,
                 rbc.RegistrationNumberYear,
                 rbc.RegistrationDate,
                 p.FirstName,
                 p.MiddleName,
                 p.LastName,
                 p.PersonalId,
                 bd.Name,
                 ef.Name,
                 rbc.Series,
                 rbc.FactoryNumber,
                 duplicates
             )).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO[]> GetExcelDataAsync(int schoolYear, int instId, int basicDocumentId, CancellationToken ct)
    {
        var origDocumentData = await
            (from rbc in this.DbContext.Set<RegBookCertificate>()
             join p in this.DbContext.Set<Person>() on rbc.PersonId equals p.PersonId
             join bd in this.DbContext.Set<BasicDocument>() on rbc.BasicDocumentId equals bd.Id

             join ef in this.DbContext.Set<EduForm>() on rbc.EduFormId equals ef.ClassEduFormId
             into j1
             from ef in j1.DefaultIfEmpty()

             where rbc.SchoolYear == schoolYear && rbc.InstitutionId == instId && rbc.BasicDocumentId == basicDocumentId

             orderby rbc.RegistrationNumberTotal.Length, rbc.RegistrationNumberTotal

             select new
             {
                 rbc.Id,
                 rbc.RegistrationNumberTotal,
                 rbc.RegistrationNumberYear,
                 rbc.RegistrationDate,
                 p.FirstName,
                 p.MiddleName,
                 p.LastName,
                 p.PersonalId,
                 BasicDocumentName = bd.Name,
                 EduFormName = ef.Name,
                 rbc.Series,
                 rbc.FactoryNumber
             }).ToArrayAsync(ct);

        var duplicates = (await
            (from d in this.DbContext.Set<RegBookCertificateDuplicate>()
             where d.SchoolYear == schoolYear && d.InstitutionId == instId
             select new
             {
                 d.Id,
                 d.RegistrationNumberTotal,
                 d.RegistrationDate
             }).ToArrayAsync(ct)).ToLookup(c => c.Id);

        return origDocumentData.Select(od => new GetExcelDataVO
        (
            od.Id,
            od.RegistrationNumberTotal,
            od.RegistrationNumberYear,
            od.RegistrationDate,
            od.FirstName,
            od.MiddleName,
            od.LastName,
            od.PersonalId,
            od.BasicDocumentName,
            od.EduFormName,
            od.Series,
            od.FactoryNumber,
            duplicates[od.Id].Select(d =>
                d.RegistrationNumberTotal + (d.RegistrationDate != null ? $"/{d.RegistrationDate.Value:dd.MM.yyyy}" : string.Empty)
            ).ToArray()
        )).ToArray();
    }

    public async Task<GetWordDataVO> GetWordDataAsync(int schoolYear, int id, CancellationToken ct)
    {
        return await
            (from rbc in this.DbContext.Set<RegBookCertificate>()
             join p in this.DbContext.Set<Person>() on rbc.PersonId equals p.PersonId
             join bd in this.DbContext.Set<BasicDocument>() on rbc.BasicDocumentId equals bd.Id
             join i in this.DbContext.Set<InstitutionSchoolYear>() on new { rbc.InstitutionId, rbc.SchoolYear } equals new { i.InstitutionId, i.SchoolYear }

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

             where rbc.SchoolYear == schoolYear && rbc.Id == id
             select new GetWordDataVO(
                 i.Name,
                 !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                 !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                 !string.IsNullOrEmpty(la.Name) ? $"район {la.Name}," : string.Empty,
                 !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                 rbc.RegistrationNumberTotal,
                 bd.Name,
                 rbc.RegistrationNumberYear,
                 rbc.RegistrationDate,
                 StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName)
             )).SingleAsync(ct);
    }
}
