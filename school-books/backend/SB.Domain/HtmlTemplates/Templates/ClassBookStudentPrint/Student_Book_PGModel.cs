namespace SB.Domain;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public record Student_Book_PGModel(
    StudentCoverPageModel CoverPageModel,
    StudentSchedulesModel SchedulesModel,
    StudentParentMeetingsModel ParentMeetingsModel,
    StudentAttendancesModel AttendancesModel,
    StudentPgResultsModel PgResultsModel,
    StudentNotesModel NotesModel
);
