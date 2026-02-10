namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateAbsencesByClassesReportCommandHandler(
        IUnitOfWork UnitOfWork,
        IAbsencesByClassesReportsAggregateRepository AbsencesByClassesReportsAggregateRepository,
        IAbsencesByClassesReportsQueryRepository AbsencesByClassesReportsQueryRepository,
        IClassBooksQueryRepository ClassBooksQueryRepository)
    : IRequestHandler<CreateAbsencesByClassesReportCommand, int>
{
    public async Task<int> Handle(CreateAbsencesByClassesReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var classBookIds = command.ClassBookIds ?? Array.Empty<int>();

        var items = await this.AbsencesByClassesReportsQueryRepository.GetItemsForAddAsync(
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

        AbsencesByClassesReport absencesByClassesReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Period!.Value,
            string.Join(", ", classBooks),
            items,
            createDate,
            command.SysUserId!.Value);

        await this.AbsencesByClassesReportsAggregateRepository.AddAsync(absencesByClassesReport, ct);

        if (absencesByClassesReport.Items.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { absencesByClassesReport },
                ct);

            await this.UnitOfWork.BulkInsertAsync(
                absencesByClassesReport.Items,
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return absencesByClassesReport.AbsencesByClassesReportId;
    }
}
