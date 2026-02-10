ALTER TABLE [school_books].[Absence] DROP CONSTRAINT [FK_Absence_StudentClass];
ALTER TABLE [school_books].[Absence] ADD
    CONSTRAINT [FK_Absence_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_Absence_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[Attendance] DROP CONSTRAINT [FK_Attendance_StudentClass];
ALTER TABLE [school_books].[Attendance] ADD
    CONSTRAINT [FK_Attendance_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_Attendance_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[ClassBookStudentActivity] DROP CONSTRAINT [FK_ClassBookStudentActivity_StudentClass];
ALTER TABLE [school_books].[ClassBookStudentActivity] ADD
    CONSTRAINT [FK_ClassBookStudentActivity_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_ClassBookStudentActivity_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[ClassBookStudentGradeless] DROP CONSTRAINT [FK_ClassBookStudentGradeless_StudentClass];
ALTER TABLE [school_books].[ClassBookStudentGradeless] ADD
    CONSTRAINT [FK_ClassBookStudentGradeless_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_ClassBookStudentGradeless_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[ClassBookStudentSpecialNeeds] DROP CONSTRAINT [FK_ClassBookStudentSpecialNeeds_StudentClass];
ALTER TABLE [school_books].[ClassBookStudentSpecialNeeds] ADD
    CONSTRAINT [FK_ClassBookStudentSpecialNeeds_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_ClassBookStudentSpecialNeeds_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[ExamResultsBookRecord] DROP CONSTRAINT [FK_ExamResultsBookRecord_StudentClass];
ALTER TABLE [school_books].[ExamResultsBookRecord] ADD
    CONSTRAINT [FK_ExamResultsBookRecord_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_ExamResultsBookRecord_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[MainBookRecord] DROP CONSTRAINT [FK_MainBookRecord_StudentClass];
ALTER TABLE [school_books].[MainBookRecord] ADD
    CONSTRAINT [FK_MainBookRecord_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_MainBookRecord_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[SpbsBookRecord] DROP CONSTRAINT [FK_SpbsBookRecord_StudentClass];
ALTER TABLE [school_books].[SpbsBookRecord] ADD
    CONSTRAINT [FK_SpbsBookRecord_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_SpbsBookRecord_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[FirstGradeResult] DROP CONSTRAINT [FK_FirstGradeResult_StudentClass];
ALTER TABLE [school_books].[FirstGradeResult] ADD
    CONSTRAINT [FK_FirstGradeResult_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_FirstGradeResult_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[Grade] DROP CONSTRAINT [FK_Grade_StudentClass];
ALTER TABLE [school_books].[Grade] ADD
    CONSTRAINT [FK_Grade_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_Grade_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[GradeResult] DROP CONSTRAINT [FK_GradeResult_StudentClass];
ALTER TABLE [school_books].[GradeResult] ADD
    CONSTRAINT [FK_GradeResult_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_GradeResult_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[IndividualWork] DROP CONSTRAINT [FK_IndividualWork_StudentClass];
ALTER TABLE [school_books].[IndividualWork] ADD
    CONSTRAINT [FK_IndividualWork_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_IndividualWork_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[Note] DROP CONSTRAINT [FK_Note_StudentClass];
ALTER TABLE [school_books].[Note] ADD
    CONSTRAINT [FK_Note_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_Note_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[PgResult] DROP CONSTRAINT [FK_PgResult_StudentClass];
ALTER TABLE [school_books].[PgResult] ADD
    CONSTRAINT [FK_PgResult_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_PgResult_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[ExamDutyProtocolStudent] DROP CONSTRAINT [FK_ExamDutyProtocolStudent_StudentClass];
ALTER TABLE [school_books].[ExamDutyProtocolStudent] ADD
    CONSTRAINT [FK_ExamDutyProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_ExamDutyProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[ExamResultProtocolStudent] DROP CONSTRAINT [FK_ExamResultProtocolStudent_StudentClass];
ALTER TABLE [school_books].[ExamResultProtocolStudent] ADD
    CONSTRAINT [FK_ExamResultProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_ExamResultProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[GradeChangeExamsAdmProtocolStudent] DROP CONSTRAINT [FK_GradeChangeExamsAdmProtocolStudent_StudentClass];
ALTER TABLE [school_books].[GradeChangeExamsAdmProtocolStudent] ADD
    CONSTRAINT [FK_GradeChangeExamsAdmProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_GradeChangeExamsAdmProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[HighSchoolCertificateProtocolStudent] DROP CONSTRAINT [FK_HighSchoolCertificateProtocolStudent_StudentClass];
ALTER TABLE [school_books].[HighSchoolCertificateProtocolStudent] ADD
    CONSTRAINT [FK_HighSchoolCertificateProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_HighSchoolCertificateProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[NvoExamDutyProtocolStudent] DROP CONSTRAINT [FK_NvoExamDutyProtocolStudent_StudentClass];
ALTER TABLE [school_books].[NvoExamDutyProtocolStudent] ADD
    CONSTRAINT [FK_NvoExamDutyProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_NvoExamDutyProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[QualificationAcquisitionProtocolStudent] DROP CONSTRAINT [FK_QualificationAcquisitionProtocolStudent_StudentClass];
ALTER TABLE [school_books].[QualificationAcquisitionProtocolStudent] ADD
    CONSTRAINT [FK_QualificationAcquisitionProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_QualificationAcquisitionProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[QualificationExamResultProtocolStudent] DROP CONSTRAINT [FK_QualificationExamResultProtocolStudent_StudentClass];
ALTER TABLE [school_books].[QualificationExamResultProtocolStudent] ADD
    CONSTRAINT [FK_QualificationExamResultProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_QualificationExamResultProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[StateExamsAdmProtocolStudent] DROP CONSTRAINT [FK_StateExamsAdmProtocol_StudentClass];
ALTER TABLE [school_books].[StateExamsAdmProtocolStudent] ADD
    CONSTRAINT [FK_StateExamsAdmProtocol_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_StateExamsAdmProtocol_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[Remark] DROP CONSTRAINT [FK_Remark_StudentClass];
ALTER TABLE [school_books].[Remark] ADD
    CONSTRAINT [FK_Remark_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_Remark_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[Sanction] DROP CONSTRAINT [FK_Sanction_StudentClass];
ALTER TABLE [school_books].[Sanction] ADD
    CONSTRAINT [FK_Sanction_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_Sanction_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[Schedule] DROP CONSTRAINT [FK_Schedule_StudentClass];
ALTER TABLE [school_books].[Schedule] ADD
    CONSTRAINT [FK_Schedule_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_Schedule_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);

ALTER TABLE [school_books].[SupportStudent] DROP CONSTRAINT [FK_SupportStudent_PersonId];
ALTER TABLE [school_books].[SupportStudent] DROP CONSTRAINT [FK_SupportStudent_StudentClass];
ALTER TABLE [school_books].[SupportStudent] ADD
    CONSTRAINT [FK_SupportStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_SupportStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]);
