namespace SB.Domain;

public record StudentAttendancesModel(
    StudentAttendancesModelMonth[] Months
);

public record StudentAttendancesModelMonth(
    string MonthName,
    StudentAttendancesModelDay[] Days,
    StudentAttendancesModelAttendance Attendances
);

public record StudentAttendancesModelDay(
    int DayOfMonth,
    bool IsOffDay,
    bool IsWithoutTotal
);

public record StudentAttendancesModelAttendance(
    AttendanceType?[] Attendances,
    int PresencesTotal
);
