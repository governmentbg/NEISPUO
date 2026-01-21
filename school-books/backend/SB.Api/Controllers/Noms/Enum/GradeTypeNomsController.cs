namespace SB.Api;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Data;
using SB.Domain;

[Authorize(Policy = Policies.InstitutionAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class GradeTypeNomsController
{
    [HttpGet]
    public IList<EnumNomVO<GradeType>> GetNomsById(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromQuery] GradeType[] ids,
        [FromServices] IEnumNomsRepository<GradeType> repository)
        => repository.GetNomsById(ids);

    [HttpGet]
    public IList<EnumNomVO<GradeType>> GetNomsByTerm(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromQuery] string? term,
        [FromQuery] bool showFinalGradeType,
        [FromQuery] bool showTermGradeType,
        [FromQuery] bool showGradeTypesWithoutScheduleLesson,
        [FromQuery][SuppressMessage("", "IDE0060")] int? offset,
        [FromQuery][SuppressMessage("", "IDE0060")] int? limit,
        [FromServices] IGradeTypeNomsRepository repository)
        => repository.GetNomsByTerm(showFinalGradeType, showTermGradeType, showGradeTypesWithoutScheduleLesson, term);
}
