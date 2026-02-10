namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IClassBooksQueryRepository;

public class ClassBooksController : BookController
{
    [HttpGet]
    public async Task<GetVO> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromServices] IClassBooksQueryRepository classBooksQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] ICommonCachedQueryStore commonCachedQueryStore,
        CancellationToken ct)
    {
        var personId = httpContextAccessor.GetPersonId();
        var classBookInfo = await classBooksQueryRepository.GetAsync(schoolYear, classBookId, ct);

        classBookInfo.HasCBExtProvider =
            await commonCachedQueryStore.GetInstHasCBExtProviderAsync(
                schoolYear,
                instId,
                ct);

        classBookInfo.SchoolYearIsFinalized =
            await commonCachedQueryStore.GetSchoolYearIsFinalizedAsync(
                schoolYear,
                instId,
                ct);

        bool hasClassBookWriteAccess =
            await httpContextAccessor.HasClassBookWriteAccessAsync(
                schoolYear,
                instId,
                classBookId,
                ct);

        bool hasClassBookAdminWriteAccess =
            await httpContextAccessor.HasClassBookAdminWriteAccessAsync(
                schoolYear,
                instId,
                classBookId,
                ct);

        bool hasInstitutionAdminReadAccess =
            await httpContextAccessor.HasInstitutionAdminReadAccessAsync(
                instId,
                ct);

        classBookInfo.HasCreateAdditionalActivityAccess =
            classBookInfo.HasCreateNoteAccess =
            classBookInfo.HasCreateIndividualWorkAccess =
            hasClassBookWriteAccess;

        classBookInfo.HasCreateAttendanceAccess =
            classBookInfo.HasRemoveAttendanceAccess =
            classBookInfo.HasRemoveAbsenceAccess =
            classBookInfo.HasRemoveDplrAbsenceAccess =
            classBookInfo.HasEditFirstGradeResultsAccess =
            classBookInfo.HasEditGradeResultsAccess =
            classBookInfo.HasEditGradeResultSessionsAccess =
            classBookInfo.HasCreateParentMeetingAccess =
            classBookInfo.HasEditParentMeetingAccess =
            classBookInfo.HasRemoveParentMeetingAccess =
            classBookInfo.HasCreatePgResultAccess =
            classBookInfo.HasEditPgResultAccess =
            classBookInfo.HasRemovePgResultAccess =
            classBookInfo.HasCreateSanctionAccess =
            classBookInfo.HasEditSanctionAccess =
            classBookInfo.HasRemoveSanctionAccess =
            classBookInfo.HasCreateSupportAccess =
            classBookInfo.HasCreatePerformanceAccess =
            classBookInfo.HasEditPerformanceAccess =
            classBookInfo.HasRemovePerformanceAccess =
            classBookInfo.HasCreateReplrParticipationAccess =
            classBookInfo.HasEditReplrParticipationAccess =
            classBookInfo.HasRemoveReplrParticipationAccess =
            classBookInfo.HasEditAdditionalActivityAccess =
            classBookInfo.HasRemoveAdditionalActivityAccess =
            classBookInfo.HasEditNoteAccess =
            classBookInfo.HasRemoveNoteAccess =
            classBookInfo.HasEditIndividualWorkAccess =
            classBookInfo.HasRemoveIndividualWorkAccess =
            hasClassBookAdminWriteAccess;

        classBookInfo.HasExportForAllBooksPerformanceAccess = hasInstitutionAdminReadAccess;

        classBookInfo.CreateAttendanceReplTeacherAccessDates =
            !classBookInfo.HasCreateAttendanceAccess && personId.HasValue ?
            await classBooksQueryRepository.GetReplTeacherDatesAsync(schoolYear, classBookId, personId.Value, ct)
            : null;

        return classBookInfo;
    }

    [HttpGet]
    public async Task<GetStudentsVO[]> GetStudentsAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromServices]IClassBooksQueryRepository classBooksQueryRepository,
        CancellationToken ct)
        => await classBooksQueryRepository.GetStudentsAsync(schoolYear, classBookId, ct);

    [HttpGet]
    public async Task<GetRemovedStudentsVO[]> GetRemovedStudentsAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromQuery][NotNull]int[] personIds,
        [FromServices]IClassBooksQueryRepository classBooksQueryRepository,
        CancellationToken ct)
        => await classBooksQueryRepository.GetRemovedStudentsAsync(schoolYear, instId, classBookId, personIds, ct);

    [HttpGet]
    public async Task<GetDefaultCurriculumVO> GetDefaultCurriculumAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] bool excludeGradeless,
        [FromServices] IClassBooksQueryRepository classBooksQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await classBooksQueryRepository.GetDefaultCurriculumAsync(
            httpContextAccessor.GetPersonId(),
            schoolYear,
            classBookId,
            excludeGradeless,
            ct);

    [HttpGet]
    public async Task<GetCurriculumsVO[]> GetCurriculumsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] bool excludeGradeless,
        [FromQuery] bool includeInvalidWithGrades,
        [FromQuery] bool includeInvalidWithTopicPlans,
        [FromServices] IClassBooksQueryRepository classBooksQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        bool hasClassBookAdminWriteAccess =
            await authService.HasClassBookAdminAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var curriculums =
            await classBooksQueryRepository.GetCurriculumsAsync(
                schoolYear,
                classBookId,
                excludeGradeless,
                includeInvalidWithGrades,
                includeInvalidWithTopicPlans,
                ct);

        foreach (var curriculum in curriculums)
        {
            curriculum.HasCreateGradeAccess =
                hasClassBookAdminWriteAccess
                || curriculum.WriteAccessCurriculumTeacherPersonIds.Any(pId => pId == token.SelectedRole.PersonId)
                || curriculum.ReplTeacherPersonIds.Any(pId => pId == token.SelectedRole.PersonId);
            curriculum.HasCreateForecastGradeAccess =
            curriculum.HasCreateTopicPlanAccess =
                hasClassBookAdminWriteAccess
                || curriculum.WriteAccessCurriculumTeacherPersonIds.Any(pId => pId == token.SelectedRole.PersonId);
        }

        return curriculums;
    }
}
