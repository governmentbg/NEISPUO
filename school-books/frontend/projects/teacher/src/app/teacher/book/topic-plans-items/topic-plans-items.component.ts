import { Component, Inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faSpinnerThird as fadSpinnerThird } from '@fortawesome/pro-duotone-svg-icons/faSpinnerThird';
import { faTrashAlt as fadTrashAlt } from '@fortawesome/pro-duotone-svg-icons/faTrashAlt';
import { faCircle as farCircle } from '@fortawesome/pro-regular-svg-icons/faCircle';
import { faCheckCircle as fasCheckCircle } from '@fortawesome/pro-solid-svg-icons/faCheckCircle';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faFileImport as fasFileImport } from '@fortawesome/pro-solid-svg-icons/faFileImport';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBooks_GetCurriculums } from 'projects/sb-api-client/src/api/classBooks.service';
import {
  ClassBookTopicPlanItemsService,
  ClassBookTopicPlanItems_GetAll
} from 'projects/sb-api-client/src/api/classBookTopicPlanItems.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { of } from 'rxjs';
import { delay, tap } from 'rxjs/operators';
import { CLASS_BOOK_INFO } from '../book/book.component';
import { TopicPlansImportDialogComponent } from '../topic-plans-import-dialog/topic-plans-import-dialog.component';
import { TopicPlansItemDialogSkeletonComponent } from '../topic-plans-item-dialog/topic-plans-item-dialog.component';
import { TopicPlansLoadDialogComponent } from '../topic-plans-load-dialog/topic-plans-load-dialog.component';
import { CLASS_BOOK_TOPIC_PLANS_CURRICULUMS } from '../topic-plans/topic-plans.component';

@Component({
  selector: 'sb-topic-plans-items',
  templateUrl: './topic-plans-items.component.html'
})
export class TopicPlansItemsComponent {
  private schoolYear: number;
  private instId: number;
  private classBookId: number;
  private curriculumId: number;
  readonly fasPlus = fasPlus;
  readonly fasPencil = fasPencil;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fadTrashAlt = fadTrashAlt;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fadSpinnerThird = fadSpinnerThird;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasFileExcel = fasFileExcel;
  readonly fasFileImport = fasFileImport;
  readonly farCircle = farCircle;
  readonly fasCheckCircle = fasCheckCircle;
  canCreate = false;
  canRemove = false;
  canEdit = false;
  downloadUrl?: string;
  removing = false;
  removingAct: { [key: number]: boolean } = {};
  itemsLength: number | null = null;

  dataSource: TableDataSource<ClassBookTopicPlanItems_GetAll>;

  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    @Inject(CLASS_BOOK_TOPIC_PLANS_CURRICULUMS) curriculums: Promise<ClassBooks_GetCurriculums>,
    public classBookTopicPlanItemsService: ClassBookTopicPlanItemsService,
    route: ActivatedRoute,
    private actionService: ActionService,
    private dialog: MatDialog
  ) {
    this.schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    this.instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    this.classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    this.curriculumId = tryParseInt(route.snapshot.paramMap.get('curriculumId')) ?? throwParamError('curriculumId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      classBookTopicPlanItemsService
        .getAll({
          schoolYear: this.schoolYear,
          instId: this.instId,
          classBookId: this.classBookId,
          curriculumId: this.curriculumId,
          offset,
          limit
        })
        .pipe(
          tap((res) => {
            this.itemsLength = res.length;
          })
        )
    );

    classBookTopicPlanItemsService
      .downloadExcelFile({
        schoolYear: this.schoolYear,
        instId: this.instId,
        classBookId: this.classBookId,
        curriculumId: this.curriculumId
      })
      .toPromise()
      .then((url) => (this.downloadUrl = url));

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    Promise.all([classBookInfo, curriculums]).then(([classBookInfo, curriculums]) => {
      const curriculum =
        curriculums.find((c) => c.curriculumId === this.curriculumId) ?? throwError('curriculum not found');
      this.canCreate = classBookInfo.bookAllowsModifications && curriculum.hasCreateTopicPlanAccess;
      this.canRemove = classBookInfo.bookAllowsModifications && curriculum.hasCreateTopicPlanAccess;
      this.canEdit = classBookInfo.bookAllowsModifications && curriculum.hasCreateTopicPlanAccess;
    });
  }

  openAddOrUpdateItemDialog(classBookTopicPlanItemId: number | null) {
    openTypedDialog(this.dialog, TopicPlansItemDialogSkeletonComponent, {
      data: {
        schoolYear: this.schoolYear,
        instId: this.instId,
        classBookId: this.classBookId,
        curriculumId: this.curriculumId,
        classBookTopicPlanItemId
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          this.dataSource.reloadPage();
        }
      });
  }

  onRemove() {
    this.removing = true;

    const removeParams = {
      schoolYear: this.schoolYear,
      instId: this.instId,
      classBookId: this.classBookId,
      curriculumId: this.curriculumId
    };

    this.actionService
      .execute({
        confirmMessage:
          'Сигурни ли сте, че искате да премахнете тематичното разпределение? Въведените теми в часовете ще останат, но ще се загуби връзката с това, кои теми в тематичното разпределение са маркирани като взети, дори и да го заредите отново.',
        errorsMessage: 'Не може да премахнете тематичното разпределение защото:',
        httpAction: () => this.classBookTopicPlanItemsService.removeAll(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.dataSource.reload();
        }
      })
      .finally(() => {
        this.removing = false;
      });
  }

  onRemoveItem(classBookTopicPlanItemId: number) {
    this.removingAct[classBookTopicPlanItemId] = true;
    const removeParams = {
      schoolYear: this.schoolYear,
      instId: this.instId,
      classBookId: this.classBookId,
      curriculumId: this.curriculumId,
      classBookTopicPlanItemId: classBookTopicPlanItemId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете темата?',
        errorsMessage: 'Не може да изтриете темата, защото:',
        httpAction: () => this.classBookTopicPlanItemsService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.dataSource.reloadPage();
        }
      })
      .finally(() => {
        this.removingAct[classBookTopicPlanItemId] = false;
      });
  }

  onSetTakenItem(row: any, classBookTopicPlanItemId: number, taken: boolean) {
    row.taking = of(true).pipe(delay(1000));
    const updateTakenParams = {
      schoolYear: this.schoolYear,
      instId: this.instId,
      classBookId: this.classBookId,
      curriculumId: this.curriculumId,
      classBookTopicPlanItemId: classBookTopicPlanItemId,
      updateTakenClassBookTopicPlanItemCommand: { taken }
    };

    this.actionService
      .execute({
        httpAction: () => this.classBookTopicPlanItemsService.updateTaken(updateTakenParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.dataSource.reloadPage(true);
        } else {
          row.taking = null;
        }
      });
  }

  openLoadFromTopicPlanDialog() {
    openTypedDialog(this.dialog, TopicPlansLoadDialogComponent, {
      data: {
        schoolYear: this.schoolYear,
        instId: this.instId,
        classBookId: this.classBookId,
        curriculumId: this.curriculumId
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          this.dataSource.reload();
        }
      });
  }

  openImportFromExcelDialog() {
    openTypedDialog(this.dialog, TopicPlansImportDialogComponent, {
      data: {
        schoolYear: this.schoolYear,
        instId: this.instId,
        classBookId: this.classBookId,
        curriculumId: this.curriculumId
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          this.dataSource.reload();
        }
      });
  }
}
