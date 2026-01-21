USE [$(dbName)]
GO

---------------------------------------------------------------
--Stored Procedures
---------------------------------------------------------------

:r "./CreateSchoolBooks/StoredProcedures/spCreateIdSequence.sql"
:r "./CreateSchoolBooks/StoredProcedures/spDropSchemaObjects.sql"
:r "./CreateSchoolBooks/StoredProcedures/BookSyncProcedures.sql"
:r "./CreateSchoolBooks/StoredProcedures/spDesc.sql"

---------------------------------------------------------------
--Tables
---------------------------------------------------------------

:r "./CreateSchoolBooks/Tables/ClassBookExtProvider/ClassBookExtProvider.sql"

:r "./CreateSchoolBooks/Tables/HisMedicalNoticeBatch/HisMedicalNoticeBatch.sql"
:r "./CreateSchoolBooks/Tables/HisMedicalNotice/HisMedicalNotice.sql"
:r "./CreateSchoolBooks/Tables/HisMedicalNotice/HisMedicalNoticeSchoolYear.sql"
:r "./CreateSchoolBooks/Tables/HisMedicalNoticeReadReceipt/HisMedicalNoticeReadReceipt.sql"
:r "./CreateSchoolBooks/Tables/HisMedicalNoticeReadReceipt/HisMedicalNoticeReadReceiptAccess.sql"
:r "./CreateSchoolBooks/Tables/PersonMedicalNotice/PersonMedicalNotice.sql"

:r "./CreateSchoolBooks/Tables/OffDay/OffDay.sql"
:r "./CreateSchoolBooks/Tables/OffDay/OffDayClass.sql"
:r "./CreateSchoolBooks/Tables/SchoolYearSettings/SchoolYearSettings.sql"
:r "./CreateSchoolBooks/Tables/SchoolYearSettings/SchoolYearSettingsClass.sql"
:r "./CreateSchoolBooks/Tables/SchoolYearSettings/SchoolYearSettingsDefault.sql"

:r "./CreateSchoolBooks/Tables/Shift/Shift.sql"
:r "./CreateSchoolBooks/Tables/Shift/ShiftHour.sql"

:r "./CreateSchoolBooks/Tables/ClassBook/ClassBook.sql"
:r "./CreateSchoolBooks/Tables/ClassBook/ClassBookStudentGradeless.sql"
:r "./CreateSchoolBooks/Tables/ClassBook/ClassBookStudentSpecialNeeds.sql"
:r "./CreateSchoolBooks/Tables/ClassBook/ClassBookStudentFirstGradeResultSpecialNeeds.sql"
:r "./CreateSchoolBooks/Tables/ClassBook/ClassBookCurriculumGradeless.sql"
:r "./CreateSchoolBooks/Tables/ClassBook/ClassBookStudentActivity.sql"
:r "./CreateSchoolBooks/Tables/ClassBook/ClassBookStudentCarriedAbsence.sql"
:r "./CreateSchoolBooks/Tables/ClassBook/ClassBookPrint.sql"
:r "./CreateSchoolBooks/Tables/ClassBook/ClassBookStudentPrint.sql"
:r "./CreateSchoolBooks/Tables/ClassBook/ClassBookPrintSignature.sql"
:r "./CreateSchoolBooks/Tables/ClassBook/ClassBookStatusChange.sql"

:r "./CreateSchoolBooks/Tables/OffDay/OffDayClassBook.sql"
:r "./CreateSchoolBooks/Tables/SchoolYearSettings/SchoolYearSettingsClassBook.sql"
:r  "./CreateSchoolBooks/Tables/ClassBookOffDayDate/ClassBookOffDayDate.sql"
:r  "./CreateSchoolBooks/Tables/ClassBookSchoolYearSettings/ClassBookSchoolYearSettings.sql"

:r "./CreateSchoolBooks/Tables/Schedule/Schedule.sql"
:r "./CreateSchoolBooks/Tables/Schedule/ScheduleDate.sql"
:r "./CreateSchoolBooks/Tables/Schedule/ScheduleHour.sql"
:r "./CreateSchoolBooks/Tables/Schedule/ScheduleLesson.sql"

:r "./CreateSchoolBooks/Tables/TeacherAbsence/TeacherAbsence.sql"
:r "./CreateSchoolBooks/Tables/TeacherAbsence/TeacherAbsenceHour.sql"

:r "./CreateSchoolBooks/Tables/ClassBookTopicPlanItem/ClassBookTopicPlanItem.sql"
:r "./CreateSchoolBooks/Tables/TopicPlan/TopicPlanPublisher.sql"
:r "./CreateSchoolBooks/Tables/TopicPlan/TopicPlan.sql"
:r "./CreateSchoolBooks/Tables/TopicPlan/TopicPlanItem.sql"

:r "./CreateSchoolBooks/Tables/LectureSchedule/LectureSchedule.sql"
:r "./CreateSchoolBooks/Tables/LectureSchedule/LectureScheduleHour.sql"

:r "./CreateSchoolBooks/Tables/Absence/AbsenceReason.sql"
:r "./CreateSchoolBooks/Tables/Absence/Absence.sql"
:r "./CreateSchoolBooks/Tables/Attendance/Attendance.sql"
:r "./CreateSchoolBooks/Tables/Exam/Exam.sql"
:r "./CreateSchoolBooks/Tables/FirstGradeResult/FirstGradeResult.sql"
:r "./CreateSchoolBooks/Tables/Grade/Grade.sql"
:r "./CreateSchoolBooks/Tables/GradeResult/GradeResult.sql"
:r "./CreateSchoolBooks/Tables/GradeResult/GradeResultSubject.sql"
:r "./CreateSchoolBooks/Tables/IndividualWork/IndividualWork.sql"
:r "./CreateSchoolBooks/Tables/Note/Note.sql"
:r "./CreateSchoolBooks/Tables/Note/NoteStudent.sql"
:r "./CreateSchoolBooks/Tables/ParentMeeting/ParentMeeting.sql"
:r "./CreateSchoolBooks/Tables/PgResult/PgResult.sql"
:r "./CreateSchoolBooks/Tables/Remark/Remark.sql"
:r "./CreateSchoolBooks/Tables/Sanction/Sanction.sql"
:r "./CreateSchoolBooks/Tables/Topic/Topic.sql"
:r "./CreateSchoolBooks/Tables/Topic/TopicTitle.sql"
:r "./CreateSchoolBooks/Tables/Topic/TopicTeacher.sql"
:r "./CreateSchoolBooks/Tables/TopicDplr/TopicDplr.sql"
:r "./CreateSchoolBooks/Tables/TopicDplr/TopicDplrTeacher.sql"
:r "./CreateSchoolBooks/Tables/TopicDplr/TopicDplrStudent.sql"
:r "./CreateSchoolBooks/Tables/AdditionalActivity/AdditionalActivity.sql"

:r "./CreateSchoolBooks/Tables/Support/Support.sql"
:r "./CreateSchoolBooks/Tables/Support/SupportActivityType.sql"
:r "./CreateSchoolBooks/Tables/Support/SupportActivity.sql"
:r "./CreateSchoolBooks/Tables/Support/SupportDifficultyType.sql"
:r "./CreateSchoolBooks/Tables/Support/SupportDifficulty.sql"
:r "./CreateSchoolBooks/Tables/Support/SupportTeacher.sql"
:r "./CreateSchoolBooks/Tables/Support/SupportStudent.sql"

:r "./CreateSchoolBooks/Tables/Performance/PerformanceType.sql"
:r "./CreateSchoolBooks/Tables/Performance/Performance.sql"

:r "./CreateSchoolBooks/Tables/ReplrParticipation/ReplrParticipationType.sql"
:r "./CreateSchoolBooks/Tables/ReplrParticipation/ReplrParticipation.sql"

:r "./CreateSchoolBooks/Tables/Documents/SpbsBookRecord/SpbsBookRecord.sql"
:r "./CreateSchoolBooks/Tables/Documents/SpbsBookRecord/SpbsBookRecordMovement.sql"
:r "./CreateSchoolBooks/Tables/Documents/SpbsBookRecord/SpbsBookRecordEscape.sql"
:r "./CreateSchoolBooks/Tables/Documents/SpbsBookRecord/SpbsBookRecordAbsence.sql"

:r "./CreateSchoolBooks/Tables/Conversations/Conversation.sql"
:r "./CreateSchoolBooks/Tables/Conversations/ConversationParticipantGroup.sql"
:r "./CreateSchoolBooks/Tables/Conversations/ConversationParticipant.sql"
:r "./CreateSchoolBooks/Tables/Conversations/ConversationMessage.sql"

:r "./CreateSchoolBooks/Tables/Protocols/ExamDutyProtocol/ProtocolExamType.sql"
:r "./CreateSchoolBooks/Tables/Protocols/ExamDutyProtocol/ProtocolExamSubType.sql"
:r "./CreateSchoolBooks/Tables/Protocols/ExamDutyProtocol/ExamDutyProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/ExamDutyProtocol/ExamDutyProtocolClass.sql"
:r "./CreateSchoolBooks/Tables/Protocols/ExamDutyProtocol/ExamDutyProtocolStudent.sql"
:r "./CreateSchoolBooks/Tables/Protocols/ExamDutyProtocol/ExamDutyProtocolSupervisor.sql"
:r "./CreateSchoolBooks/Tables/Protocols/StateExamDutyProtocol/StateExamDutyProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/StateExamDutyProtocol/StateExamDutyProtocolSupervisor.sql"

:r "./CreateSchoolBooks/Tables/Protocols/GradeChangeExamsAdmProtocol/GradeChangeExamsAdmProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/GradeChangeExamsAdmProtocol/GradeChangeExamsAdmProtocolCommissioner.sql"
:r "./CreateSchoolBooks/Tables/Protocols/GradeChangeExamsAdmProtocol/GradeChangeExamsAdmProtocolStudent.sql"
:r "./CreateSchoolBooks/Tables/Protocols/GradeChangeExamsAdmProtocol/GradeChangeExamsAdmProtocolStudentSubject.sql"

:r "./CreateSchoolBooks/Tables/Protocols/StateExamsAdmProtocol/StateExamsAdmProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/StateExamsAdmProtocol/StateExamsAdmProtocolCommissioner.sql"
:r "./CreateSchoolBooks/Tables/Protocols/StateExamsAdmProtocol/StateExamsAdmProtocolStudent.sql"
:r "./CreateSchoolBooks/Tables/Protocols/StateExamsAdmProtocol/StateExamsAdmProtocolStudentSubject.sql"

:r "./CreateSchoolBooks/Tables/Protocols/HighSchoolCertificateProtocol/HighSchoolCertificateProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/HighSchoolCertificateProtocol/HighSchoolCertificateProtocolStudent.sql"
:r "./CreateSchoolBooks/Tables/Protocols/HighSchoolCertificateProtocol/HighSchoolCertificateProtocolCommissioner.sql"

:r "./CreateSchoolBooks/Tables/Protocols/SkillsCheckExamDutyProtocol/SkillsCheckExamDutyProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/SkillsCheckExamDutyProtocol/SkillsCheckExamDutyProtocolSupervisor.sql"

:r "./CreateSchoolBooks/Tables/Protocols/NvoExamDutyProtocol/NvoExamDutyProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/NvoExamDutyProtocol/NvoExamDutyProtocolStudent.sql"
:r "./CreateSchoolBooks/Tables/Protocols/NvoExamDutyProtocol/NvoExamDutyProtocolSupervisor.sql"

:r "./CreateSchoolBooks/Tables/Protocols/ExamResultProtocol/ExamResultProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/ExamResultProtocol/ExamResultProtocolClass.sql"
:r "./CreateSchoolBooks/Tables/Protocols/ExamResultProtocol/ExamResultProtocolStudent.sql"
:r "./CreateSchoolBooks/Tables/Protocols/ExamResultProtocol/ExamResultProtocolCommissioner.sql"

:r "./CreateSchoolBooks/Tables/Protocols/QualificationExamResultProtocol/QualificationDegree.sql"
:r "./CreateSchoolBooks/Tables/Protocols/QualificationExamResultProtocol/QualificationExamType.sql"
:r "./CreateSchoolBooks/Tables/Protocols/QualificationExamResultProtocol/QualificationExamResultProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/QualificationExamResultProtocol/QualificationExamResultProtocolClass.sql"
:r "./CreateSchoolBooks/Tables/Protocols/QualificationExamResultProtocol/QualificationExamResultProtocolStudent.sql"
:r "./CreateSchoolBooks/Tables/Protocols/QualificationExamResultProtocol/QualificationExamResultProtocolCommissioner.sql"

:r "./CreateSchoolBooks/Tables/Protocols/SkillsCheckExamResultProtocol/SkillsCheckExamResultProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/SkillsCheckExamResultProtocol/SkillsCheckExamResultProtocolEvaluator.sql"

:r "./CreateSchoolBooks/Tables/Protocols/QualificationAcquisitionProtocol/QualificationAcquisitionProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/QualificationAcquisitionProtocol/QualificationAcquisitionProtocolStudent.sql"
:r "./CreateSchoolBooks/Tables/Protocols/QualificationAcquisitionProtocol/QualificationAcquisitionProtocolCommissioner.sql"

:r "./CreateSchoolBooks/Tables/Protocols/GraduationThesisDefenseProtocol/GraduationThesisDefenseProtocol.sql"
:r "./CreateSchoolBooks/Tables/Protocols/GraduationThesisDefenseProtocol/GraduationThesisDefenseProtocolCommissioner.sql"

:r "./CreateSchoolBooks/Tables/MissingTopicsReport/MissingTopicsReport.sql"
:r "./CreateSchoolBooks/Tables/MissingTopicsReport/MissingTopicsReportItem.sql"
:r "./CreateSchoolBooks/Tables/MissingTopicsReport/MissingTopicsReportItemTeacher.sql"

:r "./CreateSchoolBooks/Tables/LectureSchedulesReport/LectureSchedulesReport.sql"
:r "./CreateSchoolBooks/Tables/LectureSchedulesReport/LectureSchedulesReportItem.sql"

:r "./CreateSchoolBooks/Tables/StudentsAtRiskOfDroppingOutReport/StudentsAtRiskOfDroppingOutReport.sql"
:r "./CreateSchoolBooks/Tables/StudentsAtRiskOfDroppingOutReport/StudentsAtRiskOfDroppingOutReportItem.sql"

:r "./CreateSchoolBooks/Tables/StudentSettings/StudentSettings.sql"

:r "./CreateSchoolBooks/Tables/AbsencesByClassesReport/AbsencesByClassesReport.sql"
:r "./CreateSchoolBooks/Tables/AbsencesByClassesReport/AbsencesByClassesReportItem.sql"

:r "./CreateSchoolBooks/Tables/AbsencesByStudentsReport/AbsencesByStudentsReport.sql"
:r "./CreateSchoolBooks/Tables/AbsencesByStudentsReport/AbsencesByStudentsReportItem.sql"

:r "./CreateSchoolBooks/Tables/GradelessStudentsReport/GradelessStudentsReport.sql"
:r "./CreateSchoolBooks/Tables/GradelessStudentsReport/GradelessStudentsReportItem.sql"

:r "./CreateSchoolBooks/Tables/RegularGradePointAverageByClassesReport/RegularGradePointAverageByClassesReport.sql"
:r "./CreateSchoolBooks/Tables/RegularGradePointAverageByClassesReport/RegularGradePointAverageByClassesReportItem.sql"

:r "./CreateSchoolBooks/Tables/RegularGradePointAverageByStudentsReport/RegularGradePointAverageByStudentsReport.sql"
:r "./CreateSchoolBooks/Tables/RegularGradePointAverageByStudentsReport/RegularGradePointAverageByStudentsReportItem.sql"

:r "./CreateSchoolBooks/Tables/FinalGradePointAverageByStudentsReport/FinalGradePointAverageByStudentsReport.sql"
:r "./CreateSchoolBooks/Tables/FinalGradePointAverageByStudentsReport/FinalGradePointAverageByStudentsReportItem.sql"

:r "./CreateSchoolBooks/Tables/FinalGradePointAverageByClassesReport/FinalGradePointAverageByClassesReport.sql"
:r "./CreateSchoolBooks/Tables/FinalGradePointAverageByClassesReport/FinalGradePointAverageByClassesReportItem.sql"

:r "./CreateSchoolBooks/Tables/SessionStudentsReport/SessionStudentsReport.sql"
:r "./CreateSchoolBooks/Tables/SessionStudentsReport/SessionStudentsReportItem.sql"

:r "./CreateSchoolBooks/Tables/ExamsReport/ExamsReport.sql"
:r "./CreateSchoolBooks/Tables/ExamsReport/ExamsReportItem.sql"

:r "./CreateSchoolBooks/Tables/DateAbsencesReport/DateAbsencesReport.sql"
:r "./CreateSchoolBooks/Tables/DateAbsencesReport/DateAbsencesReportItem.sql"

:r "./CreateSchoolBooks/Tables/ScheduleAndAbsencesByTermReport/ScheduleAndAbsencesByTermReport.sql"
:r "./CreateSchoolBooks/Tables/ScheduleAndAbsencesByTermReport/ScheduleAndAbsencesByTermReportWeek.sql"
:r "./CreateSchoolBooks/Tables/ScheduleAndAbsencesByTermReport/ScheduleAndAbsencesByTermReportWeekDay.sql"
:r "./CreateSchoolBooks/Tables/ScheduleAndAbsencesByTermReport/ScheduleAndAbsencesByTermReportWeekDayHour.sql"

:r "./CreateSchoolBooks/Tables/ScheduleAndAbsencesByMonthReport/ScheduleAndAbsencesByMonthReport.sql"
:r "./CreateSchoolBooks/Tables/ScheduleAndAbsencesByMonthReport/ScheduleAndAbsencesByMonthReportWeek.sql"
:r "./CreateSchoolBooks/Tables/ScheduleAndAbsencesByMonthReport/ScheduleAndAbsencesByMonthReportWeekDay.sql"
:r "./CreateSchoolBooks/Tables/ScheduleAndAbsencesByMonthReport/ScheduleAndAbsencesByMonthReportWeekDayHour.sql"

:r "./CreateSchoolBooks/Tables/ScheduleAndAbsencesByTermAllClassesReport/ScheduleAndAbsencesByTermAllClassesReport.sql"

:r "./CreateSchoolBooks/Tables/Publication/Publication.sql"
:r "./CreateSchoolBooks/Tables/Publication/PublicationFile.sql"

:r "./CreateSchoolBooks/Tables/PersonnelSchoolBookAccess/PersonnelSchoolBookAccess.sql"

:r "./CreateSchoolBooks/Tables/System/UpdateScript.sql"
:r "./CreateSchoolBooks/Tables/QueueMessage/QueueMessage.sql"

:r "./CreateSchoolBooks/Tables/PushSubscription/UserPushSubscription.sql"

---------------------------------------------------------------
--Functions
---------------------------------------------------------------
:r "./CreateSchoolBooks/Functions/fn_join_names.sql"
:r "./CreateSchoolBooks/Functions/fn_closest_weekday.sql"

---------------------------------------------------------------
--Views
---------------------------------------------------------------

:r "./CreateSchoolBooks/Views/Isosud/vwExtInstitution.sql"
:r "./CreateSchoolBooks/Views/Isosud/vwExtInstitutionStaff.sql"
:r "./CreateSchoolBooks/Views/Oidc/vwClassBooks.sql"
:r "./CreateSchoolBooks/Views/Oidc/vwStudentClassBooks.sql"
:r "./CreateSchoolBooks/Views/Internal/vwEplrHoursTaken.sql"
:r "./CreateSchoolBooks/Views/Internal/vwStudentsWithClassBookData.sql"
:r "./CreateSchoolBooks/Views/Internal/vwTeacherCurriculumClassBooks.sql"
:r "./CreateSchoolBooks/Views/RegBooks/vwRegBookCertificate.sql"
:r "./CreateSchoolBooks/Views/RegBooks/vwRegBookCertificateBasicDocument.sql"
:r "./CreateSchoolBooks/Views/RegBooks/vwRegBookCertificateDuplicate.sql"
:r "./CreateSchoolBooks/Views/RegBooks/vwRegBookCertificateDuplicateBasicDocument.sql"
:r "./CreateSchoolBooks/Views/RegBooks/vwRegBookQualification.sql"
:r "./CreateSchoolBooks/Views/RegBooks/vwRegBookQualificationBasicDocument.sql"
:r "./CreateSchoolBooks/Views/RegBooks/vwRegBookQualificationDuplicate.sql"
:r "./CreateSchoolBooks/Views/RegBooks/vwRegBookQualificationDuplicateBasicDocument.sql"
