import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  StudentInfoClassBookService,
  StudentInfoClassBook_GetNotes
} from 'projects/sb-api-client/src/api/studentInfoClassBook.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  selector: 'sb-notes',
  templateUrl: './notes.component.html'
})
export class NotesComponent {
  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;

  dataSource: TableDataSource<StudentInfoClassBook_GetNotes>;

  constructor(private studentInfoClassBookService: StudentInfoClassBookService, route: ActivatedRoute) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const studentClassBookId =
      tryParseInt(route.snapshot.paramMap.get('studentClassBookId')) ?? throwParamError('studentClassBookId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      studentInfoClassBookService.getNotes({
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
