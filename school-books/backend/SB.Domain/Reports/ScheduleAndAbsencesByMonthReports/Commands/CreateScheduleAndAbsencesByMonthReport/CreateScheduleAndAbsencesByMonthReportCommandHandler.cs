namespace SB.Domain;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record CreateScheduleAndAbsencesByMonthReportCommandHandler(
    IUnitOfWork UnitOfWork,
    IScheduleAndAbsencesByMonthReportsAggregateRepository ScheduleAndAbsencesByMonthReportsAggregateRepository,
    IScheduleAndAbsencesByMonthReportsQueryRepository ScheduleAndAbsencesByMonthReportsQueryRepository,
    IClassBooksQueryRepository ClassBooksQueryRepository)
    : IRequestHandler<CreateScheduleAndAbsencesByMonthReportCommand, int>
{
    public async Task<int> Handle(CreateScheduleAndAbsencesByMonthReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var classBook = await this.ClassBooksQueryRepository.GetAsync(command.SchoolYear!.Value, command.ClassBookId!.Value, ct);

        var weeks = await this.ScheduleAndAbsencesByMonthReportsQueryRepository.GetWeeksForAddAsync(
            command.SchoolYear!.Value,
            command.Year!.Value,
            command.Month!.Value,
            command.ClassBookId!.Value,
            ct);

        ScheduleAndAbsencesByMonthReport scheduleAndAbsencesByMonthReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Year!.Value,
            command.Month!.Value,
            classBook.FullBookName,
            classBook.BookType == ClassBookType.Book_DPLR,
            weeks,
            createDate,
            command.SysUserId!.Value);

        await this.ScheduleAndAbsencesByMonthReportsAggregateRepository.AddAsync(scheduleAndAbsencesByMonthReport, ct);

        if (scheduleAndAbsencesByMonthReport.Weeks.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { scheduleAndAbsencesByMonthReport },
                ct);
            await this.UnitOfWork.BulkInsertAsync(
                scheduleAndAbsencesByMonthReport.Weeks,
                ct);
            await this.UnitOfWork.BulkInsertAsync(
                scheduleAndAbsencesByMonthReport.Weeks.SelectMany(x => x.Days),
                ct);
            await this.UnitOfWork.BulkInsertAsync(
                scheduleAndAbsencesByMonthReport.Weeks.SelectMany(w => w.Days.SelectMany(x => x.Hours)),
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return scheduleAndAbsencesByMonthReport.ScheduleAndAbsencesByMonthReportId;
    }
}
