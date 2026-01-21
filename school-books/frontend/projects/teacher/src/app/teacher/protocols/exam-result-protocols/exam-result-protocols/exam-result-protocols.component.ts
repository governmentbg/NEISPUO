import { Component, Inject } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  ExamResultProtocolsService,
  ExamResultProtocols_GetAll
} from 'projects/sb-api-client/src/api/examResultProtocols.service';
import { ExamResultProtocolTypeNomsService } from 'projects/sb-api-client/src/api/examResultProtocolTypeNoms.service';
import { ExamResultProtocolType } from 'projects/sb-api-client/src/model/examResultProtocolType';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { deepEqual, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { ExamResultProtocolsTypeDialogSkeletonComponent } from '../exam-result-protocols-type-dialog/exam-result-protocols-type-dialog.component';

@Component({
  selector: 'sb-exam-result-protocols',
  templateUrl: './exam-result-protocols.component.html'
})
export class ExamResultProtocolsComponent {
  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fasFileExcel = fasFileExcel;

  searchForm = this.fb.group({
    protocolType: null,
    orderNumber: null,
    protocolNumber: null,
    orderDate: null,
    protocolDate: null
  });

  dataSource: TableDataSource<ExamResultProtocols_GetAll>;
  examResultProtocolTypeNomsService: INomService<ExamResultProtocolType, { instId: number; schoolYear: number }>;
  canCreate = false;

  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    examResultProtocolsService: ExamResultProtocolsService,
    examResultProtocolTypeNomsService: ExamResultProtocolTypeNomsService,
    public route: ActivatedRoute,
    private router: Router,
    private fb: UntypedFormBuilder,
    private dialog: MatDialog
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      examResultProtocolsService.getAll({
        schoolYear,
        instId,
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

    this.examResultProtocolTypeNomsService = new NomServiceWithParams(examResultProtocolTypeNomsService, () => ({
      instId: instId,
      schoolYear: schoolYear
    }));

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    institutionInfo.then((institutionInfo) => {
      this.canCreate = institutionInfo.hasProtocolsCreateAccess;
    });
  }

  onClick() {
    const schoolYear = tryParseInt(this.route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(this.route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    openTypedDialog(this.dialog, ExamResultProtocolsTypeDialogSkeletonComponent, {
      data: {
        schoolYear: schoolYear,
        instId: instId
      }
    })
      .afterClosed()
      .toPromise()
      .then((type) => {
        if (type === ExamResultProtocolType.Exam) {
          this.router.navigate(['./new'], { relativeTo: this.route });
        } else if (type === ExamResultProtocolType.Qualification) {
          this.router.navigate(['./newQualification'], { relativeTo: this.route });
        } else if (type === ExamResultProtocolType.SkillsCheck) {
          this.router.navigate(['./newSkillsCheck'], { relativeTo: this.route });
        } else if (
          type === ExamResultProtocolType.QualificationAcquisition ||
          type === ExamResultProtocolType.QualificationAcquisitionExamGrades ||
          type === ExamResultProtocolType.QualificationAcquisitionStateExamGrades
        ) {
          this.router.navigate(['./newQualificationAcquisition', { type: type }], { relativeTo: this.route });
        } else if (type === ExamResultProtocolType.HighSchoolCertificate) {
          this.router.navigate(['./newHighSchoolCertificate'], { relativeTo: this.route });
        } else if (type === ExamResultProtocolType.GraduationThesisDefense) {
          this.router.navigate(['./newGraduationThesisDefense'], { relativeTo: this.route });
        }

        return Promise.resolve();
      });
  }
}
