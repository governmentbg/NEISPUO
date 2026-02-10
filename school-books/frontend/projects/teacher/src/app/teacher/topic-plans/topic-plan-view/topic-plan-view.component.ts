import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faFileImport as fasFileImport } from '@fortawesome/pro-solid-svg-icons/faFileImport';
import { faListOl as fasListOl } from '@fortawesome/pro-solid-svg-icons/faListOl';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { BasicClassNomsService } from 'projects/sb-api-client/src/api/basicClassNoms.service';
import { SubjectNomsService } from 'projects/sb-api-client/src/api/subjectNoms.service';
import { SubjectTypeNomsService } from 'projects/sb-api-client/src/api/subjectTypeNoms.service';
import { TopicPlanItemsService, TopicPlanItems_GetItems } from 'projects/sb-api-client/src/api/topicPlanItems.service';
import { TopicPlanItemsExcelService } from 'projects/sb-api-client/src/api/topicPlanItemsExcel.service';
import { TopicPlanPublisherNomsService } from 'projects/sb-api-client/src/api/topicPlanPublisherNoms.service';
import { TopicPlansService, TopicPlans_Get } from 'projects/sb-api-client/src/api/topicPlans.service';
import { UpdateTopicPlanCommand } from 'projects/sb-api-client/src/model/updateTopicPlanCommand';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { distinctUntilChanged, takeUntil, tap } from 'rxjs/operators';
import { TopicPlanViewDialogSkeletonComponent } from '../topic-plan-view-dialog/topic-plan-view-dialog.component';
import { TopicPlanViewImportDialogComponent } from '../topic-plan-view-import-dialog/topic-plan-view-import-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class TopicPlanViewSkeletonComponent extends SkeletonComponentBase {
  constructor(topicPlansService: TopicPlansService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const topicPlanId = tryParseInt(route.snapshot.paramMap.get('topicPlanId'));

    if (topicPlanId) {
      this.resolve(TopicPlanViewComponent, {
        schoolYear,
        instId,
        topicPlan: topicPlansService.get({
          schoolYear,
          instId,
          topicPlanId: topicPlanId
        })
      });
    } else {
      this.resolve(TopicPlanViewComponent, {
        schoolYear,
        instId,
        topicPlan: null
      });
    }
  }
}

@Component({
  selector: 'sb-topic-plan-view',
  templateUrl: './topic-plan-view.component.html'
})
export class TopicPlanViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    topicPlan: TopicPlans_Get | null;
  };
  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasPencil = fasPencil;
  readonly fasListOl = fasListOl;
  readonly fasFileExcel = fasFileExcel;
  readonly fasFileImport = fasFileImport;
  readonly topicPlanPublisherOtherId = 999;
  downloadUrl?: string;
  nameChanged = false;
  itemsLength: number | null = null;

  readonly form = this.fb.group({
    basicClassId: [null],
    subjectId: [null],
    subjectTypeId: [null],
    topicPlanPublisherId: [null],
    topicPlanPublisherOther: [null],
    name: [null, Validators.required]
  });

  private readonly destroyed$ = new Subject<void>();

  editable = false;
  removing = false;
  removingAllItems = false;
  removingAct: { [key: number]: boolean } = {};
  topicPlanPublisherNomsService: INomService<number, { schoolYear: number; instId: number }>;
  basicClassNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectNomsService: INomService<number, { instId: number; schoolYear: number }>;
  subjectTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  dataSource!: TableDataSource<TopicPlanItems_GetItems>;

  constructor(
    private fb: UntypedFormBuilder,
    private topicPlansService: TopicPlansService,
    private topicPlanItemsService: TopicPlanItemsService,
    private topicPlanItemsExcelService: TopicPlanItemsExcelService,
    topicPlanPublisherNomsService: TopicPlanPublisherNomsService,
    basicClassNomsService: BasicClassNomsService,
    subjectNomsService: SubjectNomsService,
    subjectTypeNomsService: SubjectTypeNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const topicPlanId = tryParseInt(route.snapshot.paramMap.get('topicPlanId'));

    this.topicPlanPublisherNomsService = new NomServiceWithParams(topicPlanPublisherNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.basicClassNomsService = new NomServiceWithParams(basicClassNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));

    this.subjectNomsService = new NomServiceWithParams(subjectNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.subjectTypeNomsService = new NomServiceWithParams(subjectTypeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.topicPlanItemsService
        .getItems({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          topicPlanId: this.data.topicPlan!.topicPlanId,
          offset,
          limit
        })
        .pipe(
          tap((res) => {
            this.itemsLength = res.length;
          })
        )
    );

    topicPlanItemsExcelService
      .downloadExcelFile({
        schoolYear: schoolYear,
        instId: instId,
        topicPlanId: topicPlanId ?? -1
      })
      .toPromise()
      .then((url) => (this.downloadUrl = url));
  }

  ngOnInit() {
    const topicPlan = this.data.topicPlan;
    if (topicPlan != null) {
      this.form.setValue({
        name: topicPlan.name,
        basicClassId: topicPlan.basicClassId,
        subjectId: topicPlan.subjectId,
        subjectTypeId: topicPlan.subjectTypeId,
        topicPlanPublisherId: topicPlan.topicPlanPublisherId,
        topicPlanPublisherOther: topicPlan.topicPlanPublisherOther
      });
    } else {
      this.form.valueChanges
        .pipe(
          distinctUntilChanged(
            (oldVal: UpdateTopicPlanCommand, newVal: UpdateTopicPlanCommand) =>
              oldVal.basicClassId === newVal.basicClassId &&
              oldVal.subjectId === newVal.subjectId &&
              oldVal.subjectTypeId === newVal.subjectTypeId &&
              oldVal.topicPlanPublisherId === newVal.topicPlanPublisherId
          ),
          tap((val) => {
            if (!this.nameChanged && val.subjectId && val.subjectTypeId && val.topicPlanPublisherId) {
              let basicClassName: Promise<string | null>;
              if (val.basicClassId) {
                basicClassName = this.basicClassNomsService
                  .getNomsById({
                    instId: this.data.instId,
                    schoolYear: this.data.schoolYear,
                    ids: [val.basicClassId]
                  })
                  .toPromise()
                  .then((nom) => {
                    return nom[0].name;
                  });
              } else {
                basicClassName = Promise.resolve(null);
              }

              const subjectName = this.subjectNomsService
                .getNomsById({
                  instId: this.data.instId,
                  schoolYear: this.data.schoolYear,
                  ids: [val.subjectId]
                })
                .toPromise()
                .then((nom) => {
                  return nom[0].name;
                });

              const subjectTypeName = this.subjectTypeNomsService
                .getNomsById({
                  instId: this.data.instId,
                  schoolYear: this.data.schoolYear,
                  ids: [val.subjectTypeId]
                })
                .toPromise()
                .then((nom) => {
                  return nom[0].name;
                });

              const publisherName = this.topicPlanPublisherNomsService
                .getNomsById({
                  instId: this.data.instId,
                  schoolYear: this.data.schoolYear,
                  ids: [val.topicPlanPublisherId]
                })
                .toPromise()
                .then((nom) => {
                  return nom[0].name;
                });

              Promise.all([basicClassName, subjectName, subjectTypeName, publisherName]).then(
                ([basicClassName, subjectName, subjectTypeName, publisherName]) => {
                  let className = '';
                  if (basicClassName) {
                    const isRegularClass = Number(basicClassName) >= 1 && Number(basicClassName) <= 12;
                    className = `${basicClassName}${isRegularClass ? ' клас' : ''} `;
                  }
                  this.form.controls['name'].setValue(
                    `${subjectName.substring(0, 50)} / ${subjectTypeName} ${className}(${publisherName})`
                  );
                }
              );
            }
          }),
          takeUntil(this.destroyed$)
        )
        .subscribe();
    }
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  setNameChanged() {
    this.nameChanged = true;
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const topicPlan = {
      name: <string>value.name,
      basicClassId: <number | null>value.basicClassId,
      subjectId: <number | null>value.subjectId,
      subjectTypeId: <number | null>value.subjectTypeId,
      topicPlanPublisherId: <number | null>value.topicPlanPublisherId,
      topicPlanPublisherOther: <string | null>value.topicPlanPublisherOther
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.topicPlan == null) {
            return this.topicPlansService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createTopicPlanCommand: topicPlan
              })
              .toPromise()
              .then((newTopicPlanId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newTopicPlanId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              topicPlanId: this.data.topicPlan.topicPlanId
            };
            return this.topicPlansService
              .update({
                updateTopicPlanCommand: topicPlan,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.topicPlansService.get(updateArgs).toPromise())
              .then((newTopicPlan) => {
                this.data.topicPlan = newTopicPlan;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.topicPlan) {
      throw new Error('onRemove requires a topicPlan to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      topicPlanId: this.data.topicPlan.topicPlanId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете тематичното разпределение?',
        errorsMessage: 'Не може да изтриете тематичното разпределение, защото:',
        httpAction: () => this.topicPlansService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.router.navigate(['../'], { relativeTo: this.route });
        }
      })
      .finally(() => {
        this.removing = false;
      });
  }

  onRemoveItem(topicPlanItemId: number) {
    if (!this.data.topicPlan) {
      throw new Error('onRemoveItem requires its topicPlan to have been loaded.');
    }
    this.removingAct[topicPlanItemId] = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      topicPlanId: this.data.topicPlan.topicPlanId,
      topicPlanItemId: topicPlanItemId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете темата?',
        errorsMessage: 'Не може да изтриете темата, защото:',
        httpAction: () => this.topicPlanItemsService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.dataSource.reload();
        }
      })
      .finally(() => {
        this.removingAct[topicPlanItemId] = false;
      });
  }

  onRemoveAllItems() {
    if (!this.data.topicPlan) {
      throw new Error('onRemove requires a topicPlan to have been loaded.');
    }
    this.removingAllItems = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      topicPlanId: this.data.topicPlan.topicPlanId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете всички теми от тематичното разпределение?',
        errorsMessage: 'Не може да изтриете всички теми от тематичното разпределение, защото:',
        httpAction: () => this.topicPlanItemsService.removeAll(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.dataSource.reload();
        }
      })
      .finally(() => {
        this.removingAllItems = false;
      });
  }

  openAddOrUpdateItemDialog(topicPlanItemId: number | null) {
    if (!this.data.topicPlan) {
      throw new Error('onAddOrUpdate an topic requires a topicPlan to have been loaded.');
    }
    openTypedDialog(this.dialog, TopicPlanViewDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        topicPlanId: this.data.topicPlan.topicPlanId,
        topicPlanItemId: topicPlanItemId
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.dataSource.reload();
        }

        return Promise.resolve();
      });
  }

  openImportFromExcelDialog(topicPlanId: number) {
    if (!this.data.topicPlan) {
      throw new Error('import requires a topicPlan to have been loaded.');
    }
    openTypedDialog(this.dialog, TopicPlanViewImportDialogComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        topicPlanId: this.data.topicPlan.topicPlanId
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.dataSource.reload();
        }

        return Promise.resolve();
      });
  }
}
