namespace SB.Domain;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record CreateLectureSchedulesReportCommandHandler(
    IUnitOfWork UnitOfWork,
    ILectureSchedulesReportsAggregateRepository LectureSchedulesReportsAggregateRepository,
    ILectureSchedulesReportsQueryRepository LectureSchedulesReportsQueryRepository)
    : IRequestHandler<CreateLectureSchedulesReportCommand, int>
{
    public async Task<int> Handle(CreateLectureSchedulesReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var items = await this.LectureSchedulesReportsQueryRepository.GetItemsForAddAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            command.Year,
            command.Month,
            command.TeacherPersonId,
            createDate,
            ct);

        string? teacherPersonName = null;
        if (command.TeacherPersonId.HasValue)
        {
            teacherPersonName =
                await this.LectureSchedulesReportsQueryRepository.GetTeacherNameAsync(
                    command.TeacherPersonId.Value,
                    ct);
        }

        var lectureSchedulesReport = new LectureSchedulesReport(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            command.Year,
            command.Month,
            command.TeacherPersonId,
            teacherPersonName,
            items,
            createDate,
            command.SysUserId!.Value);

        await this.LectureSchedulesReportsAggregateRepository.AddAsync(lectureSchedulesReport, ct);

        if (lectureSchedulesReport.Items.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { lectureSchedulesReport },
                ct);
            await this.UnitOfWork.BulkInsertAsync(
                lectureSchedulesReport.Items,
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }
        return lectureSchedulesReport.LectureSchedulesReportId;
    }
}
