namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IHisMedicalNoticesQueryRepository;

[ApiController]
[Route("api/[controller]")]
public class HisMedicalNoticesController
{
    [Authorize(Policy = Policies.AuthenticatedAccess)]
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromQuery] int schoolYear,
        [FromQuery] string? nrnMedicalNotice,
        [FromQuery] string? nrnExamination,
        [FromQuery] string? identifier,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IHisMedicalNoticesQueryRepository hisMedicalNoticesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        TableResultVO<GetAllVO> medicalNotices;
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? regionId = token.SelectedRole.RegionId;

        if (sysRoleId == SysRole.CIOO
            || sysRoleId == SysRole.MonAdmin
            || sysRoleId == SysRole.MonExpert)
        {
            medicalNotices =
                await hisMedicalNoticesQueryRepository.GetAllAsync(
                    schoolYear,
                    nrnMedicalNotice,
                    nrnExamination,
                    identifier,
                    offset,
                    limit,
                    ct);
        }
        else if (regionId != null &&
            (sysRoleId == SysRole.RuoAdmin
            || sysRoleId == SysRole.RuoExpert))
        {
            medicalNotices =
                await hisMedicalNoticesQueryRepository.GetAllForRegion(
                    regionId.Value,
                    schoolYear,
                    nrnMedicalNotice,
                    nrnExamination,
                    identifier,
                    offset,
                    limit,
                    ct);
        }
        else
        {
            return new ForbidResult();
        }

        return medicalNotices;
    }

    [Authorize(Policy = Policies.HisMedicalNoticeAccess)]
    [HttpGet("{hisMedicalNoticeId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int hisMedicalNoticeId,
        [FromServices] IHisMedicalNoticesQueryRepository hisMedicalNoticesQueryRepository,
        CancellationToken ct)
    {
        return await hisMedicalNoticesQueryRepository.GetAsync(hisMedicalNoticeId, ct);
    }
}
