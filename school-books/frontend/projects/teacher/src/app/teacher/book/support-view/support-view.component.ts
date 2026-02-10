import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBookStudentNomsService } from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import { SupportDifficultyTypeNomsService } from 'projects/sb-api-client/src/api/supportDifficultyTypeNoms.service';
import {
  SupportsService,
  Supports_Get,
  Supports_GetActivityAll
} from 'projects/sb-api-client/src/api/supports.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { ALL_GROUP_ID } from 'projects/shared/components/nom-select/nom-select.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import { CLASS_BOOK_INFO } from '../book/book.component';
import { SupportViewDialogSkeletonComponent } from '../support-view-dialog/support-view-dialog.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class SupportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    supportsService: SupportsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const supportId = tryParseInt(route.snapshot.paramMap.get('supportId'));

    if (supportId) {
      this.resolve(SupportViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        support: supportsService.get({
          schoolYear,
          instId,
          classBookId,
          supportId: supportId
        })
      });
    } else {
      this.resolve(SupportViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        support: null
      });
    }
  }
}

@Component({
  selector: 'sb-support-view',
  templateUrl: './support-view.component.html'
})
export class SupportViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    support: Supports_Get | null;
  };
  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasPencil = fasPencil;

  readonly form = this.fb.group({
    studentIds: [[], Validators.required],
    supportDifficultyTypeIds: [null, Validators.required],
    description: [null],
    expectedResult: [null],
    endDate: [null, Validators.required],
    teacherIds: [null, Validators.required]
  });

  editable = false;
  canEdit = false;
  canRemove = false;
  removing = false;
  removingAct: { [key: number]: boolean } = {};
  classBookStudentNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;
  instTeacherNomsService: INomService<number, { schoolYear: number; instId: number }>;
  supportDifficultyTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  dataSource!: TableDataSource<Supports_GetActivityAll>;

  constructor(
    private fb: UntypedFormBuilder,
    private supportsService: SupportsService,
    classBookStudentNomsService: ClassBookStudentNomsService,
    instTeacherNomsService: InstTeacherNomsService,
    supportDifficultyTypeNomsService: SupportDifficultyTypeNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog
  ) {
    this.classBookStudentNomsService = new NomServiceWithParams(classBookStudentNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    }));

    this.supportDifficultyTypeNomsService = new NomServiceWithParams(supportDifficultyTypeNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));

    this.instTeacherNomsService = new NomServiceWithParams(instTeacherNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.supportsService.getActivityAll({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        supportId: this.data.support!.supportId,
        offset,
        limit
      })
    );
  }

  ngOnInit() {
    const support = this.data.support;
    if (support != null) {
      this.form.setValue({
        supportDifficultyTypeIds: support.supportDifficultyTypeIds,
        studentIds: !support.isForAllStudents ? support.studentIds : [ALL_GROUP_ID],
        description: support.description,
        expectedResult: support.expectedResult,
        endDate: support.endDate,
        teacherIds: support.teacherIds
      });

      this.canEdit = this.data.classBookInfo.bookAllowsModifications && support.hasEditAccess;
      this.canRemove = this.data.classBookInfo.bookAllowsModifications && support.hasRemoveAccess;
    }
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const isForAllStudents = (<Array<any>>value.studentIds).includes(ALL_GROUP_ID);
    const support = {
      supportDifficultyTypeIds: <number[]>value.supportDifficultyTypeIds,
      isForAllStudents: isForAllStudents,
      studentIds: !isForAllStudents ? <Array<number>>value.studentIds : [],
      description: <string | null>value.description,
      expectedResult: <string | null>value.expectedResult,
      endDate: <Date>value.endDate,
      teacherIds: <number[]>value.teacherIds
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.support == null) {
            return this.supportsService
              .createSupport({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                createSupportCommand: support
              })
              .toPromise()
              .then((newSupportId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newSupportId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              supportId: this.data.support.supportId
            };
            return this.supportsService
              .update({
                updateSupportCommand: support,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.supportsService.get(updateArgs).toPromise())
              .then((newSupport) => {
                this.data.support = newSupport;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.support) {
      throw new Error('onRemove requires a support to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      supportId: this.data.support.supportId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете подкрепата?',
        errorsMessage: 'Не може да изтриете подкрепата, защото:',
        httpAction: () => this.supportsService.remove(removeParams).toPromise()
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

  onRemoveActivity(supportActivityId: number) {
    if (!this.data.support) {
      throw new Error('onRemoveActivity requires its support to have been loaded.');
    }
    this.removingAct[supportActivityId] = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      supportId: this.data.support.supportId,
      supportActivityId: supportActivityId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете дейността?',
        errorsMessage: 'Не може да изтриете дейността, защото:',
        httpAction: () => this.supportsService.removeActivity(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          return this.dataSource.reload();
        }
      })
      .finally(() => {
        this.removingAct[supportActivityId] = false;
      });
  }

  openAddOrUpdateActivityDialog(activityId: number | null) {
    if (!this.data.support) {
      throw new Error('onAddOrUpdate an activity requires a support to have been loaded.');
    }
    openTypedDialog(this.dialog, SupportViewDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        supportId: this.data.support.supportId,
        supportActivityId: activityId
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
