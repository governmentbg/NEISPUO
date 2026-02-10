namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.IStudentSettingsQueryRepository;

internal class StudentSettingsQueryRepository : Repository, IStudentSettingsQueryRepository
{
    public StudentSettingsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetVO?> GetStudentSettings(int personId, CancellationToken ct)
    {
        return await this.DbContext.Set<StudentSettings>()
            .Where(ss => ss.PersonId == personId)
            .Select(ss => new GetVO(
                ss.AllowGradeEmails,
                ss.AllowAbsenceEmails,
                ss.AllowRemarkEmails,
                ss.AllowMessageEmails,
                ss.AllowGradeNotifications,
                ss.AllowAbsenceNotifications,
                ss.AllowRemarkNotifications,
                ss.AllowMessageNotifications))
            .FirstOrDefaultAsync(ct);
    }
}
