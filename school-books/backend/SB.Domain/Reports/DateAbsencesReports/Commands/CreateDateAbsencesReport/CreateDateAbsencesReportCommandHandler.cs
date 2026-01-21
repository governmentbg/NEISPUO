namespace SB.Domain;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record CreateDateAbsencesReportCommandHandler(
    IUnitOfWork UnitOfWork,
    IDateAbsencesReportsAggregateRepository DateAbsencesReportsAggregateRepository,
    IDateAbsencesReportsQueryRepository DateAbsencesReportsQueryRepository,
    IClassBooksQueryRepository ClassBooksQueryRepository,
    IShiftsQueryRepository ShiftsQueryRepository)
    : IRequestHandler<CreateDateAbsencesReportCommand, int>
{
    public async Task<int> Handle(CreateDateAbsencesReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var classBookIds = command.ClassBookIds ?? Array.Empty<int>();
        var shiftIds = command.ShiftIds ?? Array.Empty<int>();

        var items = await this.DateAbsencesReportsQueryRepository.GetItemsForAddAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ReportDate!.Value,
            command.IsUnited!.Value,
            classBookIds,
            shiftIds,
            ct);

        var classBooks = Array.Empty<string>();
        var shifts = Array.Empty<string>();

        if (classBookIds.Any())
        {
            classBooks = await this.ClassBooksQueryRepository.GetClassBookNamesByIdsAsync(command.SchoolYear!.Value, command.InstId!.Value, classBookIds, ct);
        }

        if (shiftIds.Any())
        {
            shifts = await this.ShiftsQueryRepository.GetShiftNamesByIdsAsync(command.SchoolYear!.Value, command.InstId!.Value, shiftIds, ct);
        }

        DateAbsencesReport dateAbsencesReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ReportDate!.Value,
            command.IsUnited!.Value,
            string.Join(", ", classBooks),
            string.Join(", ", shifts),
            items,
            createDate,
            command.SysUserId!.Value);

        await this.DateAbsencesReportsAggregateRepository.AddAsync(dateAbsencesReport, ct);

        if (dateAbsencesReport.Items.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { dateAbsencesReport },
                ct);

            await this.UnitOfWork.BulkInsertAsync(
                dateAbsencesReport.Items,
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return dateAbsencesReport.DateAbsencesReportId;
    }
}
