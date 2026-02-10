namespace SB.Domain;

using System;

public record SchedulesModel(
    SchedulesModelSchedule[] Schedules,
    SchedulesModelSchedule[] IFOSchedules
);

public record SchedulesModelSchedule(
    int? StudentPersonId,
    string? StudentName,
    DateTime StartDate,
    DateTime EndDate,
    SchedulesModelScheduleLessons[] Lessons
);

public record SchedulesModelScheduleLessons(
    int Day,
    int HourNumber,
    string SubjectName,
    string SubjectTypeName
);
