USE [$(dbName)]
GO

---------------------------------------------------------------
--Stored Procedures
---------------------------------------------------------------

:r "./CreateExt/StoredProcedures/DigitalBackpack/Get_DBP_ClassSchedule.sql"
:r "./CreateExt/StoredProcedures/DigitalBackpack/Get_DBP_StudentAbsences.sql"
:r "./CreateExt/StoredProcedures/DigitalBackpack/Get_DBP_StudentGrades.sql"
:r "./CreateExt/StoredProcedures/DigitalBackpack/Get_DBP_StudentRemarks.sql"
:r "./CreateExt/StoredProcedures/DigitalBackpack/Get_DBP_StudentSchedule.sql"
:r "./CreateExt/StoredProcedures/DigitalBackpack/Get_DBP_TeacherSchedule.sql"

:r "./CreateExt/StoredProcedures/Nomenclatures/Get_NomAbsenceReason.sql"
:r "./CreateExt/StoredProcedures/Nomenclatures/Get_NomPerformanceType.sql"
:r "./CreateExt/StoredProcedures/Nomenclatures/Get_NomReplrParticipationType.sql"
:r "./CreateExt/StoredProcedures/Nomenclatures/Get_NomSupportActivityType.sql"
:r "./CreateExt/StoredProcedures/Nomenclatures/Get_NomSupportDifficultyType.sql"

:r "./CreateExt/StoredProcedures/EduSurvey/Get_EduSurvey_Students.sql"
:r "./CreateExt/StoredProcedures/EduSurvey/Get_EduSurvey_Teachers.sql"

:r "./CreateExt/StoredProcedures/PPO/Get_PPO_StudentGrades.sql"
