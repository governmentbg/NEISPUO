namespace SB.Data;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.ISpbsBookRecordsQueryRepository;

internal class SpbsBookRecordsQueryRepository : Repository, ISpbsBookRecordsQueryRepository
{
    public SpbsBookRecordsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int instId,
        int? recordSchoolYear,
        int? recordNumber,
        string? studentName,
        string? personalId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<SpbsBookRecord>();
        predicate = predicate.AndEquals(sbr => sbr.SchoolYear, recordSchoolYear);
        predicate = predicate.AndEquals(sbr => sbr.RecordNumber, recordNumber);

        var personPredicate = PredicateBuilder.True<Person>();
        personPredicate = personPredicate.AndStringContains(p => p.PersonalId, personalId);

        if (!string.IsNullOrWhiteSpace(studentName))
        {
            string[] words = studentName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                personPredicate = personPredicate.AndAnyStringContains(p => p.FirstName, p => p.MiddleName, p => p.LastName, word);
            }
        }

        return await (
            from sbr in this.DbContext.Set<SpbsBookRecord>().Where(predicate)

            join p in this.DbContext.Set<Person>().Where(personPredicate) on sbr.PersonId equals p.PersonId

            join g in this.DbContext.Set<Gender>() on p.Gender equals g.GenderId

            where sbr.InstId == instId

            orderby sbr.SchoolYear, sbr.RecordNumber

            select new GetAllVO(
                sbr.SpbsBookRecordId,
                sbr.SchoolYear,
                sbr.RecordNumber,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                p.PersonalId,
                g.Name)
        )
        .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int spbsBookRecordId,
        CancellationToken ct)
    {
        var spbsBookRecord = await (
            from sbr in this.DbContext.Set<SpbsBookRecord>()

            where sbr.SchoolYear == schoolYear && sbr.SpbsBookRecordId == spbsBookRecordId

            select new
            {
                sbr.PersonId,
                sbr.SendingCommission,
                sbr.SendingCommissionAddress,
                sbr.SendingCommissionPhoneNumber,
                sbr.InspectorNames,
                sbr.InspectorAddress,
                sbr.InspectorPhoneNumber,
            }
        )
        .SingleAsync(ct);

        var classBookNames = string.Join(", ", await (
            from cb in this.DbContext.ClassBooksForStudents(schoolYear, new int[] { spbsBookRecord.PersonId })
            select cb.FullBookName)
        .ToArrayAsync(ct));

        var studentPersonalInfo = await (
            from sbr in this.DbContext.Set<SpbsBookRecord>()

            join p in this.DbContext.Set<Person>()
                on sbr.PersonId equals p.PersonId

            join g in this.DbContext.Set<Gender>()
                on p.Gender equals g.GenderId

            join tp in this.DbContext.Set<Town>()
                on p.PermanentTownId equals tp.TownId
                into j1 from tp in j1.DefaultIfEmpty()

            join tb in this.DbContext.Set<Town>()
                on p.BirthPlaceTownId equals tb.TownId
                into j2 from tb in j2.DefaultIfEmpty()

            join cbp in this.DbContext.Set<Country>()
                on p.BirthPlaceCountry equals cbp.CountryId
                into j3 from cbp in j3.DefaultIfEmpty()

            where sbr.SchoolYear == schoolYear && sbr.SpbsBookRecordId == spbsBookRecordId

            select new GetVOStudent(
                sbr.SchoolYear,
                sbr.RecordNumber,
                p.PersonalId,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                g.Name,
                cbp.Name,
                tb.Name,
                tp.Name,
                p.PermanentAddress,
                classBookNames)
        )
        .SingleAsync(ct);

        var studentRelatives = (await (
            from sbr in this.DbContext.Set<SpbsBookRecord>()

            join r in this.DbContext.Set<Relative>() on sbr.PersonId equals r.PersonId

            where sbr.SchoolYear == schoolYear && sbr.SpbsBookRecordId == spbsBookRecordId

            select new
            {
                r.FirstName,
                r.MiddleName,
                r.LastName,
                r.PhoneNumber,
                r.Email
            }
        )
        .ToListAsync(ct))
        .Select(sr =>
            new GetVORelative(
                StringUtils.JoinNames(sr.FirstName, sr.MiddleName, sr.LastName),
                sr.PhoneNumber,
                sr.Email,
                ""))
        .ToArray();

        var studentMovement = await (
            from sbrm in this.DbContext.Set<SpbsBookRecordMovement>()

            join iin in this.DbContext.Set<InstitutionSchoolYear>()
            on new { sbrm.IncomingInstId, sbrm.SchoolYear } equals new { IncomingInstId = (int?)iin.InstitutionId, iin.SchoolYear }
            into j1 from iin in j1.DefaultIfEmpty()

            join tin in this.DbContext.Set<InstitutionSchoolYear>()
            on new { sbrm.TransferInstId, sbrm.SchoolYear } equals new { TransferInstId = (int?)tin.InstitutionId, tin.SchoolYear }
            into j2 from tin in j2.DefaultIfEmpty()

            where sbrm.SchoolYear == schoolYear && sbrm.SpbsBookRecordId == spbsBookRecordId

            orderby sbrm.OrderNum

            select new GetVOMovement(
                sbrm.OrderNum,
                sbrm.CourtDecisionNumber,
                sbrm.CourtDecisionDate,
                iin.Name,
                sbrm.IncommingLetterNumber,
                sbrm.IncommingLetterDate,
                sbrm.IncommingDate,
                sbrm.IncommingDocNumber,
                tin.Name,
                sbrm.TransferReason,
                sbrm.TransferProtocolNumber,
                sbrm.TransferProtocolDate,
                sbrm.TransferLetterNumber,
                sbrm.TransferLetterDate,
                sbrm.TransferCertificateNumber,
                sbrm.TransferCertificateDate,
                sbrm.TransferMessageNumber,
                sbrm.TransferMessageDate)
        )
        .SingleAsync(ct);

        return new GetVO(
            studentPersonalInfo,
            studentRelatives,
            spbsBookRecord.SendingCommission,
            spbsBookRecord.SendingCommissionAddress,
            spbsBookRecord.SendingCommissionPhoneNumber,
            spbsBookRecord.InspectorNames,
            spbsBookRecord.InspectorAddress,
            spbsBookRecord.InspectorPhoneNumber,
            studentMovement);
    }

    public async Task<TableResultVO<GetEscapeVO>> GetEscapeAllAsync(int schoolYear, int spbsBookRecordId, int? offset, int? limit, CancellationToken ct)
    {
        return await this.DbContext.Set<SpbsBookRecordEscape>()
            .Where(sbre =>
                sbre.SchoolYear == schoolYear &&
                sbre.SpbsBookRecordId == spbsBookRecordId)
            .OrderBy(sbre => sbre.OrderNum)
            .Select(sbre => new GetEscapeVO(
                sbre.OrderNum,
                sbre.EscapeDate,
                sbre.EscapeTime,
                sbre.PoliceNotificationDate,
                sbre.PoliceNotificationTime,
                sbre.PoliceLetterNumber,
                sbre.PoliceLetterDate,
                sbre.ReturnDate))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetEscapeVO> GetEscapeAsync(int schoolYear, int spbsBookRecordId, int orderNum, CancellationToken ct)
    {
        return await this.DbContext.Set<SpbsBookRecordEscape>()
            .Where(sbre =>
                sbre.SchoolYear == schoolYear &&
                sbre.SpbsBookRecordId == spbsBookRecordId &&
                sbre.OrderNum == orderNum)
            .Select(sbre => new GetEscapeVO(
                sbre.OrderNum,
                sbre.EscapeDate,
                sbre.EscapeTime,
                sbre.PoliceNotificationDate,
                sbre.PoliceNotificationTime,
                sbre.PoliceLetterNumber,
                sbre.PoliceLetterDate,
                sbre.ReturnDate))
            .SingleAsync(ct);
    }

    public async Task<TableResultVO<GetAbsenceVO>> GetAbsenceAllAsync(int schoolYear, int spbsBookRecordId, int? offset, int? limit, CancellationToken ct)
    {
        return await this.DbContext.Set<SpbsBookRecordAbsence>()
            .Where(sbra =>
                sbra.SchoolYear == schoolYear &&
                sbra.SpbsBookRecordId == spbsBookRecordId)
            .OrderBy(sbra => sbra.OrderNum)
            .Select(sbra => new GetAbsenceVO(
                sbra.OrderNum,
                sbra.AbsenceDate,
                sbra.AbsenceReason))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetAbsenceVO> GetAbsenceAsync(int schoolYear, int spbsBookRecordId, int orderNum, CancellationToken ct)
    {
        return await this.DbContext.Set<SpbsBookRecordAbsence>()
            .Where(sbra =>
                sbra.SchoolYear == schoolYear &&
                sbra.SpbsBookRecordId == spbsBookRecordId &&
                sbra.OrderNum == orderNum)
            .Select(sbra => new GetAbsenceVO(
                sbra.OrderNum,
                sbra.AbsenceDate,
                sbra.AbsenceReason))
            .SingleAsync(ct);
    }
}
