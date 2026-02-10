namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public abstract class HtmlTemplateConfig
{
    public static readonly HtmlTemplateConfig<Book_I_IIIModel> ClassBookPrint_Book_I_III = new(
        "Book_I_III",
        "ClassBookPrint.Book_I_III",
        new Book_I_IIIModel(
            false,
            true,
            CoverPageModelSample.SampleRegular,
            TeachersModelSample.Sample,
            SchedulesModelSample.Sample,
            ParentMeetingsModelSample.Sample,
            ExamsModelSample.ClassworkSample,
            ExamsModelSample.Sample,
            StudentsModelSample.Sample,
            ScheduleAndAbsencesModelSample.Sample,
            GradesModelSample.TermOneSample,
            GradesModelSample.TermTwoSample,
            RemarksModelSample.Sample,
            FinalGradesModelSample.Sample,
            FirstGradeResultsModelSample.Sample,
            AbsencesModelSample.Sample,
            IndividualWorksModelSample.Sample,
            SanctionsModelSample.Sample,
            SupportsModelSample.Sample,
            NotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Book_IVModel> ClassBookPrint_Book_IV = new(
        "Book_IV",
        "ClassBookPrint.Book_IV",
        new Book_IVModel(
            CoverPageModelSample.SampleRegular,
            TeachersModelSample.Sample,
            SchedulesModelSample.Sample,
            ParentMeetingsModelSample.Sample,
            ExamsModelSample.ClassworkSample,
            ExamsModelSample.Sample,
            StudentsModelSample.Sample,
            ScheduleAndAbsencesModelSample.Sample,
            GradesModelSample.TermOneSample,
            GradesModelSample.TermTwoSample,
            RemarksModelSample.Sample,
            FinalGradesModelSample.Sample,
            AbsencesModelSample.Sample,
            IndividualWorksModelSample.Sample,
            SanctionsModelSample.Sample,
            SupportsModelSample.Sample,
            NotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Book_V_XIIModel> ClassBookPrint_Book_V_XII = new(
        "Book_V_XII",
        "ClassBookPrint.Book_V_XII",
        new Book_V_XIIModel(
            CoverPageModelSample.SampleRegularWithSpeciality,
            TeachersModelSample.Sample,
            SchedulesModelSample.Sample,
            ParentMeetingsModelSample.Sample,
            ExamsModelSample.ClassworkSample,
            ExamsModelSample.Sample,
            StudentsModelSample.Sample,
            ScheduleAndAbsencesModelSample.Sample,
            GradesModelSample.TermOneSample,
            GradesModelSample.TermTwoSample,
            RemarksModelSample.Sample,
            FinalGradesModelSample.Sample,
            AbsencesModelSample.Sample,
            IndividualWorksModelSample.Sample,
            GradeResultsModelSample.Sample,
            GradeResultSessionsModelSample.Sample,
            SanctionsModelSample.Sample,
            SupportsModelSample.Sample,
            NotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Book_PGModel> ClassBookPrint_Book_PG = new(
        "Book_PG",
        "ClassBookPrint.Book_PG",
        new Book_PGModel(
            CoverPageModelSample.SamplePG,
            StudentsModelSample.Sample,
            SchedulesModelSample.Sample,
            ParentMeetingsModelSample.Sample,
            AttendancesModelSample.Sample,
            PgResultsModelSample.Sample,
            ScheduleAndAbsencesModelSample.Sample,
            NotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Book_CDOModel> ClassBookPrint_Book_CDO = new(
        "Book_CDO",
        "ClassBookPrint.Book_CDO",
        new Book_CDOModel(
            CoverPageModelSample.SampleCDO,
            StudentsModelSample.Sample,
            AbsencesCdoModelSample.Sample,
            SchoolYearProgramModelSample.Sample,
            ScheduleAndAbsencesModelSample.Sample,
            NotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Book_CSOPModel> ClassBookPrint_Book_CSOP = new(
        "Book_CSOP",
        "ClassBookPrint.Book_CSOP",
        new Book_CSOPModel(
            false,
            true,
            CoverPageModelSample.SampleRegular,
            TeachersModelSample.Sample,
            SchedulesModelSample.Sample,
            ParentMeetingsModelSample.Sample,
            StudentsModelSample.Sample,
            AbsencesCdoModelSample.Sample,
            ScheduleAndAbsencesModelSample.Sample,
            GradesModelSample.TermOneSample,
            GradesModelSample.TermTwoSample,
            FinalGradesModelSample.Sample,
            FirstGradeResultsModelSample.Sample,
            SupportsModelSample.Sample,
            AbsencesModelSample.Sample,
            IndividualWorksModelSample.Sample,
            NotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Book_DPLRModel> ClassBookPrint_Book_DPLR = new(
        "Book_DPLR",
        "ClassBookPrint.Book_DPLR",
        new Book_DPLRModel(
            CoverPageModelSample.SampleDPLR,
            StudentsModelSample.Sample,
            ScheduleAndAbsencesModelSample.Sample,
            PerformancesModelSample.Sample,
            ReplrParticipationsModelSample.Sample,
            NotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Student_Book_I_IIIModel> ClassBookStudentPrint_Book_I_III = new(
        "Student_Book_I_III",
        "ClassBookStudentPrint.Student_Book_I_III",
        new Student_Book_I_IIIModel(
            true,
            StudentCoverPageModelSample.SampleRegular,
            StudentTeachersModelSample.Sample,
            StudentSchedulesModelSample.Sample,
            StudentParentMeetingsModelSample.Sample,
            StudentExamsModelSample.ClassworkSample,
            StudentExamsModelSample.Sample,
            StudentGradesModelSample.TermOneSample,
            StudentRemarksModelSample.TermOneSample,
            StudentGradesModelSample.TermTwoSample,
            StudentRemarksModelSample.TermTwoSample,
            StudentFinalGradesModelSample.Sample,
            StudentFirstGradeResultsModelSample.Sample,
            StudentAbsencesModelSample.Sample,
            StudentIndividualWorksModelSample.Sample,
            StudentSanctionsModelSample.Sample,
            StudentSupportsModelSample.Sample,
            StudentNotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Student_Book_IVModel> ClassBookStudentPrint_Book_IV = new(
        "Student_Book_IV",
        "ClassBookStudentPrint.Student_Book_IV",
        new Student_Book_IVModel(
            StudentCoverPageModelSample.SampleRegular,
            StudentTeachersModelSample.Sample,
            StudentSchedulesModelSample.Sample,
            StudentParentMeetingsModelSample.Sample,
            StudentExamsModelSample.ClassworkSample,
            StudentExamsModelSample.Sample,
            StudentGradesModelSample.TermOneSample,
            StudentRemarksModelSample.TermOneSample,
            StudentGradesModelSample.TermTwoSample,
            StudentRemarksModelSample.TermTwoSample,
            StudentFinalGradesModelSample.Sample,
            StudentAbsencesModelSample.Sample,
            StudentIndividualWorksModelSample.Sample,
            StudentSanctionsModelSample.Sample,
            StudentSupportsModelSample.Sample,
            StudentNotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Student_Book_V_XIIModel> ClassBookStudentPrint_Book_V_XII = new(
        "Student_Book_V_XII",
        "ClassBookStudentPrint.Student_Book_V_XII",
        new Student_Book_V_XIIModel(
            StudentCoverPageModelSample.SampleRegularWithSpeciality,
            StudentTeachersModelSample.Sample,
            StudentSchedulesModelSample.Sample,
            StudentParentMeetingsModelSample.Sample,
            StudentExamsModelSample.ClassworkSample,
            StudentExamsModelSample.Sample,
            StudentGradesModelSample.TermOneSample,
            StudentRemarksModelSample.TermOneSample,
            StudentGradesModelSample.TermTwoSample,
            StudentRemarksModelSample.TermTwoSample,
            StudentFinalGradesModelSample.Sample,
            StudentAbsencesModelSample.Sample,
            StudentIndividualWorksModelSample.Sample,
            StudentGradeResultsModelSample.Sample,
            StudentGradeResultSessionsModelSample.Sample,
            StudentSanctionsModelSample.Sample,
            StudentSupportsModelSample.Sample,
            StudentNotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Student_Book_PGModel> ClassBookStudentPrint_Book_PG = new(
        "Student_Book_PG",
        "ClassBookStudentPrint.Student_Book_PG",
        new Student_Book_PGModel(
            StudentCoverPageModelSample.SamplePG,
            StudentSchedulesModelSample.Sample,
            StudentParentMeetingsModelSample.Sample,
            StudentAttendancesModelSample.Sample,
            StudentPgResultsModelSample.Sample,
            StudentNotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Student_Book_CDOModel> ClassBookStudentPrint_Book_CDO = new(
        "Student_Book_CDO",
        "ClassBookStudentPrint.Student_Book_CDO",
        new Student_Book_CDOModel(
            StudentCoverPageModelSample.SampleCDO,
            StudentAbsencesCdoModelSample.Sample,
            StudentNotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Student_Book_CSOPModel> ClassBookStudentPrint_Book_CSOP = new(
        "Student_Book_CSOP",
        "ClassBookStudentPrint.Student_Book_CSOP",
        new Student_Book_CSOPModel(
            StudentCoverPageModelSample.SampleRegular,
            StudentTeachersModelSample.Sample,
            StudentSchedulesModelSample.Sample,
            StudentParentMeetingsModelSample.Sample,
            StudentGradesModelSample.TermOneSample,
            StudentGradesModelSample.TermTwoSample,
            StudentFinalGradesModelSample.Sample,
            StudentFirstGradeResultsModelSample.Sample,
            StudentAbsencesCdoModelSample.Sample,
            StudentSupportsModelSample.Sample,
            StudentAbsencesModelSample.Sample,
            StudentIndividualWorksModelSample.Sample,
            StudentNotesModelSample.Sample));

    public static readonly HtmlTemplateConfig<Student_Book_DPLRModel> ClassBookStudentPrint_Book_DPLR = new(
        "Student_Book_DPLR",
        "ClassBookStudentPrint.Student_Book_DPLR",
        new Student_Book_DPLRModel(
            StudentCoverPageModelSample.SampleDPLR,
            StudentNotesModelSample.Sample));

    public static readonly IReadOnlyDictionary<string, HtmlTemplateConfig> AllTemplates =
        new ReadOnlyDictionary<string, HtmlTemplateConfig>(
            new Dictionary<string, HtmlTemplateConfig>(StringComparer.InvariantCultureIgnoreCase)
            {
                { ClassBookPrint_Book_I_III.TemplateName, ClassBookPrint_Book_I_III },
                { ClassBookPrint_Book_IV.TemplateName, ClassBookPrint_Book_IV },
                { ClassBookPrint_Book_V_XII.TemplateName, ClassBookPrint_Book_V_XII },
                { ClassBookPrint_Book_PG.TemplateName, ClassBookPrint_Book_PG },
                { ClassBookPrint_Book_CDO.TemplateName, ClassBookPrint_Book_CDO },
                { ClassBookPrint_Book_CSOP.TemplateName, ClassBookPrint_Book_CSOP },
                { ClassBookPrint_Book_DPLR.TemplateName, ClassBookPrint_Book_DPLR },

                { ClassBookStudentPrint_Book_I_III.TemplateName, ClassBookStudentPrint_Book_I_III },
                { ClassBookStudentPrint_Book_IV.TemplateName, ClassBookStudentPrint_Book_IV },
                { ClassBookStudentPrint_Book_V_XII.TemplateName, ClassBookStudentPrint_Book_V_XII },
                { ClassBookStudentPrint_Book_PG.TemplateName, ClassBookStudentPrint_Book_PG },
                { ClassBookStudentPrint_Book_CDO.TemplateName, ClassBookStudentPrint_Book_CDO },
                { ClassBookStudentPrint_Book_CSOP.TemplateName, ClassBookStudentPrint_Book_CSOP },
                { ClassBookStudentPrint_Book_DPLR.TemplateName, ClassBookStudentPrint_Book_DPLR },
            });

    protected HtmlTemplateConfig(
        string templateName,
        string templateFileName,
        Type modelType,
        object sampleModel)
    {
        this.TemplateName = templateName;
        this.TemplateFileName = templateFileName;
        this.ModelType = modelType;
        this.SampleModel = sampleModel;
    }

    public string TemplateName { get; private set; }

    public string TemplateFileName { get; private set; }

    public Type ModelType { get; private set; }

    public object SampleModel { get; private set; }

    public static HtmlTemplateConfig Get(string name)
    {
        if (!AllTemplates.ContainsKey(name))
        {
            throw new Exception($"Could not find template with name {name}");
        }

        return AllTemplates[name];
    }
}

public class HtmlTemplateConfig<T> : HtmlTemplateConfig where T : notnull
{
    public HtmlTemplateConfig(
        string templateName,
        string templateFileName,
        T sampleModel)
        : base(
            templateName,
            templateFileName,
            typeof(T),
            sampleModel)
    {
    }
}
