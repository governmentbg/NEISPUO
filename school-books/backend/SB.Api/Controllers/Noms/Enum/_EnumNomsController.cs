namespace SB.Api;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Data;

[Authorize(Policy = Policies.InstitutionAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public abstract class EnumNomsController<TEnum>
    where TEnum : struct, IConvertible
{
    [HttpGet]
    public IList<EnumNomVO<TEnum>> GetNomsById(
        [FromRoute][SuppressMessage("", "IDE0060")]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromQuery]TEnum[] ids,
        [FromServices]IEnumNomsRepository<TEnum> repository)
        => repository.GetNomsById(ids);

    [HttpGet]
    public IList<EnumNomVO<TEnum>> GetNomsByTerm(
        [FromRoute][SuppressMessage("", "IDE0060")]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromQuery]string? term,
        [FromQuery][SuppressMessage("", "IDE0060")]int? offset,
        [FromQuery][SuppressMessage("", "IDE0060")]int? limit,
        [FromServices]IEnumNomsRepository<TEnum> repository)
        => repository.GetNomsByTerm(term);
}
