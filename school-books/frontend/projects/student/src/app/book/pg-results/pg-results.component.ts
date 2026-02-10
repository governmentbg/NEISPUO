import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  StudentClassBookService,
  StudentClassBook_GetPgResults
} from 'projects/sb-api-client/src/api/studentClassBook.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  selector: 'sb-pg-results',
  templateUrl: './pg-results.component.html'
})
export class PgResultsComponent {
  readonly fasPlus = fasPlus;

  dataSource: TableDataSource<StudentClassBook_GetPgResults>;

  constructor(private studentClassBookService: StudentClassBookService, route: ActivatedRoute) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      studentClassBookService.getPgResults({ schoolYear, classBookId, personId, offset, limit })
    );
  }
}
