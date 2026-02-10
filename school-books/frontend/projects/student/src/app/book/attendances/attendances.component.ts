import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarTimes as farCalendarTimes } from '@fortawesome/pro-regular-svg-icons/faCalendarTimes';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import { format } from 'date-fns';
import { bg } from 'date-fns/locale';
import {
  StudentClassBookService,
  StudentClassBook_GetAttendances,
  StudentClassBook_GetAttendancesMonthStats
} from 'projects/sb-api-client/src/api/studentClassBook.service';
import { AttendanceType } from 'projects/sb-api-client/src/model/attendanceType';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class AttendancesSkeletonComponent extends SkeletonComponentBase {
  constructor(studentClassBookService: StudentClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');

    this.resolve(AttendancesComponent, {
      schoolYear,
      classBookId,
      personId,
      attendancesByMonths: studentClassBookService.getAttendancesMonthStats({
        schoolYear,
        classBookId,
        personId
      })
    });
  }
}

type Attendance = ArrayElementType<StudentClassBook_GetAttendances>;

@Component({
  selector: 'sb-attendances',
  templateUrl: './attendances.component.html'
})
export class AttendancesComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    classBookId: number;
    personId: number;
    attendancesByMonths: StudentClassBook_GetAttendancesMonthStats;
  };

  readonly fasBookOpen = fasBookOpen;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly farCalendarTimes = farCalendarTimes;

  readonly ATTENDANCE_TYPE_PRESENCE = AttendanceType.Presence;
  readonly ATTENDANCE_TYPE_UNEXCUSED = AttendanceType.UnexcusedAbsence;
  readonly ATTENDANCE_TYPE_EXCUSED = AttendanceType.ExcusedAbsence;

  studentAttendances!: ReturnType<typeof getStudentAttendances>;

  selectedYear?: number;
  selectedMonth?: number;
  selectedType?: AttendanceType;
  selectedAttendances?: Attendance[];
  loadingAttendances = false;

  constructor(private studentClassBookService: StudentClassBookService) {}

  ngOnInit() {
    this.studentAttendances = getStudentAttendances(this.data);
  }

  viewAttendances(year: number, month: number, type: AttendanceType) {
    this.selectedAttendances = undefined;

    if (this.selectedYear === year && this.selectedMonth === month && this.selectedType === type) {
      this.selectedYear = undefined;
      this.selectedMonth = undefined;
      this.selectedType = undefined;

      return;
    }

    this.selectedYear = year;
    this.selectedMonth = month;
    this.selectedType = type;
    this.loadingAttendances = true;

    return this.studentClassBookService
      .getAttendances({
        schoolYear: this.data.schoolYear,
        classBookId: this.data.classBookId,
        personId: this.data.personId,
        year: this.selectedYear,
        month: this.selectedMonth,
        type: this.selectedType
      })
      .toPromise()
      .then((attendances) => {
        this.selectedAttendances = attendances;
      })
      .catch((err) => {
        this.selectedYear = undefined;
        this.selectedMonth = undefined;
        this.selectedType = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingAttendances = false;
      });
  }
}

function getStudentAttendances(data: AttendancesComponent['data']) {
  const repeatingMonths: boolean[] = [];
  const monthAttendances = data.attendancesByMonths.map((m) => {
    const result = {
      ...m,
      monthName: format(
        new Date(m.year, m.month - 1, 1),
        !repeatingMonths[m.month] ? 'MMMM' : 'MMMM (yyyy)', // September repeats, so add year
        { locale: bg }
      )
    };

    repeatingMonths[m.month] = true;

    return result;
  });

  const totals = {
    presencesCountTotal: monthAttendances.reduce((sum, s) => sum + s.presencesCount, 0),
    unexcusedAbsencesCountTotal: monthAttendances.reduce((sum, s) => sum + s.unexcusedAbsencesCount, 0),
    excusedAbsencesCountTotal: monthAttendances.reduce((sum, s) => sum + s.excusedAbsencesCount, 0)
  };

  return {
    monthAttendances,
    totals
  };
}
