namespace SB.Domain;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public record Student_Book_CSOPModel(
    StudentCoverPageModel CoverPageModel,
    StudentTeachersModel TeachersModel,
    StudentSchedulesModel SchedulesModel,
    StudentParentMeetingsModel ParentMeetingsModel,
    StudentGradesModel TermOneGradesModel,
    StudentGradesModel TermTwoGradesModel,
    StudentFinalGradesModel FinalGradesModel,
    StudentFirstGradeResultsModel FirstGradeResultsModel,
    StudentAbsencesCdoModel AbsencesCdoModel,
    StudentSupportsModel SupportsModel,
    StudentAbsencesModel AbsencesModel,
    StudentIndividualWorksModel IndividualWorksModel,
    StudentNotesModel NotesModel
);
