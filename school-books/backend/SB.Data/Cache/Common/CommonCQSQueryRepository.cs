namespace SB.Data;

using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.ICommonCQSQueryRepository;

internal class CommonCQSQueryRepository : Repository, ICommonCQSQueryRepository
{
    public CommonCQSQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllExtProvidersVO[]> GetAllExtProvidersAsync(int schoolYear, CancellationToken ct)
    {
        return await (
            // start from InstitutionSchoolYear because there
            // may not be a record in ClassBookExtProvider
            from isy in this.DbContext.Set<InstitutionSchoolYear>()

            join cbep in this.DbContext.Set<ClassBookExtProvider>()
                on new { isy.SchoolYear, isy.InstitutionId }
                equals new { cbep.SchoolYear, InstitutionId = cbep.InstId }
            into j1 from cbep in j1.DefaultIfEmpty()

            where isy.SchoolYear == schoolYear
            select new GetAllExtProvidersVO(
                isy.InstitutionId,
                cbep.ExtSystemId,
                cbep.ScheduleExtSystemId)
        ).ToArrayAsync(ct);
    }

    public async Task<bool> GetInstitutionSchoolYearIsFinalizedAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        var isy = await this.DbContext.Set<InstitutionSchoolYear>()
            .Where(isy =>
                isy.SchoolYear == schoolYear &&
                isy.InstitutionId == instId)
            .SingleOrDefaultAsync(ct);

        return isy?.IsFinalized ?? false;
    }

    public async Task<bool> ShouldSendNotificationAsync(
        int personId,
        StudentSettingsNotificationType notificationType,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<StudentSettings>();
        predicate = predicate.AndEquals(ss => ss.PersonId, personId);

        switch (notificationType)
        {
            case StudentSettingsNotificationType.GradeEmail:
                predicate = predicate.And(ss => ss.AllowGradeEmails);
                break;
            case StudentSettingsNotificationType.AbsenceEmail:
                predicate = predicate.And(ss => ss.AllowAbsenceEmails);
                break;
            case StudentSettingsNotificationType.RemarkEmail:
                predicate = predicate.And(ss => ss.AllowRemarkEmails);
                break;
            case StudentSettingsNotificationType.MessageEmail:
                predicate = predicate.And(ss => ss.AllowAbsenceEmails);
                break;
            case StudentSettingsNotificationType.GradePushNotification:
                predicate = predicate.And(ss => ss.AllowGradeNotifications);
                break;
            case StudentSettingsNotificationType.AbsencePushNotification:
                predicate = predicate.And(ss => ss.AllowAbsenceNotifications);
                break;
            case StudentSettingsNotificationType.RemarkPushNotification:
                predicate = predicate.And(ss => ss.AllowRemarkNotifications);
                break;
            case StudentSettingsNotificationType.MessagePushNotification:
                predicate = predicate.And(ss => ss.AllowMessageNotifications);
                break;
            default:
                throw new InvalidEnumArgumentException($"Invalid NotificationType - {notificationType}");
        }

        return await this.DbContext.Set<StudentSettings>().Where(predicate).AnyAsync(ct);
    }
}
