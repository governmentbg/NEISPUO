namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IInstitutionsQueryRepository;

[ApiController]
[Route("api/[controller]")]
public class InstitutionsController
{
    [Authorize(Policy = Policies.AuthenticatedAccess)]
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromQuery] string? institutionId,
        [FromQuery] string? institutionName,
        [FromQuery] string? townName,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IInstitutionsQueryRepository institutionsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        TableResultVO<GetAllVO> institutions;
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? regionId = token.SelectedRole.RegionId;

        if (sysRoleId == SysRole.CIOO
            || sysRoleId == SysRole.MonAdmin
            || sysRoleId == SysRole.MonExpert)
        {
            institutions =
                await institutionsQueryRepository.GetAllAsync(
                    null,
                    institutionId,
                    institutionName,
                    townName,
                    offset,
                    limit,
                    ct);
        }
        else if (regionId != null &&
            (sysRoleId == SysRole.RuoAdmin
            || sysRoleId == SysRole.RuoExpert))
        {
            institutions =
                await institutionsQueryRepository.GetAllAsync(
                    regionId.Value,
                    institutionId,
                    institutionName,
                    townName,
                    offset,
                    limit,
                    ct);
        }
        else
        {
            return new ForbidResult();
        }

        return institutions;
    }

    [Authorize(Policy = Policies.InstitutionAccess)]
    [HttpGet("{schoolYear:int}/{instId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromServices]IInstitutionsQueryRepository institutionsQueryRepository,
        [FromServices]ICommonCachedQueryStore commonCachedQueryStore,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        bool hasInstitutionAdminReadAccess = await httpContextAccessor.HasInstitutionAdminReadAccessAsync(instId, ct);
        bool hasInstitutionAdminWriteAccess = await httpContextAccessor.HasInstitutionAdminWriteAccessAsync(instId, ct);
        bool hasCBExtProvider = await commonCachedQueryStore.GetInstHasCBExtProviderAsync(schoolYear, instId, ct);
        bool schoolYearIsFinalized = await commonCachedQueryStore.GetSchoolYearIsFinalizedAsync(schoolYear, instId, ct);

        var inst = await institutionsQueryRepository.GetAsync(schoolYear, instId, hasInstitutionAdminReadAccess, ct);

        inst.HasCBExtProvider = hasCBExtProvider;
        inst.SchoolYearIsFinalized = schoolYearIsFinalized;

        inst.HasProtocolsReadAccess =
            inst.HasBooksReadAccess =
            inst.HasOffDayReadAccess =
            inst.HasSchoolYearSettingsReadAccess =
            inst.HasStudentsAtRiskOfDroppingOutReportAccess =
            inst.HasGradelessStudentsReportAccess =
            inst.HasSessionStudentsReportAccess =
            inst.HasAbsencesByStudentsReportAccess =
            inst.HasAbsencesByClassesReportAccess =
            inst.HasRegularGradePointAverageByClassesReportAccess =
            inst.HasRegularGradePointAverageByStudentsReportAccess =
            inst.HasFinalGradePointAverageByStudentsReportAccess =
            inst.HasFinalGradePointAverageByClassesReportAccess =
            inst.HasDateAbsencesReportAccess =
            inst.HasExamsReportAccess =
            inst.HasScheduleAndAbsencesByTermReportAccess =
            inst.HasScheduleAndAbsencesByMonthReportAccess =
            inst.HasScheduleAndAbsencesByTermAllClassesReportAccess =
            inst.HasShiftReadAccess =
            inst.HasPublicationReadAccess =
            inst.HasTeacherAbsencesAllReadAccess =
            inst.HasTeacherSchedulesReadAccess =
            inst.HasVerificationReadAccess =
            hasInstitutionAdminReadAccess;
        inst.HasProtocolsCreateAccess =
            inst.HasProtocolsEditAccess =
            inst.HasProtocolsRemoveAccess =
            inst.HasCreateClassBooksAccess =
            inst.HasSpbsBookCreateAccess =
            inst.HasSpbsBookEditAccess =
            inst.HasSpbsBookRemoveAccess =
            inst.HasMissingTopicsReportAdminCreateAccess =
            inst.HasLectureSchedulesReportAdminCreateAccess =
            inst.HasOffDayCreateAccess =
            inst.HasOffDayEditAccess =
            inst.HasOffDayRemoveAccess =
            inst.HasSchoolYearSettingsCreateAccess =
            inst.HasSchoolYearSettingsEditAccess =
            inst.HasSchoolYearSettingsRemoveAccess =
            inst.HasShiftCreateAccess =
            inst.HasShiftEditAccess =
            inst.HasShiftRemoveAccess =
            inst.HasPublicationCreateAccess =
            inst.HasPublicationEditAccess =
            inst.HasPublicationRemoveAccess =
            inst.HasTeacherAbsencesCreateAccess =
            inst.HasTeacherAbsencesEditAccess =
            inst.HasTeacherAbsencesRemoveAccess =
            inst.HasLectureSchedulesCreateAccess =
            inst.HasLectureSchedulesEditAccess =
            inst.HasLectureSchedulesRemoveAccess =
            inst.HasFinalizationAccess =
            inst.HasVerificationWriteAccess =
            hasInstitutionAdminWriteAccess;

        return inst;
    }
}
