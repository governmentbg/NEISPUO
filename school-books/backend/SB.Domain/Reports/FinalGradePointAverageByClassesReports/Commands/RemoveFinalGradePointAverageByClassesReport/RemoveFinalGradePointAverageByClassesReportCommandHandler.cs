namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveFinalGradePointAverageByClassesReportCommandHandler(
        IFinalGradePointAverageByClassesReportsAggregateRepository FinalGradePointAverageByClassesReportsAggregateRepository)
    : IRequestHandler<RemoveFinalGradePointAverageByClassesReportCommand>
{
    public async Task Handle(RemoveFinalGradePointAverageByClassesReportCommand command, CancellationToken ct)
    {
        await this.FinalGradePointAverageByClassesReportsAggregateRepository.RemoveAsync(
            command.SchoolYear!.Value,
            command.FinalGradePointAverageByClassesReportId!.Value,
            ct);
    }
}
