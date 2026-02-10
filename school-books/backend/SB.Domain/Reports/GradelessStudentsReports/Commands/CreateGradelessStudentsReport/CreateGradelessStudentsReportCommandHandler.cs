namespace SB.Domain;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record CreateGradelessStudentsReportCommandHandler(
    IUnitOfWork UnitOfWork,
    IGradelessStudentsReportsAggregateRepository GradelessStudentsReportsAggregateRepository,
    IGradelessStudentsReportsQueryRepository GradelessStudentsReportsQueryRepository,
    IClassBooksQueryRepository ClassBooksQueryRepository)
    : IRequestHandler<CreateGradelessStudentsReportCommand, int>
{
    public async Task<int> Handle(CreateGradelessStudentsReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var classBookIds = command.ClassBookIds ?? Array.Empty<int>();

        var items = await this.GradelessStudentsReportsQueryRepository.GetItemsForAddAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.OnlyFinalGrades!.Value,
            command.Period!.Value,
            classBookIds,
            ct);

        var classBooks = Array.Empty<string>();

        if (classBookIds.Any())
        {
            classBooks = await this.ClassBooksQueryRepository.GetClassBookNamesByIdsAsync(command.SchoolYear!.Value, command.InstId!.Value, command.ClassBookIds ?? Array.Empty<int>(), ct);
        }
        

        GradelessStudentsReport gradelessStudentsReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.OnlyFinalGrades!.Value,
            command.Period!.Value,
            string.Join(", ", classBooks),
            items,
            createDate,
            command.SysUserId!.Value);

        await this.GradelessStudentsReportsAggregateRepository.AddAsync(gradelessStudentsReport, ct);

        if (gradelessStudentsReport.Items.Any())
        {
            await this.UnitOfWork.BulkInsertAsync(
                new[] { gradelessStudentsReport },
                ct);

            await this.UnitOfWork.BulkInsertAsync(
                gradelessStudentsReport.Items,
                ct);
        }
        else
        {
            // use a simple save in case of an empty report
            await this.UnitOfWork.SaveAsync(ct);
        }

        return gradelessStudentsReport.GradelessStudentsReportId;
    }
}
