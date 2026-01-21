namespace SB.Domain;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record CreateScheduleAndAbsencesByTermReportCommandHandler(
    IUnitOfWork UnitOfWork,
    IScheduleAndAbsencesByTermReportsAggregateRepository ScheduleAndAbsencesByTermReportsAggregateRepository,
    IScheduleAndAbsencesByTermReportsQueryRepository ScheduleAndAbsencesByTermReportsQueryRepository,
    IClassBooksQueryRepository ClassBooksQueryRepository)
    : IRequestHandler<CreateScheduleAndAbsencesByTermReportCommand, int>
{
    public async Task<int> Handle(CreateScheduleAndAbsencesByTermReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var classBook = await this.ClassBooksQueryRepository.GetAsync(command.SchoolYear!.Value, command.ClassBookId!.Value, ct);

        var weeks = await this.ScheduleAndAbsencesByTermReportsQueryRepository.GetWeeksForAddAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Term!.Value,
            command.ClassBookId!.Value,
            ct);

        ScheduleAndAbsencesByTermReport scheduleAndAbsencesByTermReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Term!.Value,
            classBook.FullBookName,
            classBook.BookType == ClassBookType.Book_DPLR,
            weeks,
            createDate,
            command.SysUserId!.Value);

        await this.ScheduleAndAbsencesByTermReportsAggregateRepository.AddAsync(scheduleAndAbsencesByTermReport, ct);

        if (scheduleAndAbsencesByTermReport.Weeks.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { scheduleAndAbsencesByTermReport },
                ct);
            await this.UnitOfWork.BulkInsertAsync(
                scheduleAndAbsencesByTermReport.Weeks,
                ct);
            await this.UnitOfWork.BulkInsertAsync(
                scheduleAndAbsencesByTermReport.Weeks.SelectMany(x => x.Days),
                ct);
            await this.UnitOfWork.BulkInsertAsync(
                scheduleAndAbsencesByTermReport.Weeks.SelectMany(w => w.Days.SelectMany(x => x.Hours)),
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return scheduleAndAbsencesByTermReport.ScheduleAndAbsencesByTermReportId;
    }
}
