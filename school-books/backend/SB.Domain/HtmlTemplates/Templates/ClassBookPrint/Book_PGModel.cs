namespace SB.Domain;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public record Book_PGModel(
    CoverPageModel CoverPageModel,
    StudentsModel StudentsModel,
    SchedulesModel SchedulesModel,
    ParentMeetingsModel ParentMeetingsModel,
    AttendancesModel AttendancesModel,
    PgResultsModel PgResultsModel,
    ScheduleAndAbsencesModel ScheduleAndAbsencesModel,
    NotesModel NotesModel
);
