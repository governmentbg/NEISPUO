import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPaperclip as farPaperclip } from '@fortawesome/pro-regular-svg-icons/faPaperclip';
import { faUsers as fasUsers } from '@fortawesome/pro-solid-svg-icons/faUsers';
import {
  StudentInfoBoardService,
  StudentInfoBoard_GetAll,
  StudentInfoBoard_GetMetadata
} from 'projects/sb-api-client/src/api/studentInfoBoard.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class StudentInfoBoardSkeletonComponent extends SkeletonComponentBase {
  constructor(studentInfoBoardService: StudentInfoBoardService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const status = route.snapshot.paramMap.get('status') ?? throwParamError('status');

    this.resolve(StudentInfoBoardComponent, {
      schoolYear,
      instId,
      status,
      metadata: studentInfoBoardService.getMetadata({
        schoolYear,
        instId
      })
    });
  }
}

@Component({
  selector: 'sb-student-info-board',
  templateUrl: './student-info-board.component.html'
})
export class StudentInfoBoardComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    status: string;
    metadata: StudentInfoBoard_GetMetadata;
  };

  dataSource!: TableDataSource<StudentInfoBoard_GetAll>;

  readonly farPaperclip = farPaperclip;
  readonly fasUsers = fasUsers;

  constructor(private studentInfoBoardService: StudentInfoBoardService) {}

  ngOnInit() {
    if (this.data.metadata.count || this.data.metadata.archivedCount) {
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        this.studentInfoBoardService.getAll({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          archived: this.data.status === 'archived',
          offset,
          limit: limit
        })
      );
    }
  }
}
