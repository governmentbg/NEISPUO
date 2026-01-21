namespace SB.Domain;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record CreateMissingTopicsReportCommandHandler(
    IUnitOfWork UnitOfWork,
    IMissingTopicsReportsAggregateRepository MissingTopicsReportsAggregateRepository,
    IMissingTopicsReportsQueryRepository MissingTopicsReportsQueryRepository)
    : IRequestHandler<CreateMissingTopicsReportCommand, int>
{
    public async Task<int> Handle(CreateMissingTopicsReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var items = await this.MissingTopicsReportsQueryRepository.GetItemsForAddAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            command.Year,
            command.Month,
            command.TeacherPersonId,
            createDate,
            ct);

        MissingTopicsReport missingTopicsReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            command.Year,
            command.Month,
            command.TeacherPersonId,
            items,
            createDate,
            command.SysUserId!.Value);

        await this.MissingTopicsReportsAggregateRepository.AddAsync(missingTopicsReport, ct);

        if (missingTopicsReport.Items.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { missingTopicsReport },
                ct);
            await this.UnitOfWork.BulkInsertAsync(
                missingTopicsReport.Items,
                ct);
            await this.UnitOfWork.BulkInsertAsync(
                missingTopicsReport.Items.SelectMany(x => x.Teachers),
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return missingTopicsReport.MissingTopicsReportId;
    }
}
