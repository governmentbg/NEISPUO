namespace SB.Domain;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public record Book_IVModel(
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
    AbsencesModel AbsencesModel,
    IndividualWorksModel IndividualWorksModel,
    SanctionsModel SanctionsModel,
    SupportsModel SupportsModel,
    NotesModel NotesModel);
