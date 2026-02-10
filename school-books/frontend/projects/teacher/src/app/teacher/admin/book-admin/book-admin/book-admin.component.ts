import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { TeacherService, Teacher_GetAllClassBooksAdmin } from 'projects/sb-api-client/src/api/teacher.service';
import { ClassKind } from 'projects/sb-api-client/src/model/classKind';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { deepEqual, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'sb-book-admin',
  templateUrl: './book-admin.component.html'
})
export class BookAdminComponent {
  dataSource: TableDataSource<Teacher_GetAllClassBooksAdmin>;

  fadChevronCircleRight = fadChevronCircleRight;

  readonly searchForm = this.fb.nonNullable.group({
    bookName: this.fb.nonNullable.control<string | null | undefined>(null)
  });

  constructor(teacherService: TeacherService, route: ActivatedRoute, private fb: FormBuilder) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classKind = (route.snapshot.paramMap.get('classKind') ?? throwParamError('classKind')) as ClassKind;

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      teacherService.getAllClassBooksAdmin({
        schoolYear,
        instId,
        classKind,
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
