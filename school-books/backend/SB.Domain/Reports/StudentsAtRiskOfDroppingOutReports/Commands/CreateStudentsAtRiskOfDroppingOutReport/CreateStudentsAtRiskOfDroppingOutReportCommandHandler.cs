namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateStudentsAtRiskOfDroppingOutReportCommandHandler(
        IUnitOfWork UnitOfWork,
        IStudentsAtRiskOfDroppingOutReportAggregateRepository StudentsAtRiskOfDroppingOutReportsAggregateRepository,
        IStudentsAtRiskOfDroppingOutReportsQueryRepository StudentsAtRiskOfDroppingOutReportsQueryRepository)
    : IRequestHandler<CreateStudentsAtRiskOfDroppingOutReportCommand, int>
{
    public async Task<int> Handle(CreateStudentsAtRiskOfDroppingOutReportCommand command, CancellationToken ct)
    {
        var createDate = DateTime.Now;

        var items = await this.StudentsAtRiskOfDroppingOutReportsQueryRepository.GetItemsForAddAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ReportDate!.Value,
            ct);


        var report = new StudentsAtRiskOfDroppingOutReport(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ReportDate!.Value,
            items,
            createDate,
            command.SysUserId!.Value);

        await this.StudentsAtRiskOfDroppingOutReportsAggregateRepository.AddAsync(report, ct);

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

        return report.StudentsAtRiskOfDroppingOutReportId;
    }
}
