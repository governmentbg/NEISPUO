import { Component, Inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { ClassBookStudentNomsService } from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import {
  PersonMedicalNoticesService,
  PersonMedicalNotices_GetAllByClassBook
} from 'projects/sb-api-client/src/api/personMedicalNotices.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { deepEqual, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  selector: 'sb-medical-notices',
  templateUrl: './medical-notices.component.html'
})
export class MedicalNoticesComponent {
  readonly fasArrowLeft = fasArrowLeft;

  readonly searchForm = this.fb.nonNullable.group({
    studentPersonId: this.fb.nonNullable.control<number | null | undefined>(null),
    fromDate: this.fb.nonNullable.control<Date | null | undefined>(null),
    toDate: this.fb.nonNullable.control<Date | null | undefined>(null)
  });

  classBookStudentNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;
  dataSource: TableDataSource<PersonMedicalNotices_GetAllByClassBook>;
  canCreate = false;
  fullBookName = '';

  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    personMedicalNoticesService: PersonMedicalNoticesService,
    route: ActivatedRoute,
    classBookStudentNomsService: ClassBookStudentNomsService,
    private fb: FormBuilder
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.classBookStudentNomsService = new NomServiceWithParams(classBookStudentNomsService, () => ({
      schoolYear,
      instId,
      classBookId
    }));

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      personMedicalNoticesService.getAllByClassBook({
        schoolYear,
        instId,
        classBookId,
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
  }
}
