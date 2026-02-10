namespace SB.Domain.Tests.HisMedicalNotices;

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class HisMedicalNoticeTests
{
    public static IEnumerable<object[]> GetSchoolYearTestData()
    {
        yield return new object[]
        {
            new DateTime(2023, 08, 29),
            new DateTime(2023, 08, 29),
            new[] { 2022 },
        };
        yield return new object[]
        {
            new DateTime(2023, 08, 29),
            new DateTime(2023, 09, 01),
            new[] { 2022, 2023 },
        };
        yield return new object[]
        {
            new DateTime(2023, 09, 25),
            new DateTime(2023, 09, 30),
            new[] { 2022, 2023 },
        };
        yield return new object[]
        {
            new DateTime(2023, 10, 01),
            new DateTime(2023, 10, 15),
            new[] { 2023 },
        };
    }

    [Theory, MemberData(nameof(GetSchoolYearTestData))]
    public void HisMedicalNotice_fills_schoolYears_correctly(DateTime fromDate, DateTime toDate, int[] schoolYears)
    {
        // Act
        var hisMedicalNotice = new HisMedicalNotice(
            1,
            DateTime.Now,
            "nrnMN",
            "nrnE",
            1,
            "1234567890",
            "Иван",
            "Иванов",
            "pmi",
            fromDate,
            toDate,
            DateTime.Now);

        // Assert
        Assert.Equal(schoolYears, hisMedicalNotice.SchoolYears.Select(sy => sy.SchoolYear));
    }
}
