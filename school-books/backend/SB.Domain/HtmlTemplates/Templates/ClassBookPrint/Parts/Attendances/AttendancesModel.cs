namespace SB.Domain;

public record AttendancesModel(
    AttendancesModelMonth[] Months
);

public record AttendancesModelMonth(
    string MonthName,
    AttendancesModelDay[] Days,
    AttendancesModelStudent[] Students,
    int[] PresencesDayTotals,
    int PresencesCountTotal,
    double AveragePresencesCount
);

public record AttendancesModelDay(
    int DayOfMonth,
    bool IsOffDay,
    bool IsWithoutTotal
);

public record AttendancesModelStudent(
    int? ClassNumber,
    string FullName,
    bool IsTransferred,
    AttendanceType?[] Attendances,
    int PresencesTotal
);
