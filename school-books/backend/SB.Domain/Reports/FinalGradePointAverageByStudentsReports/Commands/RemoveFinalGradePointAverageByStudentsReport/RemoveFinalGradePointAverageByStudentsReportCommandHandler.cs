namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveFinalGradePointAverageByStudentsReportCommandHandler(
        IFinalGradePointAverageByStudentsReportsAggregateRepository FinalGradePointAverageByStudentsReportsAggregateRepository)
    : IRequestHandler<RemoveFinalGradePointAverageByStudentsReportCommand>
{
    public async Task Handle(RemoveFinalGradePointAverageByStudentsReportCommand command, CancellationToken ct)
    {
        await this.FinalGradePointAverageByStudentsReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.FinalGradePointAverageByStudentsReportId!.Value,
            ct);
    }
}
