namespace SB.Domain;

public record CreateAttendancesCommandAttendance
{
    public int? PersonId { get; init; }
    public AttendanceType? Type { get; init; }
}
