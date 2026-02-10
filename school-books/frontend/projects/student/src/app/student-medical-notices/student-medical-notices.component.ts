import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPaperclip as farPaperclip } from '@fortawesome/pro-regular-svg-icons/faPaperclip';
import { faUsers as fasUsers } from '@fortawesome/pro-solid-svg-icons/faUsers';
import {
  StudentMedicalNoticeService,
  StudentMedicalNotice_GetStudentMedicalNotices
} from 'projects/sb-api-client/src/api/studentMedicalNotice.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class StudentMedicalNoticesSkeletonComponent extends SkeletonComponentBase {
  constructor(studentMedicalNoticeService: StudentMedicalNoticeService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');

    this.resolve(StudentMedicalNoticesComponent, {
      schoolYear,
      personId,
      studentName: studentMedicalNoticeService.getStudentName({
        schoolYear,
        personId
      })
    });
  }
}

@Component({
  selector: 'sb-student-medical-notices',
  templateUrl: './student-medical-notices.component.html'
})
export class StudentMedicalNoticesComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    personId: number;
    studentName: string;
  };

  dataSource!: TableDataSource<StudentMedicalNotice_GetStudentMedicalNotices>;

  readonly farPaperclip = farPaperclip;
  readonly fasUsers = fasUsers;

  constructor(private studentMedicalNoticeService: StudentMedicalNoticeService) {}

  ngOnInit() {
    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.studentMedicalNoticeService.getStudentMedicalNotices({
        schoolYear: this.data.schoolYear,
        personId: this.data.personId,
        offset,
        limit: limit
      })
    );
  }
}
