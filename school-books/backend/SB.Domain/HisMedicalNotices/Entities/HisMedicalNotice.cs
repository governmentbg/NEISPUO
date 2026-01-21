namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class HisMedicalNotice : IAggregateRoot
{
    // EF constructor
    private HisMedicalNotice()
    {
        this.NrnMedicalNotice = null!;
        this.NrnExamination = null!;
        this.Identifier = null!;
        this.GivenName = null!;
        this.FamilyName = null!;
        this.Pmi = null!;
    }

    public HisMedicalNotice(
        int hisMedicalNoticeBatchId,
        DateTime batchCreateDate,
        string nrnMedicalNotice,
        string nrnExamination,
        int identifierType,
        string identifier,
        string givenName,
        string familyName,
        string pmi,
        DateTime fromDate,
        DateTime toDate,
        DateTime authoredOn)
    {
        this.HisMedicalNoticeBatchId = hisMedicalNoticeBatchId;
        this.CreateDate = batchCreateDate;
        this.PersonalIdTypeId = identifierType switch
        {
            1 => 0, // ЕГН
            2 => 1, // ЛНЧ
            3 or 4 or 5 => 2, // Друг идентификатор
            _ => throw new ArgumentException($"Invalid identifier type: {identifierType}", nameof(identifierType))
        };

        if (fromDate > toDate)
        {
            throw new ArgumentException($"Invalid date range: {fromDate} - {toDate}");
        }

        if (fromDate < new DateTime(2022, 09, 01))
        {
            throw new ArgumentException($"The {nameof(fromDate)} must be after 2022-09-01");
        }

        static bool areIntersecting(DateTime fromDate1, DateTime toDate1, DateTime fromDate2, DateTime toDate2)
            => fromDate1 <= toDate2 && toDate1 >= fromDate2;

        static (DateTime from, DateTime to) schoolYearPeriod(int year)
            => (from: new(year, 9, 01), to: new(year + 1, 9, 30));

        this.schoolYears.AddRange(
            Enumerable.Range(2022, toDate.Year - 2022 + 1)
                .Where(sy =>
                {
                    var (syFrom, syTo) = schoolYearPeriod(sy);
                    return areIntersecting(fromDate, toDate, syFrom, syTo);
                })
                .Select(sy => new HisMedicalNoticeSchoolYear(this, sy)));

        this.NrnMedicalNotice = nrnMedicalNotice;
        this.NrnExamination = nrnExamination;
        this.IdentifierType = identifierType;
        this.Identifier = identifier;
        this.GivenName = givenName;
        this.FamilyName = familyName;
        this.Pmi = pmi;
        this.FromDate = fromDate;
        this.ToDate = toDate;
        this.AuthoredOn = authoredOn;
    }

    public int HisMedicalNoticeId { get; private set; }

    public int HisMedicalNoticeBatchId { get; private set; }

    public int PersonalIdTypeId { get; private set; }

    public string NrnMedicalNotice { get; private set; }

    public string NrnExamination { get; private set; }

    public int IdentifierType { get; private set; }

    public string Identifier { get; private set; }

    public string GivenName { get; private set; }

    public string FamilyName { get; private set; }

    public string Pmi { get; private set; }

    public DateTime FromDate { get; private set; }

    public DateTime ToDate { get; private set; }

    public DateTime AuthoredOn { get; private set; }

    public DateTime CreateDate { get; private set; }

    public byte[] Version { get; private set; } = null!;

    // relations
    private readonly List<HisMedicalNoticeSchoolYear> schoolYears = new();
    public IReadOnlyCollection<HisMedicalNoticeSchoolYear> SchoolYears => this.schoolYears.AsReadOnly();
}
