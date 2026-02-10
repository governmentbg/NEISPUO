import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { isBefore } from 'date-fns';
import {
  HisMedicalNoticesService,
  HisMedicalNotices_GetAll
} from 'projects/sb-api-client/src/api/hisMedicalNotices.service';
import { SchoolYearNomsService } from 'projects/sb-api-client/src/api/schoolYearNoms.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { deepEqual } from 'projects/shared/utils/various';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'sb-his-medical-notices',
  templateUrl: './his-medical-notices.component.html'
})
export class HisMedicalNoticesComponent {
  readonly fasArrowLeft = fasArrowLeft;
  readonly fadChevronCircleRight = fadChevronCircleRight;

  schoolYearNomsService: INomService<number, { schoolYear: number; instId: number }>;
  readonly searchForm = this.fb.nonNullable.group({
    schoolYear: this.fb.nonNullable.control<number | null | undefined>(null, Validators.required),
    nrnMedicalNotice: this.fb.nonNullable.control<string | null | undefined>(null),
    nrnExamination: this.fb.nonNullable.control<string | null | undefined>(null),
    identifier: this.fb.nonNullable.control<string | null | undefined>(null)
  });

  dataSource: TableDataSource<HisMedicalNotices_GetAll>;

  constructor(
    schoolYearNomsService: SchoolYearNomsService,
    hisMedicalNoticesService: HisMedicalNoticesService,
    private fb: FormBuilder
  ) {
    const currentYear = new Date().getFullYear();
    let currentSchoolYear = new Date().getFullYear();
    const schoolYearCutoff = new Date(currentYear, 8, 1);
    if (isBefore(new Date(), schoolYearCutoff)) {
      currentSchoolYear = currentYear - 1;
    } else {
      currentSchoolYear = currentYear;
    }

    this.schoolYearNomsService = new NomServiceWithParams(schoolYearNomsService, () => ({
      schoolYear: currentSchoolYear,
      instId: -1
    }));

    this.searchForm.get('schoolYear')?.setValue(currentSchoolYear);
    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      hisMedicalNoticesService.getAll({
        offset,
        limit,
        schoolYear: this.searchForm.value.schoolYear ?? currentSchoolYear,
        nrnMedicalNotice: this.searchForm.value.nrnMedicalNotice,
        nrnExamination: this.searchForm.value.nrnExamination,
        identifier: this.searchForm.value.identifier
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
  }
}
