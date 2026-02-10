namespace SB.Domain;

using MediatR;

public record АcknowledMedicalNoticesCommand : IRequest
{
    public int ExtSystemId { get; init; }
    public required int[] HisMedicalNoticeIds { get; init; }
}
