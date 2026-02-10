GO

CREATE NONCLUSTERED INDEX [IX_ScheduleHour_CurriculumId] ON [school_books].[ScheduleHour] ([CurriculumId] ASC);
CREATE NONCLUSTERED INDEX [IX_ScheduleLesson_CurriculumId] ON [school_books].[ScheduleLesson] ([CurriculumId] ASC);
CREATE NONCLUSTERED INDEX [IX_Remark_CurriculumId] ON [school_books].[Remark] ([CurriculumId] ASC);
CREATE NONCLUSTERED INDEX [IX_MissingTopicsReportItem_CurriculumId] ON [school_books].[MissingTopicsReportItem] ([CurriculumId] ASC);
CREATE NONCLUSTERED INDEX [IX_GradeResultSubject_CurriculumId] ON [school_books].[GradeResultSubject] ([CurriculumId] ASC);
CREATE NONCLUSTERED INDEX [IX_Grade_CurriculumId] ON [school_books].[Grade] ([CurriculumId] ASC);
CREATE NONCLUSTERED INDEX [IX_Exam_CurriculumId] ON [school_books].[Exam] ([CurriculumId] ASC);
CREATE NONCLUSTERED INDEX [IX_ClassBookStudentGradeless_CurriculumId] ON [school_books].[ClassBookStudentGradeless] ([CurriculumId] ASC);
CREATE NONCLUSTERED INDEX [IX_ClassBookCurriculumGradeless_CurriculumId] ON [school_books].[ClassBookCurriculumGradeless] ([CurriculumId] ASC);
CREATE NONCLUSTERED INDEX [IX_ClassBookStudentSpecialNeeds_CurriculumId] ON [school_books].[ClassBookStudentSpecialNeeds] ([CurriculumId] ASC);
CREATE NONCLUSTERED INDEX [IX_Absence_CurriculumId] ON [school_books].[Absence] ([CurriculumId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Absence_ClassId] ON [school_books].[Absence] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_Attendance_ClassId] ON [school_books].[Attendance] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_ClassBook_ClassId] ON [school_books].[ClassBook] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_ClassBookStudentActivity_ClassId] ON [school_books].[ClassBookStudentActivity] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_ClassBookStudentGradeless_ClassId] ON [school_books].[ClassBookStudentGradeless] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_ClassBookStudentSpecialNeeds_ClassId] ON [school_books].[ClassBookStudentSpecialNeeds] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_ExamDutyProtocolClass_ClassId] ON [school_books].[ExamDutyProtocolClass] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_ExamDutyProtocolStudent_ClassId] ON [school_books].[ExamDutyProtocolStudent] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_ExamResultProtocolClass_ClassId] ON [school_books].[ExamResultProtocolClass] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_ExamResultProtocolStudent_ClassId] ON [school_books].[ExamResultProtocolStudent] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_FirstGradeResult_ClassId] ON [school_books].[FirstGradeResult] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_Grade_ClassId] ON [school_books].[Grade] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_GradeChangeExamsAdmProtocolStudent_ClassId] ON [school_books].[GradeChangeExamsAdmProtocolStudent] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_GradeChangeExamsAdmProtocolStudentSubject_ClassId] ON [school_books].[GradeChangeExamsAdmProtocolStudentSubject] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_GradeResult_ClassId] ON [school_books].[GradeResult] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_HighSchoolCertificateProtocolStudent_ClassId] ON [school_books].[HighSchoolCertificateProtocolStudent] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_IndividualWork_ClassId] ON [school_books].[IndividualWork] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_MissingTopicsReportItem_ClassId] ON [school_books].[MissingTopicsReportItem] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_NoteStudent_ClassId] ON [school_books].[NoteStudent] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_NvoExamDutyProtocolStudent_ClassId] ON [school_books].[NvoExamDutyProtocolStudent] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_PgResult_ClassId] ON [school_books].[PgResult] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_QualificationAcquisitionProtocolStudent_ClassId] ON [school_books].[QualificationAcquisitionProtocolStudent] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_QualificationExamResultProtocolClass_ClassId] ON [school_books].[QualificationExamResultProtocolClass] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_QualificationExamResultProtocolStudent_ClassId] ON [school_books].[QualificationExamResultProtocolStudent] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_Remark_ClassId] ON [school_books].[Remark] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_Sanction_ClassId] ON [school_books].[Sanction] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_Schedule_ClassId] ON [school_books].[Schedule] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_SpbsBookRecord_ClassId] ON [school_books].[SpbsBookRecord] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_StateExamsAdmProtocolStudent_ClassId] ON [school_books].[StateExamsAdmProtocolStudent] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_StateExamsAdmProtocolStudentSubject_ClassId] ON [school_books].[StateExamsAdmProtocolStudentSubject] ([ClassId] ASC);
CREATE NONCLUSTERED INDEX [IX_SupportStudent_ClassId] ON [school_books].[SupportStudent] ([ClassId] ASC);
GO
