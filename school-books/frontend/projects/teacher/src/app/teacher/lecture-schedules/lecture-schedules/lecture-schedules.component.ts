import { Component, Inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faCalendarStar as fadCalendarStar } from '@fortawesome/pro-duotone-svg-icons/faCalendarStar';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faUserPlus as fadUserPlus } from '@fortawesome/pro-duotone-svg-icons/faUserPlus';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  LectureSchedulesService,
  LectureSchedules_GetAll
} from 'projects/sb-api-client/src/api/lectureSchedules.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { deepEqual, enumFromStringValue, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

export enum LectureSchedulesMode {
  My = 'My',
  All = 'All'
}

@Component({
  selector: 'sb-lecture-schedules',
  templateUrl: './lecture-schedules.component.html'
})
export class LectureSchedulesComponent {
  dataSource: TableDataSource<LectureSchedules_GetAll>;

  readonly LectureSchedulesMode = LectureSchedulesMode;

  readonly searchForm = this.fb.nonNullable.group({
    teacherName: this.fb.nonNullable.control<string | null | undefined>(null)
  });

  fadCalendarMinus = fadCalendarMinus;
  fadCalendarStar = fadCalendarStar;
  fadUserPlus = fadUserPlus;
  fasPlus = fasPlus;
  fadChevronCircleRight = fadChevronCircleRight;
  canCreate = false;
  mode: LectureSchedulesMode;

  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    lectureSchedulesService: LectureSchedulesService,
    route: ActivatedRoute,
    private fb: FormBuilder
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    this.mode =
      enumFromStringValue(LectureSchedulesMode, route.snapshot.paramMap.get('mode')) ?? throwParamError('mode');

    if (this.mode === LectureSchedulesMode.All) {
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        lectureSchedulesService.getAll({
          schoolYear,
          instId,
          offset,
          limit,
          ...this.searchForm.value
        })
      );

      this.searchForm.valueChanges
        .pipe(
          debounceTime(200),
          distinctUntilChanged((a, b) => deepEqual(a, b))
        )
        .subscribe(() => {
          this.dataSource.reload();
        });
    } else if (this.mode === LectureSchedulesMode.My) {
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        lectureSchedulesService.getMySchedules({
          schoolYear,
          instId,
          offset,
          limit
        })
      );
    } else {
      throwParamError('mode');
    }

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    institutionInfo.then((institutionInfo) => {
      this.canCreate =
        institutionInfo.schoolYearAllowsModifications &&
        institutionInfo.hasLectureSchedulesCreateAccess &&
        this.mode === LectureSchedulesMode.All;
    });
  }
}
