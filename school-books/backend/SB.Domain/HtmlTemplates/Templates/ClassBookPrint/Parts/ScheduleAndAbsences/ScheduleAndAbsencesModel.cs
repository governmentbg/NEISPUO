namespace SB.Domain;

using System;

public record ScheduleAndAbsencesModel(
    bool IsDPLR,
    ScheduleAndAbsencesModelWeek[] Weeks,
    ScheduleAndAbsencesModelWeek[] IFOWeeks
);

public record ScheduleAndAbsencesModelWeek(
    string WeekName,
    string? StudentName,
    string[] AdditionalActivities,
    ScheduleAndAbsencesModelWeekDay[] Days
);

public record ScheduleAndAbsencesModelWeekDay(
    DateTime Date,
    string DayString,
    bool IsOffDay,
    ScheduleAndAbsencesModelWeekDayHour[] Hours
);

public record ScheduleAndAbsencesModelWeekDayHour(
    int HourNumber,
    bool? IsEmptyHour,
    bool? ReplTeacherIsNonSpecialist,
    string? SubjectName,
    string? SubjectTypeName,
    string? CurriculumTeachers,
    string ExcusedStudentClassNumbers,
    string UnexcusedStudentClassNumbers,
    string LateStudentClassNumbers,
    string DplrAbsenceStudentClassNumbers,
    string DplrAttendanceStudentClassNumbers,
    string[] Topics,
    string? Location
);
