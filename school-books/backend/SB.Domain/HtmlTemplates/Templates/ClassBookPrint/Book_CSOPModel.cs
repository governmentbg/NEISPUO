namespace SB.Domain;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public record Book_CSOPModel(
    bool HasFinalGrades,
    bool HasFirstGradeResults,
    CoverPageModel CoverPageModel,
    TeachersModel TeachersModel,
    SchedulesModel SchedulesModel,
    ParentMeetingsModel ParentMeetingsModel,
    StudentsModel StudentsModel,
    AbsencesCdoModel AbsencesCdoModel,
    ScheduleAndAbsencesModel ScheduleAndAbsencesModel,
    GradesModel TermOneGradesModel,
    GradesModel TermTwoGradesModel,
    FinalGradesModel FinalGradesModel,
    FirstGradeResultsModel FirstGradeResultsModel,
    SupportsModel SupportsModel,
    AbsencesModel AbsencesModel,
    IndividualWorksModel IndividualWorksModel,
    NotesModel NotesModel
);
