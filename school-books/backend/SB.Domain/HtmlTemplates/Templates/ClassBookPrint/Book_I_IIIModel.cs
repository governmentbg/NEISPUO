namespace SB.Domain;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public record Book_I_IIIModel(
    bool HasFinalGrades,
    bool HasFirstGradeResults,
    CoverPageModel CoverPageModel,
    TeachersModel TeachersModel,
    SchedulesModel SchedulesModel,
    ParentMeetingsModel ParentMeetingsModel,
    ExamsModel ClassWorkExamsModel,
    ExamsModel ExamsModel,
    StudentsModel StudentsModel,
    ScheduleAndAbsencesModel ScheduleAndAbsencesModel,
    GradesModel TermOneGradesModel,
    GradesModel TermTwoGradesModel,
    RemarksModel RemarksModel,
    FinalGradesModel FinalGradesModel,
    FirstGradeResultsModel FirstGradeResultsModel,
    AbsencesModel AbsencesModel,
    IndividualWorksModel IndividualWorksModel,
    SanctionsModel SanctionsModel,
    SupportsModel SupportsModel,
    NotesModel NotesModel);
