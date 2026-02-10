## Prerequisites:
- You must have a valid certificate.
- There must be a school that has chosen you as a service provider for the Class List Template module.
	
## Specific Business Processes:
### Beginning of the school year and class book creation:
- There must be an issued order from the Ministry of Education ("Order for determining the academic time schedule") for the respective school year, which is issued at the end of August.
- School year settings must be created for all classes/groups:
		used - `/{schoolYear}/{institutionId}/schoolYearDateInfos`

### Creating weekly schedule:
- For a class to have data entered, it must have a weekly schedule. There are two ways to create a weekly schedule:
  - Using a pre-entered shift during weekly schedule creation:
		- a shift must be created in advance and then the created shift must be used when creating the schedule:
			used - `/{schoolYear}/{institutionId}/shifts`
			then the created shift is used at - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/schedules`
  - Defining a shift during weekly schedule creation:
		used - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/schedules`
- Schedule splitting - after a schedule is created and for any of the periods there are entered grades, student absences, topics, teacher absences, lecture hours, they must be removed to be able to edit the schedule. In most cases this is very difficult and therefore in these cases schedule splitting has been created, through which you can separate the remainder of the schedule period into a separate schedule, for which there will be no entered data and can be edited. Only the weeks that you want to separate from the schedule and for which there is still no entered data should be passed. Used - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/schedules/{scheduleId}/splitSchedule`:
	
### Creating non-school day:
- To be able to create a non-school day/period, all grades, student absences, topics, teacher absences, lecture hours for the non-school days period must be removed:
	used - `/{schoolYear}/{institutionId}/offDays`

### Class book finalization/Removing finalization:
- At the end of the school year, every class book must be finalized before moving to the new school year. This is done by signing a class book printed by an external provider in PDF format, which institution directors sign with electronic signature, after which it must be sent to - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/finalize`

- Another option is for these signed class books to be uploaded manually by directors in a UI created for this purpose in NEISPUO.

## Additional Business Processes:
### Types of class books and their corresponding sections:
- Preparatory group (ClassBookType = Book_PG):
  - Have only attendance/absences - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/attendances`
  - Do not have grades, as well as term/annual grades - have only "Results"
  - Parent meetings - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Preschool results - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/pgResults`
  - Notes - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- 1st grade class books (ClassBookType = Book_I_III):
	- Have tardiness/unexcused absences/excused absences - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
	- Have grades (qualitative), without term/annual grades by individual subjects, as "Overall annual success" is filled in at the end of the school year - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/grades`- qualitativeGrade
  - Topics - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Remarks - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/remarks`
  - Parent meetings - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Individual work - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/individualWorks`
  - Tests - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/exams`
  - Support - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/supports`
  - Overall annual success - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/firstGradeResults`
  - Notes - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- 2nd, 3rd grade class books (ClassBookType = Book_I_III):
	- Have tardiness/unexcused absences/excused absences - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
  - Have grades (qualitative), without term grades by individual subjects, but have annual grades - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/grades` - qualitativeGrade
  - Topics - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Remarks - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/remarks`
  - Parent meetings - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Individual work - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/individualWorks`
  - Tests - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/exams`
  - Support - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/supports`
  - Notes - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- 4th grade class books (ClassBookType = Book_IV):
	- Have tardiness/unexcused absences/excused absences - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
  - Have grades (quantitative), have term and annual grades - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/grades` - decimalGrade
  - Topics - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Remarks - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/remarks`
  - Parent meetings - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Individual work - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/individualWorks`
  - Tests - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/exams`
  - Sanctions - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/sanctions`
  - Support - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/supports`
  - Notes - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- 5th to 12th grade class books (ClassBookType = Book_V_XII):
	- Have tardiness/unexcused absences/excused absences - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
  - Have grades (quantitative), have term and annual grades - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/grades` - decimalGrade
  - Topics - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Remarks - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/remarks`
  - Parent meetings - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Individual work - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/individualWorks`
  - Exams - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/exams`
  - Sanctions - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/sanctions`
  - Support - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/supports`
  - Results - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/gradeResults`
  - Notes - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- CDO (ClassBookType = Book_CDO):
  - Have tardiness/unexcused absences/excused absences - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
  - Topics - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Notes - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- DPLR (ClassBookType = Book_DPLR):
  - Have attendance/absences - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences` - using DPLR values
  - Topics - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Performances - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/performances`
  - REPLR participation - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/replrParticipations`
  - Notes - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- CSOP (ClassBookType = Book_CSOP):
  - Have tardiness/unexcused absences/excused absences - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
  - Have grades (special grades), have term and annual grades - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/grades` - specialGrade
  - Topics - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Parent meetings - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Individual work - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/individualWorks`
  - Support - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/supports`
  - Notes - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`

### Student endpoints:
- Indicating that a student receives SOP grades for a given subject - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/studentSpecialNeedCurriculums`
- Transferring absences for a student coming from another parallel class/institution - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/studentCarriedAbsences`
- Indicating that a student is exempt from a given subject - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/studentGradelessCurriculums`
- Renumbering a student from a given class - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/studentClassNumbers`
- Indicating extracurricular activities (informative information, optional) - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/studentActivities`
