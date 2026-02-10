import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  StudentInfoClassBookService,
  StudentInfoClassBook_GetSanctions
} from 'projects/sb-api-client/src/api/studentInfoClassBook.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  selector: 'sb-sanctions',
  templateUrl: './sanctions.component.html'
})
export class SanctionsComponent {
  readonly fasPlus = fasPlus;

  dataSource: TableDataSource<StudentInfoClassBook_GetSanctions>;

  constructor(private studentInfoClassBookService: StudentInfoClassBookService, route: ActivatedRoute) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const studentClassBookId =
      tryParseInt(route.snapshot.paramMap.get('studentClassBookId')) ?? throwParamError('studentClassBookId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      studentInfoClassBookService.getSanctions({
        schoolYear,
        instId,
        classBookId,
        personId,
        studentClassBookId,
        offset,
        limit
      })
    );
  }
}
