namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateUpdateStudentSettingsCommandHandler(
    IUnitOfWork UnitOfWork,
    IStudentSettingsAggregateRepository StudentSettingsAggregateRepository)
    : IRequestHandler<CreateUpdateStudentSettingsCommand, int>
{
    public async Task<int> Handle(CreateUpdateStudentSettingsCommand command, CancellationToken ct)
    {
        var studentSettings =
            await this.StudentSettingsAggregateRepository.FindOrDefaultAsync(command.UserPersonId!.Value, ct);
        int studentSettingsId;

        if (studentSettings == null)
        {
            var newSettings = new StudentSettings(
                command.UserPersonId!.Value,
                command.AllowGradeEmails!.Value,
                command.AllowAbsenceEmails!.Value,
                command.AllowRemarkEmails!.Value,
                command.AllowMessageEmails!.Value,
                command.AllowGradeNotifications!.Value,
                command.AllowAbsenceNotifications!.Value,
                command.AllowRemarkNotifications!.Value,
                command.AllowMessageNotifications!.Value);

            await this.StudentSettingsAggregateRepository.AddAsync(newSettings, ct);

            studentSettingsId = newSettings.StudentSettingsId;
        }
        else
        {
            studentSettings.Update(
                command.AllowGradeEmails!.Value,
                command.AllowAbsenceEmails!.Value,
                command.AllowRemarkEmails!.Value,
                command.AllowMessageEmails!.Value,
                command.AllowGradeNotifications!.Value,
                command.AllowAbsenceNotifications!.Value,
                command.AllowRemarkNotifications!.Value,
                command.AllowMessageNotifications!.Value);

            studentSettingsId = studentSettings.StudentSettingsId;
        }

        await this.UnitOfWork.SaveAsync(ct);

        return studentSettingsId;
    }
}
