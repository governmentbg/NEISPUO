import { Component, Inject } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { AdmProtocolTypeNomsService } from 'projects/sb-api-client/src/api/admProtocolTypeNoms.service';
import {
  StateExamsAdmProtocolsService,
  StateExamsAdmProtocols_GetAll
} from 'projects/sb-api-client/src/api/stateExamsAdmProtocols.service';
import { AdmProtocolType } from 'projects/sb-api-client/src/model/admProtocolType';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { deepEqual, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { AdmProtocolsTypeDialogSkeletonComponent } from './adm-protocols-type-dialog.component';

@Component({
  selector: 'sb-adm-protocols',
  templateUrl: './adm-protocols.component.html'
})
export class AdmProtocolsComponent {
  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;

  searchForm = this.fb.group({
    protocolType: null,
    protocolSchoolYear: null,
    protocolNum: null,
    protocolDate: null
  });

  dataSource: TableDataSource<StateExamsAdmProtocols_GetAll>;
  admProtocolTypeNomsService: INomService<AdmProtocolType, { instId: number; schoolYear: number }>;
  canCreate = false;

  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    stateExamsAdmProtocolsService: StateExamsAdmProtocolsService,
    admProtocolTypeNomsService: AdmProtocolTypeNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog,
    private fb: UntypedFormBuilder
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      stateExamsAdmProtocolsService.getAll({
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

    this.admProtocolTypeNomsService = new NomServiceWithParams(admProtocolTypeNomsService, () => ({
      instId: instId,
      schoolYear: schoolYear
    }));

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    institutionInfo.then((institutionInfo) => {
      this.canCreate = institutionInfo.hasProtocolsCreateAccess;
    });
  }

  getProtocolTypeSegment(type: AdmProtocolType): string {
    return type === AdmProtocolType.StateExams ? 'stateExams' : 'gradeChangeExams';
  }

  openProtocolTypeDialog() {
    const schoolYear = tryParseInt(this.route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(this.route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    openTypedDialog(this.dialog, AdmProtocolsTypeDialogSkeletonComponent, {
      data: {
        schoolYear: schoolYear,
        instId: instId
      }
    })
      .afterClosed()
      .toPromise()
      .then((type) => {
        if (type) {
          this.router.navigate([`./${this.getProtocolTypeSegment(type)}/new`], { relativeTo: this.route });
        }

        return Promise.resolve();
      });
  }
}
