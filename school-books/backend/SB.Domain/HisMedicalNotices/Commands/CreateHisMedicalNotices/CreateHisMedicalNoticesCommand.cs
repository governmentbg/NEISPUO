namespace SB.Domain;

using MediatR;

public record CreateHisMedicalNoticesCommand : IRequest
{
    public required HisMedicalNoticeDO[] MedicalNotices { get; init; }
}
