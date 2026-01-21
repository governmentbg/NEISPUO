namespace SB.Domain;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public record Book_DPLRModel(
    CoverPageModel CoverPageModel,
    StudentsModel StudentsModel,
    ScheduleAndAbsencesModel ScheduleAndAbsencesModel,
    PerformancesModel PerformancesModel,
    ReplrParticipationsModel ReplrParticipationsModel,
    NotesModel NotesModel
);
