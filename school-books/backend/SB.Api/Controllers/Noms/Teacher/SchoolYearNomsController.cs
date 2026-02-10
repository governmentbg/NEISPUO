namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Data;

public class SchoolYearNomsController : InstitutionNomsController
{
    // TODO
    private const int FirstSchoolYear = 2016;
    private static string GetNomName(int year) => $"{year}-{year + 1}";

    public NomVO[] GetNomsById(
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromQuery][NotNull] int[] ids)
    {
        return ids.Select(y => new NomVO(y, GetNomName(y))).ToArray();
    }

    public NomVO[] GetNomsByTerm(
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromQuery][SuppressMessage("", "IDE0060")] string? term,
        [FromQuery] int? offset,
        [FromQuery] int? limit)
    {
        var lastSyear = DateTime.Now.Month <= 7
            ? DateTime.Now.Year - 1
            : DateTime.Now.Year;

        return Enumerable.Range(FirstSchoolYear, lastSyear + 1 - FirstSchoolYear)
            .OrderByDescending(sy => sy)
            .Select(year => new NomVO(year, GetNomName(year)))
            .WithOffsetAndLimit(offset, limit)
            .ToArray();
    }
}
