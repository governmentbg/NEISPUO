namespace SB.Domain;

using System;

public record StudentSchedulesModel(
    StudentSchedulesModelSchedule[] Schedules
);

public record StudentSchedulesModelSchedule(
    DateTime StartDate,
    DateTime EndDate,
    StudentSchedulesModelScheduleLessons[] Lessons
);

public record StudentSchedulesModelScheduleLessons(
    int Day,
    int HourNumber,
    string SubjectName,
    string SubjectTypeName
);
