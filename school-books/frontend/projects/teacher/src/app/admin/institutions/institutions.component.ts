import { Component } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { InstitutionsService, Institutions_GetAll } from 'projects/sb-api-client/src/api/institutions.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'sb-institutions',
  templateUrl: './institutions.component.html'
})
export class InstitutionsComponent {
  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;

  dataSource: TableDataSource<Institutions_GetAll>;
  searchForm = this.fb.group({
    institutionId: null,
    institutionName: null,
    townName: null
  });

  constructor(institutionsService: InstitutionsService, private fb: UntypedFormBuilder) {
    this.dataSource = new TableDataSource<Institutions_GetAll>((sortBy, sortDirection, offset, limit) =>
      institutionsService
        .getAll({
          offset,
          limit,
          ...this.searchForm.value
        })
        .pipe(catchError(() => of({ result: [], length: 0 })))
    );

    this.searchForm.valueChanges
      .pipe(
        debounceTime(200),
        distinctUntilChanged(
          (a, b) =>
            a.institutionId === b.institutionId && a.institutionName === b.institutionName && a.townName === b.townName
        )
      )
      .subscribe(() => {
        this.dataSource.reload();
      });
  }
}
