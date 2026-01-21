namespace SB.Domain;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public record Student_Book_I_IIIModel(
    bool HasFirstGradeResults,
    StudentCoverPageModel CoverPageModel,
    StudentTeachersModel TeachersModel,
    StudentSchedulesModel SchedulesModel,
    StudentParentMeetingsModel ParentMeetingsModel,
    StudentExamsModel ClassWorkExamsModel,
    StudentExamsModel ExamsModel,
    StudentGradesModel TermOneGradesModel,
    StudentRemarksModel TermOneRemarksModel,
    StudentGradesModel TermTwoGradesModel,
    StudentRemarksModel TermTwoRemarksModel,
    StudentFinalGradesModel FinalGradesModel,
    StudentFirstGradeResultsModel FirstGradeResultsModel,
    StudentAbsencesModel AbsencesModel,
    StudentIndividualWorksModel IndividualWorksModel,
    StudentSanctionsModel SanctionsModel,
    StudentSupportsModel SupportsModel,
    StudentNotesModel NotesModel);
