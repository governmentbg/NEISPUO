namespace SB.Domain;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record CreateAbsencesByStudentsReportCommandHandler(
    IUnitOfWork UnitOfWork,
    IAbsencesByStudentsReportsAggregateRepository AbsencesByStudentsReportsAggregateRepository,
    IAbsencesByStudentsReportsQueryRepository AbsencesByStudentsReportsQueryRepository,
    IClassBooksQueryRepository ClassBooksQueryRepository)
    : IRequestHandler<CreateAbsencesByStudentsReportCommand, int>
{
    public async Task<int> Handle(CreateAbsencesByStudentsReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var classBookIds = command.ClassBookIds ?? Array.Empty<int>();

        var items = await this.AbsencesByStudentsReportsQueryRepository.GetItemsForAddAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            classBookIds,
            createDate,
            ct);

        var classBooks = Array.Empty<string>();

        if (classBookIds.Any())
        {
            classBooks = await this.ClassBooksQueryRepository.GetClassBookNamesByIdsAsync(command.SchoolYear!.Value, command.InstId!.Value, classBookIds, ct);
        }

        AbsencesByStudentsReport absencesByStudentsReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            string.Join(", ", classBooks),
            items,
            createDate,
            command.SysUserId!.Value);

        await this.AbsencesByStudentsReportsAggregateRepository.AddAsync(absencesByStudentsReport, ct);

        if (absencesByStudentsReport.Items.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { absencesByStudentsReport },
                ct);

            await this.UnitOfWork.BulkInsertAsync(
                absencesByStudentsReport.Items,
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return absencesByStudentsReport.AbsencesByStudentsReportId;
    }
}
