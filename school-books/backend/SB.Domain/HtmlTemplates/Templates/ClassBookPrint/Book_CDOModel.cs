namespace SB.Domain;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public record Book_CDOModel(
    CoverPageModel CoverPageModel,
    StudentsModel StudentsModel,
    AbsencesCdoModel AbsencesCdoModel,
    SchoolYearProgramModel SchoolYearProgramModel,
    ScheduleAndAbsencesModel ScheduleAndAbsencesModel,
    NotesModel NotesModel
);
