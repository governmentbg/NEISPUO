import { Component, Inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faUserMinus as fadUserMinus } from '@fortawesome/pro-duotone-svg-icons/faUserMinus';
import { faUserPlus as fadUserPlus } from '@fortawesome/pro-duotone-svg-icons/faUserPlus';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { TeacherAbsencesService, TeacherAbsences_GetAll } from 'projects/sb-api-client/src/api/teacherAbsences.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { deepEqual, enumFromStringValue, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

export enum TeacherAbsencesMode {
  My = 'My',
  MyRepl = 'MyRepl',
  All = 'All'
}

@Component({
  selector: 'sb-teacher-absences',
  templateUrl: './teacher-absences.component.html'
})
export class TeacherAbsencesComponent {
  dataSource: TableDataSource<TeacherAbsences_GetAll>;

  readonly TeacherAbsencesMode = TeacherAbsencesMode;

  readonly searchForm = this.fb.nonNullable.group({
    teacherName: this.fb.nonNullable.control<string | null | undefined>(null),
    replTeacherName: this.fb.nonNullable.control<string | null | undefined>(null)
  });

  fadCalendarMinus = fadCalendarMinus;
  fadUserMinus = fadUserMinus;
  fadUserPlus = fadUserPlus;
  fasPlus = fasPlus;
  fadChevronCircleRight = fadChevronCircleRight;
  canCreate = false;
  mode: TeacherAbsencesMode;

  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    teacherAbsencesService: TeacherAbsencesService,
    route: ActivatedRoute,
    private fb: FormBuilder
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    this.mode =
      enumFromStringValue(TeacherAbsencesMode, route.snapshot.paramMap.get('mode')) ?? throwParamError('mode');

    if (this.mode === TeacherAbsencesMode.All) {
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        teacherAbsencesService.getAll({
          schoolYear,
          instId,
          offset,
          limit,
          ...this.searchForm.value
        })
      );
    } else if (this.mode === TeacherAbsencesMode.My) {
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        teacherAbsencesService.getMyAbsences({
          schoolYear,
          instId,
          replTeacherName: this.searchForm.value.replTeacherName,
          offset,
          limit
        })
      );
    } else if (this.mode === TeacherAbsencesMode.MyRepl) {
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        teacherAbsencesService.getMyReplacements({
          schoolYear,
          instId,
          teacherName: this.searchForm.value.teacherName,
          offset,
          limit
        })
      );
    } else {
      throwParamError('mode');
    }

    this.searchForm.valueChanges
      .pipe(
        debounceTime(200),
        distinctUntilChanged((a, b) => deepEqual(a, b))
      )
      .subscribe(() => {
        this.dataSource.reload();
      });

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    institutionInfo.then((institutionInfo) => {
      this.canCreate =
        institutionInfo.schoolYearAllowsModifications &&
        institutionInfo.hasTeacherAbsencesCreateAccess &&
        this.mode === TeacherAbsencesMode.All;
    });
  }
}
