namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateExamsReportCommandHandler(
        IUnitOfWork UnitOfWork,
        IExamsReportsAggregateRepository ExamsReportsAggregateRepository,
        IExamsReportsQueryRepository ExamsReportsQueryRepository)
    : IRequestHandler<CreateExamsReportCommand, int>
{
    public async Task<int> Handle(CreateExamsReportCommand command, CancellationToken ct)
    {
        var createDate = DateTime.Now;

        var items = await this.ExamsReportsQueryRepository.GetItemsForAddAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct);


        var report = new ExamsReport(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            items,
            createDate,
            command.SysUserId!.Value);

        await this.ExamsReportsAggregateRepository.AddAsync(report, ct);

        if (report.Items.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { report },
                ct);
            await this.UnitOfWork.BulkInsertAsync(
                report.Items,
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return report.ExamsReportId;
    }
}
