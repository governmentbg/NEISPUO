import { Component, Inject } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  ExamDutyProtocolsService,
  ExamDutyProtocols_GetAll
} from 'projects/sb-api-client/src/api/examDutyProtocols.service';
import { ExamDutyProtocolTypeNomsService } from 'projects/sb-api-client/src/api/examDutyProtocolTypeNoms.service';
import { ExamDutyProtocolType } from 'projects/sb-api-client/src/model/examDutyProtocolType';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { deepEqual, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { ExamDutyProtocolsTypeDialogSkeletonComponent } from '../exam-duty-protocols-type-dialog/exam-duty-protocols-type-dialog.component';

@Component({
  selector: 'sb-exam-duty-protocols',
  templateUrl: './exam-duty-protocols.component.html'
})
export class ExamDutyProtocolsComponent {
  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fasFileExcel = fasFileExcel;

  searchForm = this.fb.group({
    protocolType: null,
    orderNumber: null,
    orderDate: null,
    protocolDate: null
  });

  dataSource: TableDataSource<ExamDutyProtocols_GetAll>;
  examDutyProtocolTypeNomsService: INomService<ExamDutyProtocolType, { instId: number; schoolYear: number }>;
  canCreate = false;

  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    examDutyProtocolsService: ExamDutyProtocolsService,
    examDutyProtocolTypeNomsService: ExamDutyProtocolTypeNomsService,
    public route: ActivatedRoute,
    private router: Router,
    private fb: UntypedFormBuilder,
    private dialog: MatDialog
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      examDutyProtocolsService.getAll({
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

    this.examDutyProtocolTypeNomsService = new NomServiceWithParams(examDutyProtocolTypeNomsService, () => ({
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

    openTypedDialog(this.dialog, ExamDutyProtocolsTypeDialogSkeletonComponent, {
      data: {
        schoolYear: schoolYear,
        instId: instId
      }
    })
      .afterClosed()
      .toPromise()
      .then((type) => {
        if (type === ExamDutyProtocolType.State) {
          this.router.navigate(['./newState'], { relativeTo: this.route });
        } else if (type === ExamDutyProtocolType.SkillsCheck) {
          this.router.navigate(['./newSkillsCheck'], { relativeTo: this.route });
        } else if (type === ExamDutyProtocolType.Nvo) {
          this.router.navigate(['./newNvo'], { relativeTo: this.route });
        } else if (type === ExamDutyProtocolType.Exam) {
          this.router.navigate(['./new'], { relativeTo: this.route });
        }

        return Promise.resolve();
      });
  }
}
