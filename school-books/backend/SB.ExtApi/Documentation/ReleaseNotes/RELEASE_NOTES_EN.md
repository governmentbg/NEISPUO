# Release Notes

[comment]: # (!!! Don't forget to change document.Version in services.AddOpenApiDocument too !!!)

[comment]: # (## v1.0.23)
[comment]: # (- change 1)
[comment]: # (- change 2)

## v1.0.22
- Added an indicator (`MedicalNoticeBatchDO.HasMore`) whether there are more medical notices to retrieve for the current provider in the `MedicalNotices` API for retrieving medical notices. In rare cases, fewer notices than requested with the next parameter may be received, but there may actually be more to retrieve. Therefore, it is advisable to check this field and if it is `true`, call the MedicalNotices_GetNext method again.

## v1.0.21
- Added API for retrieving medical notices from the National Health Information System (NHIS) - `MedicalNotices`
- Removed validation for the `GradeResult.InitialResultType` field, records with `RepeatsGrade` value can now be created or modified.

## v1.0.20
- Changed the StatusCodes returned for database errors as follows:
    - Timeout - 408 RequestTimeout
    - ViolationOfCheckConstraint - 400 BadRequest
    - ViolationOfReferenceConstraint - 400 BadRequest
    - ViolationOfUniqueKeyConstraint - 409 Conflict
    - ViolationOfUniqueIndex - 409 Conflict

## v1.0.19
- The `GradeDO.CurriculumId` field is no longer required when `ScheduleLessonId` is provided.

## v1.0.18
- Added APIs for "Finalization" functionality:
    - `ClassBookDO.IsFinalized` - Flag for finalized class book
    - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/finalize` - endpoint for finalization that accepts only PDF files signed with qualified electronic signature.
    - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/unfinalize` - endpoint for removing finalization
- Added ability to upload fractional annual grades by subjects with study method:
    - 152 - PP (I profiling subject)
    - 153 - PP (II profiling subject)
    - 154 - PP (III profiling subject)
    - 155 - PP (IV profiling subject)
    - 156 - PP (III/IV profiling subject)
- Added field `PgResultDO.SubjectId` and UNIQUE CONSTRAINT in PgResult `(SchoolYear, ClassBookId, PersonId, SubjectId)`. The field `PgResultDO.CurriculumId` is `deprecated`, and `Curriculum.SubjectId` is taken from it if none is filled in `PgResultDO`

## v1.0.17
- Added field `SysErrorMessage` to errors with `StatusCode = 400`, which contains database errors matching some of the following regular expressions. For these errors, the system returns `StatusCode = 400` instead of `StatusCode = 500`.

    - `The timeout period elapsed prior to completion of the operation or the server is not responding`
    - `The (INSERT|UPDATE) statement conflicted with the CHECK constraint \"CHK_[a-zA-Z_0-9]+\"`
    - `The (UPDATE|DELETE) statement conflicted with the REFERENCE constraint \"FK_[a-zA-Z_0-9]+\"`
    - `Violation of UNIQUE KEY constraint 'UK_[a-zA-Z_0-9]+'`
    - `Cannot insert duplicate key row in object '[a-zA-Z_0-9.]+' with unique index '(UQ|IX)_[a-zA-Z_0-9]+'`

## v1.0.16
- Added ability to provide Term for grades that don't have ScheduleLessonId - `GradeDO.Term`.

## v1.0.15
- Added ability for PG Results by educational direction - `PgResultDO.CurriculumId`. The option to provide results without educational direction (`CurriculumId = NULL`) is preserved. There is a uniqueness index for the ordered n-tuple `(SchoolYear, ClassBookId, PersonId, CurriculumId)`.

## v1.0.14
- Added ability for multiple topics in one lesson - `TopicDO.Titles`, the `Title` field is kept for backwards compatibility

## v1.0.13
- Added API for "Carried Absences" - `StudentCarriedAbsences`
- Added ability for school year settings to be for specific classes/groups - `SchoolYearDateInfoDO.ClassBookIds`
- Added ability to create class books for classes/groups without "Class Type" (`ClassType`)

## v1.0.12
- Added validation for the length of all "string" type fields (maxLength attribute in swagger)

## v1.0.11
- Added grade type 98 - "From other class"
- **BREAKING CHANGE!** Grades of type 98 - "From other class" and 99 - "From other school" are now **without** `ScheduleLessonId` similar to 21 - "Term" and 22 - "Annual"

## v1.0.10
- Added ability for non-school periods to be for specific classes/groups - `OffDayDO.ClassBookIds`
- Added ability to provide data in class books for withdrawn students

## v1.0.9
- Added ability to change the shift of an existing schedule with a compatible one, i.e., one that covers all used hours (in absence, grade, topic).
- Added ability to create schedules with custom shifts (Adhoc shifts).
- Added ability to create schedules including Saturday and Sunday.
- Added API for "Interest Activities" - `StudentActivities`
- Added API for "Student Number" - `StudentClassNumbers`
- Added API for "Exempt from Subjects" - `StudentGradelessCurriculums`
- Added API for "SEN Grades by Subjects" - `StudentSpecialNeedCurriculums`

## v1.0.8
- Added API for "Teacher Absences" - `TeacherAbsences`
- Added API for "Additional Activities" for class book 3-5 (KG/PG) - `AdditionalActivities`
- Removed the curriculum subject identifier field (`CurriculumId`) from the absence object (`Absence`), as it overlapped with the same field in the lesson it is associated with (`ScheduleLesson`).

## v1.0.7
- Added ability to separate weeks from a weekly schedule (`Schedule`) into a new schedule. Unlike schedule editing, week separation can be done even with existing related objects (Grades/Absences/Topics) for the schedule lessons. The identifiers of the separated lessons (ScheduleLessonId) do not change after the operation, and all related objects (Grades/Absences/Topics) remain correctly associated with the separated schedule.

## v1.0.6
- Added ability to create weekly schedules (`Schedule`) with fewer hours than the shift hours
- Added ability to create shifts (`Shift`) with start > end (e.g., 18:00 -> 01:00)
- Added ability to create shifts (`Shift`) with non-consecutive hours - 1, 3, 5.
- **BREAKING CHANGE!** In shifts (`Shift`) there is a new requirement that the hour must be greater than or equal to 0 (`hourNumber >= 0`).
  This requirement has always applied when creating schedules (`Schedule`) and attempts to create schedules with such hours have returned an error.
- Added ability to create shifts (`Shift`) with different modes for different days of the week.
  For this purpose, a field `isMultiday` has been added to `ShiftDO`, which indicates that the shift has different modes for different days.
  Also, a field `day` has been added to `ShiftHourDO`, which indicates which hour applies to which day. If the shift has no different modes (`isMultiday = false`), then `day` should always be 1.
  Temporarily, the option for `isMultiday` and `day` not to be required is left, in which cases it is considered that `isMultiday=false` and `day=1`.

## v1.0.5
- Added API for "Support" for class books 3-14(grades 1-3), 3-16(grade 4), 3-87(grades 5-12) and 3-62(CSOP) - `Supports`
- Added API for "Individual Work" for class books 3-16(grade 4) and 3-62(CSOP) - `IndividualWorks`
- Added API for "Performances" for class book 3-63.1(DPLR) - `Performances`
- Added API for "REPLR Participation" for class book 3-63.1(DPLR) - `ReplrParticipations`

## v1.0.4
- **BREAKING CHANGE!** Notes can now be created for some/several/all students.
  For this purpose, two new properties `IsForAllStudents` and `Students` have been added, and `ClassId` and `PersonId` have been removed

## v1.0.3
- Added API for "School Year Settings" - `SchoolYearDateInfo`

## v1.0.2
- The field `TopicDO.curriculumId` becomes optional and **deprecated** and will be removed in one of the following versions

## v1.0.1
- The field `ScheduleDO.term` becomes optional
