type ScheduleHourTeacher = {
  teacherPersonId: number;
  teacherFirstName: string;
  teacherLastName: string;
  markedAsNoReplacement: boolean;
};

type ScheduleHourSubject = {
  curriculumGroupName?: string | null;
  subjectName?: string;
  subjectNameShort?: string | null;
  subjectTypeName?: string;
  isIndividualLesson?: boolean;
  isIndividualCurriculum?: boolean;
};

type ScheduleHour = ScheduleHourSubject & {
  curriculumTeachers?: ScheduleHourTeacher[];
  teacherAbsenceId?: number | null;
  replTeacher?: ScheduleHourTeacher | null;
  replTeacherIsNonSpecialist?: boolean | null;
  extTeacherName?: string | null;
  isEmptyHour?: boolean | null;
};

export const dayNames: Record<number, string> = {
  1: 'Понеделник',
  2: 'Вторник',
  3: 'Сряда',
  4: 'Четвъртък',
  5: 'Петък',
  6: 'Събота',
  7: 'Неделя'
};

const getHourSubject = <S extends ScheduleHourSubject>(hour: S) =>
  (hour.subjectName && hour.subjectTypeName && hour.curriculumGroupName
    ? `${hour.subjectName} / ${hour.subjectTypeName} - ${hour.curriculumGroupName}`
    : hour.subjectName && hour.subjectTypeName
    ? `${hour.subjectName} / ${hour.subjectTypeName}`
    : '') +
  (hour.isIndividualLesson ? ' (ИЧ)' : '') +
  (hour.isIndividualCurriculum ? ' (ИУП)' : '');

const getHourSubjectShort = <S extends ScheduleHourSubject>(hour: S) =>
  ((hour.subjectNameShort || hour.subjectName) && hour.subjectTypeName && hour.curriculumGroupName
    ? `${hour.subjectNameShort || hour.subjectName} / ${hour.subjectTypeName} - ${hour.curriculumGroupName}`
    : (hour.subjectNameShort || hour.subjectName) && hour.subjectTypeName
    ? `${hour.subjectNameShort || hour.subjectName} / ${hour.subjectTypeName}`
    : '') +
  (hour.isIndividualLesson ? ' (ИЧ)' : '') +
  (hour.isIndividualCurriculum ? ' (ИУП)' : '');

export const extendScheduleHour = <S extends ScheduleHour>(hour: S) => ({
  ...hour,
  subject: hour.isEmptyHour
    ? `Свободен час\n(${getHourSubject(hour)})`
    : hour.replTeacherIsNonSpecialist
    ? 'Гражданско образование'
    : getHourSubject(hour),
  subjectShort: hour.isEmptyHour // prettier - force multiline
    ? ` Св. час\n(${getHourSubjectShort(hour)})`
    : hour.replTeacherIsNonSpecialist
    ? 'ГО'
    : getHourSubjectShort(hour),
  teacher: hour.extTeacherName
    ? `${hour.extTeacherName}(външен лектор)`
    : hour.teacherAbsenceId
    ? hour.replTeacher
      ? `${hour.replTeacher.teacherFirstName} ${hour.replTeacher.teacherLastName}(зам.)`
      : null
    : hour.curriculumTeachers
        ?.map((t) =>
          t.markedAsNoReplacement
            ? `без постоянно зам. учител (${t.teacherFirstName} ${t.teacherLastName})`
            : `${t.teacherFirstName} ${t.teacherLastName}`
        )
        ?.join(', ') || '',
  teacherShort: hour.extTeacherName
    ? `${hour.extTeacherName}(в. лектор)`
    : hour.teacherAbsenceId
    ? hour.replTeacher
      ? `${hour.replTeacher.teacherLastName}(зам.)`
      : null
    : hour.curriculumTeachers
        ?.map((t) => (t.markedAsNoReplacement ? `Без зам. уч.(${t.teacherLastName})` : t.teacherLastName))
        ?.join(', ') || ''
});

type ScheduleLesson = ScheduleHourSubject & {
  replTeacherIsNonSpecialist?: boolean | null;
  isIndividualSchedule: boolean;
  studentFirstName?: string | null;
  studentMiddleName?: string | null;
  studentLastName?: string | null;
};

const joinNames = (
  firstName: string | null | undefined,
  middleName: string | null | undefined,
  lastName: string | null | undefined
) => [firstName, middleName, lastName].filter(Boolean).join(' ');

export const getLessonName = <S extends ScheduleLesson>(lesson: S) =>
  (lesson.replTeacherIsNonSpecialist ? 'Гражданско образование' : getHourSubject(lesson)) +
  (lesson.replTeacherIsNonSpecialist != null ? ' (зам.)' : '') +
  (lesson.isIndividualSchedule
    ? ` (${joinNames(lesson.studentFirstName, lesson.studentMiddleName, lesson.studentLastName)})`
    : '');

export const getLessonNameShort = <S extends ScheduleLesson>(lesson: S) =>
  (lesson.replTeacherIsNonSpecialist ? 'ГО' : getHourSubjectShort(lesson)) +
  (lesson.replTeacherIsNonSpecialist != null ? ' (зам.)' : '') +
  (lesson.isIndividualSchedule ? ` (${joinNames(lesson.studentFirstName, null, lesson.studentLastName)})` : '');
