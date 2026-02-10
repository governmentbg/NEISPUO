namespace SB.Api;

using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class AuthService : IAuthService
{
    private readonly IApiAuthCachedQueryStore apiAuthCachedQueryStore;

    public AuthService(IApiAuthCachedQueryStore apiAuthCachedQueryStore)
    {
        this.apiAuthCachedQueryStore = apiAuthCachedQueryStore;
    }

    public async Task<bool> HasConversationsAccessAsync(
        OidcToken token,
        AccessType accessType,
        int instId,
        CancellationToken ct)
    {
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? roleInstId = token.SelectedRole.InstitutionId;
        int[]? studentIds = token.SelectedRole.StudentPersonIds;

        bool result = sysRoleId switch
        {
            SysRole.Institution or
            SysRole.InstitutionExpert or
            SysRole.Teacher => roleInstId == instId,

            SysRole.Parent => studentIds != null &&
                              await this.apiAuthCachedQueryStore
                                  .GetStudentsBelongsToInstitutionAsync(
                                      ConversationsHelper.CurrentSchoolYear(),
                                      instId,
                                      studentIds,
                                      ct),

            _ => false
        };

        return result;
    }

    public async Task<bool> HasInstitutionAccessAsync(
        OidcToken token,
        AccessType accessType,
        int instId,
        CancellationToken ct)
    {
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? roleInstId = token.SelectedRole.InstitutionId;
        int? roleRegionId = token.SelectedRole.RegionId;

        switch (accessType)
        {
            case AccessType.Read:
                switch (sysRoleId)
                {
                    case SysRole.CIOO:
                    case SysRole.MonAdmin:
                    case SysRole.MonExpert:
                        return true;

                    case SysRole.RuoAdmin:
                    case SysRole.RuoExpert:
                        return
                            roleRegionId.HasValue &&
                            await this.apiAuthCachedQueryStore.GetInstitutionBelongsToRegionAsync(instId, roleRegionId.Value, ct);

                    case SysRole.Institution:
                    case SysRole.InstitutionExpert:
                    case SysRole.Teacher:
                        return roleInstId == instId;

                    default:
                        return false;
                }

            case AccessType.Write:
                switch (sysRoleId)
                {
                    case SysRole.Institution:
                    case SysRole.InstitutionExpert:
                    case SysRole.Teacher:
                        return roleInstId == instId;

                    default:
                        return false;
                }

            default:
                return false;
        }
    }

    public async Task<bool> HasInstitutionAdminAccessAsync(
        OidcToken token,
        AccessType accessType,
        int instId,
        CancellationToken ct)
    {
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? roleInstId = token.SelectedRole.InstitutionId;
        int? roleRegionId = token.SelectedRole.RegionId;

        switch (accessType)
        {
            case AccessType.Read:
                switch (sysRoleId)
                {
                    case SysRole.CIOO:
                    case SysRole.MonAdmin:
                    case SysRole.MonExpert:
                        return true;

                    case SysRole.RuoAdmin:
                    case SysRole.RuoExpert:
                        return
                            roleRegionId.HasValue &&
                            await this.apiAuthCachedQueryStore.GetInstitutionBelongsToRegionAsync(instId, roleRegionId.Value, ct);
                }
                goto case AccessType.Write;

            case AccessType.Write:
                switch (sysRoleId)
                {
                    case SysRole.Institution:
                    case SysRole.InstitutionExpert:
                        return roleInstId == instId;
                }
                goto default;

            default:
                return false;
        }
    }

    public async Task<bool> HasClassBookAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct)
    {
        string jti = token.Jti;
        DateTime exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(token.Exp)).LocalDateTime;
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? roleInstId = token.SelectedRole.InstitutionId;
        int? roleRegionId = token.SelectedRole.RegionId;
        int? rolePersonId = token.SelectedRole.PersonId;

        switch (accessType)
        {
            case AccessType.Read:
                switch (sysRoleId)
                {
                    case SysRole.CIOO:
                    case SysRole.MonAdmin:
                    case SysRole.MonExpert:
                        return true;

                    case SysRole.RuoAdmin:
                    case SysRole.RuoExpert:
                        return
                            roleRegionId.HasValue &&
                            await this.apiAuthCachedQueryStore.GetInstitutionBelongsToRegionAsync(instId, roleRegionId.Value, ct) &&
                            await this.apiAuthCachedQueryStore.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct);
                }
                goto case AccessType.Write;

            case AccessType.Write:
                switch (sysRoleId)
                {
                    case SysRole.Institution:
                    case SysRole.InstitutionExpert:
                        return
                            roleInstId == instId &&
                            await this.apiAuthCachedQueryStore.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct);

                    case SysRole.Teacher:
                        return
                            roleInstId == instId &&
                            rolePersonId.HasValue &&
                            await this.apiAuthCachedQueryStore.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct) &&
                            await this.apiAuthCachedQueryStore.GetPersonIsClassBookTeacherAsync(jti, exp, rolePersonId.Value, schoolYear, instId, classBookId, ct);
                }
                goto default;

            default:
                return false;
        }
    }

    public async Task<bool> HasStudentInfoClassBookAccessAsync(
        OidcToken token,
        int schoolYear,
        int instId,
        int classBookId,
        int personId,
        CancellationToken ct)
    {
        return await this.HasClassBookAccessAsync(token, AccessType.Read, schoolYear, instId, classBookId, ct) &&
            await this.apiAuthCachedQueryStore.GetStudentBelongsToClassBookAsync(schoolYear, instId, classBookId, personId, ct);

    }

    public async Task<bool> HasClassBookAdminAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct)
    {
        string jti = token.Jti;
        DateTime exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(token.Exp)).LocalDateTime;
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? roleInstId = token.SelectedRole.InstitutionId;
        int? rolePersonId = token.SelectedRole.PersonId;

        switch (accessType)
        {
            case AccessType.Read:
                // same as ClassBookAccess - Read
                return await this.HasClassBookAccessAsync(token, AccessType.Read, schoolYear, instId, classBookId, ct);

            case AccessType.Write:
                switch (sysRoleId)
                {
                    case SysRole.Institution:
                    case SysRole.InstitutionExpert:
                        return
                            roleInstId == instId &&
                            await this.apiAuthCachedQueryStore.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct);

                    case SysRole.Teacher:
                        return
                            roleInstId == instId &&
                            rolePersonId.HasValue &&
                            await this.apiAuthCachedQueryStore.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct) &&
                            await this.apiAuthCachedQueryStore.GetPersonIsClassBookLeadTeacherAsync(jti, exp, rolePersonId.Value, schoolYear, instId, classBookId, ct);

                    default:
                        return false;
                }

            default:
                return false;
        }
    }

    public async Task<bool> HasCurriculumAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        int curriculumId,
        CancellationToken ct)
    {
        string jti = token.Jti;
        DateTime exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(token.Exp)).LocalDateTime;
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? roleInstId = token.SelectedRole.InstitutionId;
        int? rolePersonId = token.SelectedRole.PersonId;

        switch (accessType)
        {
            case AccessType.Read:
                return false;

            case AccessType.Write:
                switch (sysRoleId)
                {
                    case SysRole.Institution:
                    case SysRole.InstitutionExpert:
                        return
                            roleInstId == instId &&
                            await this.apiAuthCachedQueryStore.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct);

                    case SysRole.Teacher:
                        return
                            roleInstId == instId &&
                            rolePersonId.HasValue &&
                            await this.apiAuthCachedQueryStore.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct) &&
                            await this.apiAuthCachedQueryStore.GetPersonIsClassBookLeadTeacherOrCurriculumTeacherAsync(jti, exp, rolePersonId.Value, schoolYear, instId, classBookId, curriculumId, ct);

                    default:
                        return false;
                }

            default:
                return false;
        }
    }

    public async Task<bool> HasReplCurriculumAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        int curriculumId,
        DateTime date,
        CancellationToken ct)
    {
        string jti = token.Jti;
        DateTime exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(token.Exp)).LocalDateTime;
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? rolePersonId = token.SelectedRole.PersonId;

        return await this.HasClassBookAdminAccessAsync(token, accessType, schoolYear, instId, classBookId, ct) ||
            (sysRoleId == SysRole.Teacher &&
            rolePersonId.HasValue &&
            await this.apiAuthCachedQueryStore.GetPersonIsReplTeacherForDateAndCurriculumAsync(jti, exp, rolePersonId.Value, schoolYear, instId, classBookId, curriculumId, date, ct));
    }

    public async Task<bool> HasScheduleLessonAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleLessonId,
        CancellationToken ct)
    {
        string jti = token.Jti;
        DateTime exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(token.Exp)).LocalDateTime;
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? roleInstId = token.SelectedRole.InstitutionId;
        int? roleRegionId = token.SelectedRole.RegionId;
        int? rolePersonId = token.SelectedRole.PersonId;

        switch (accessType)
        {
            case AccessType.Read:
                switch (sysRoleId)
                {
                    case SysRole.CIOO:
                    case SysRole.MonAdmin:
                    case SysRole.MonExpert:
                        return true;

                    case SysRole.RuoAdmin:
                    case SysRole.RuoExpert:
                        return
                            roleRegionId.HasValue &&
                            await this.apiAuthCachedQueryStore.GetInstitutionBelongsToRegionAsync(instId, roleRegionId.Value, ct) &&
                            await this.apiAuthCachedQueryStore.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct);
                }
                goto case AccessType.Write;

            case AccessType.Write:
                switch (sysRoleId)
                {
                    case SysRole.Institution:
                    case SysRole.InstitutionExpert:
                        return
                            roleInstId == instId &&
                            await this.apiAuthCachedQueryStore.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct);

                    case SysRole.Teacher:
                        return
                            roleInstId == instId &&
                            rolePersonId.HasValue &&
                            await this.apiAuthCachedQueryStore.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct) &&
                            await this.apiAuthCachedQueryStore.GetPersonIsClassBookLeadTeacherOrLessonTeacherAsync(jti, exp, rolePersonId.Value, schoolYear, instId, classBookId, scheduleLessonId, ct);

                    default:
                        return false;
                }

            default:
                return false;
        }
    }

    public async Task<bool> HasSupportAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        int supportId,
        CancellationToken ct)
    {
        string jti = token.Jti;
        DateTime exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(token.Exp)).LocalDateTime;
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? rolePersonId = token.SelectedRole.PersonId;

        return await this.HasClassBookAdminAccessAsync(token, accessType, schoolYear, instId, classBookId, ct) ||
            (sysRoleId == SysRole.Teacher &&
            rolePersonId.HasValue &&
            await this.apiAuthCachedQueryStore.GetPersonIsInSupportTeamAsync(jti, exp, rolePersonId.Value, schoolYear, instId, classBookId, supportId, ct));
    }

    public async Task<bool> HasAttendanceDateAccessAsync(
        OidcToken token,
        AccessType accessType,
        int schoolYear,
        int instId,
        int classBookId,
        DateTime date,
        CancellationToken ct)
    {
        string jti = token.Jti;
        DateTime exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(token.Exp)).LocalDateTime;
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? rolePersonId = token.SelectedRole.PersonId;

        return await this.HasClassBookAdminAccessAsync(token, accessType, schoolYear, instId, classBookId, ct) ||
            (sysRoleId == SysRole.Teacher &&
            rolePersonId.HasValue &&
            await this.apiAuthCachedQueryStore.GetPersonIsReplTeacherForDateAsync(jti, exp, rolePersonId.Value, schoolYear, instId, classBookId, date, ct));
    }

    public Task<bool> HasStudentAccessAsync(
        OidcToken token,
        CancellationToken ct)
    {
        SysRole sysRoleId = token.SelectedRole.SysRoleId;

        bool result = sysRoleId switch
        {
            SysRole.Student or
            SysRole.Parent => true,

            _ => false
        };

        return Task.FromResult(result);
    }

    public Task<bool> HasStudentClassBookAccessAsync(
        OidcToken token,
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct)
    {
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? selectedRolePersonId = token.SelectedRole.PersonId;
        int[]? selectedRoleStudentPersonIds = token.SelectedRole.StudentPersonIds;
        int[]? classBookIds = token.SelectedRole.ClassBookIds;

        bool result = sysRoleId switch
        {
            SysRole.Student =>
                selectedRolePersonId == personId
                && classBookIds?.Any(cbId => cbId == classBookId) == true,
            SysRole.Parent =>
                selectedRoleStudentPersonIds?.Any(pId => pId == personId) == true
                && classBookIds?.Any(cbId => cbId == classBookId) == true,

            _ => false
        };

        return Task.FromResult(result);
    }

    public Task<bool> HasStudentMedicalNoticesAccessAsync(
        OidcToken token,
        int schoolYear,
        int personId,
        CancellationToken ct)
    {
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int[]? selectedRoleStudentPersonIds = token.SelectedRole.StudentPersonIds;

        bool result = sysRoleId switch
        {
            SysRole.Parent =>
                selectedRoleStudentPersonIds?.Any(pId => pId == personId) == true,

            _ => false
        };

        return Task.FromResult(result);
    }

    public async Task<bool> HasHisMedicalNoticeAccessAsync(
        OidcToken token,
        int hisMedicalNoticeId,
        CancellationToken ct)
    {
        SysRole sysRoleId = token.SelectedRole.SysRoleId;
        int? roleRegionId = token.SelectedRole.RegionId;

        switch (sysRoleId)
        {
            case SysRole.CIOO:
            case SysRole.MonAdmin:
            case SysRole.MonExpert:
                return true;

            case SysRole.RuoAdmin:
            case SysRole.RuoExpert:
                return
                    roleRegionId.HasValue &&
                    await this.apiAuthCachedQueryStore.GetHisMedicalNoticeBelongsToRegionAsync(hisMedicalNoticeId, roleRegionId.Value, ct);

            default: return false;
        }
    }
}
