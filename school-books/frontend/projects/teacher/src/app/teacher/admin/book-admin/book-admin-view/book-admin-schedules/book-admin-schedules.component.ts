import { Component, Inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faClone as farClone } from '@fortawesome/pro-regular-svg-icons/faClone';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { SchedulesService, Schedules_GetAll } from 'projects/sb-api-client/src/api/schedules.service';
import { InstType } from 'projects/sb-api-client/src/model/instType';
import {
  ElementType,
  mapTableResult,
  TableDataSource,
  TableResult
} from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { formatDate } from 'projects/shared/utils/date';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { notEmpty, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { map } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { ClassBookAdminInfoType, CLASS_BOOK_ADMIN_INFO } from '../book-admin-view.component';
import { ScheduleDialogSkeletonComponent } from './schedule-dialog/schedule-dialog.component';
import { ScheduleIndividualDialogSkeletonComponent } from './schedule-individual-dialog/schedule-individual-dialog.component';
import { SplitScheduleDialogSkeletonComponent } from './split-schedule-dialog/split-schedule-dialog.component';

type MappedSchedule = Omit<ElementType<Schedules_GetAll>, 'dates'> & {
  dates: string;
};

@Component({
  selector: 'sb-book-admin-schedules',
  templateUrl: './book-admin-schedules.component.html'
})
export class BookAdminSchedulesComponent {
  dataSource: TableDataSource<TableResult<MappedSchedule>>;
  schoolYear!: number;
  instId!: number;
  classBookId!: number;
  isIndividualSchedule!: boolean;

  readonly fasPlus = fasPlus;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly farClone = farClone;
  readonly removing: { [key: number]: boolean } = {};
  displayedColumns: string[] = [];
  canEdit = false;
  canCreate = false;
  isDG = false;

  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    @Inject(CLASS_BOOK_ADMIN_INFO) classBookAdminInfo: Promise<ClassBookAdminInfoType>,
    route: ActivatedRoute,
    private schedulesService: SchedulesService,
    private dialog: MatDialog,
    private actionService: ActionService
  ) {
    this.schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    this.instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    this.classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    this.isIndividualSchedule = route.snapshot.data.isIndividualSchedule as boolean;

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      schedulesService
        .getAll({
          schoolYear: this.schoolYear,
          instId: this.instId,
          classBookId: this.classBookId,
          isIndividualSchedule: this.isIndividualSchedule,
          offset,
          limit
        })
        .pipe(
          map((r) =>
            mapTableResult(r, (s) => ({
              ...s,
              dates: s.dates
                .map((d) =>
                  d.startDate.getTime() === d.endDate.getTime()
                    ? formatDate(d.startDate)
                    : `${formatDate(d.startDate)} - ${formatDate(d.endDate)}`
                )
                .join(', ')
            }))
          )
        )
    );

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    Promise.all([institutionInfo, classBookAdminInfo]).then(([institutionInfo, classBookAdminInfo]) => {
      this.isDG = institutionInfo.instType === InstType.DG;
      this.canCreate = classBookAdminInfo.bookAllowsModifications && classBookAdminInfo.hasCreateScheduleAccess;
      this.canEdit = classBookAdminInfo.bookAllowsModifications && classBookAdminInfo.hasEditScheduleAccess;

      this.displayedColumns = !this.isIndividualSchedule
        ? [this.canEdit ? 'action' : null, 'term', 'shiftName', 'dates'].filter(notEmpty)
        : [this.canEdit ? 'action' : null, 'studentNames', 'term', 'shiftName', 'dates'].filter(notEmpty);
    });
  }

  openScheduleDialog(scheduleId: number) {
    openTypedDialog(this.dialog, ScheduleDialogSkeletonComponent, {
      disableClose: true,
      data: {
        schoolYear: this.schoolYear,
        instId: this.instId,
        classBookId: this.classBookId,
        isIndividualSchedule: this.isIndividualSchedule,
        personId: null,
        scheduleId,
        isDG: this.isDG
      }
    })
      .afterClosed()
      .toPromise()
      .then((result) => {
        if (result) {
          this.dataSource.reload();
        }
      });
  }

  openSplitScheduleDialog(scheduleId: number) {
    openTypedDialog(this.dialog, SplitScheduleDialogSkeletonComponent, {
      data: {
        schoolYear: this.schoolYear,
        instId: this.instId,
        classBookId: this.classBookId,
        scheduleId
      }
    })
      .afterClosed()
      .toPromise()
      .then((result) => {
        if (result) {
          this.dataSource.reload();
        }
      });
  }

  onNew() {
    if (!this.isIndividualSchedule) {
      openTypedDialog(this.dialog, ScheduleDialogSkeletonComponent, {
        disableClose: true,
        data: {
          schoolYear: this.schoolYear,
          instId: this.instId,
          classBookId: this.classBookId,
          isIndividualSchedule: this.isIndividualSchedule,
          personId: null,
          scheduleId: null,
          isDG: this.isDG
        }
      })
        .afterClosed()
        .toPromise()
        .then((result) => {
          if (result) {
            this.dataSource.reload();
          }
        });
    } else {
      openTypedDialog(this.dialog, ScheduleIndividualDialogSkeletonComponent, {
        data: {
          schoolYear: this.schoolYear,
          instId: this.instId,
          classBookId: this.classBookId
        }
      })
        .afterClosed()
        .toPromise()
        .then((studentId) => {
          if (!studentId) return;

          openTypedDialog(this.dialog, ScheduleDialogSkeletonComponent, {
            disableClose: true,
            data: {
              schoolYear: this.schoolYear,
              instId: this.instId,
              classBookId: this.classBookId,
              isIndividualSchedule: this.isIndividualSchedule,
              personId: studentId,
              scheduleId: null,
              isDG: this.isDG
            }
          })
            .afterClosed()
            .toPromise()
            .then((result) => {
              if (result) {
                this.dataSource.reload();
              }
            });
        });
    }
  }

  onRemove(scheduleId: number) {
    this.removing[scheduleId] = true;

    const removeParams = {
      schoolYear: this.schoolYear,
      instId: this.instId,
      classBookId: this.classBookId,
      scheduleId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете разписанието?',
        errorsMessage: 'Не може да изтриете разписанието, защото:',
        httpAction: () => this.schedulesService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.dataSource.reload();
        }
      })
      .finally(() => {
        this.removing[scheduleId] = false;
      });
  }
}
