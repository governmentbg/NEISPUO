import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  StudentClassBookService,
  StudentClassBook_GetParentMeetings
} from 'projects/sb-api-client/src/api/studentClassBook.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  selector: 'sb-parent-meetings',
  templateUrl: './parent-meetings.component.html'
})
export class ParentMeetingsComponent {
  readonly fasPlus = fasPlus;

  dataSource: TableDataSource<StudentClassBook_GetParentMeetings>;

  constructor(private studentClassBookService: StudentClassBookService, route: ActivatedRoute) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      studentClassBookService.getParentMeetings({ schoolYear, classBookId, personId, offset, limit })
    );
  }
}
