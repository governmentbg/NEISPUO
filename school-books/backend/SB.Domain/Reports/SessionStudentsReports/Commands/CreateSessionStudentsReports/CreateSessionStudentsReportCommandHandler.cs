namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateSessionStudentsReportCommandHandler(
        IUnitOfWork UnitOfWork,
        ISessionStudentsReportsAggregateRepository SessionStudentsReportsAggregateRepository,
        ISessionStudentsReportsQueryRepository SessionStudentsReportsQueryRepository)
    : IRequestHandler<CreateSessionStudentsReportCommand, int>
{
    public async Task<int> Handle(CreateSessionStudentsReportCommand command, CancellationToken ct)
    {
        var createDate = DateTime.Now;

        var items = await this.SessionStudentsReportsQueryRepository.GetItemsForAddAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct);


        var report = new SessionStudentsReport(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            items,
            createDate,
            command.SysUserId!.Value);

        await this.SessionStudentsReportsAggregateRepository.AddAsync(report, ct);

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

        return report.SessionStudentsReportId;
    }
}
