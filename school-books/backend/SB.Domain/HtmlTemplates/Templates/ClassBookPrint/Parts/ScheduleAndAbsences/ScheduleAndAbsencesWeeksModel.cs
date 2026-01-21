namespace SB.Domain;

public record ScheduleAndAbsencesWeeksModel(
    bool IsDPLR,
    ScheduleAndAbsencesModelWeek[] Weeks
);
