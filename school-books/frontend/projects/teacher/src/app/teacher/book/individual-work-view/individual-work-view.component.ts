import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBookStudentNomsService } from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import { IndividualWorksService, IndividualWorks_Get } from 'projects/sb-api-client/src/api/individualWorks.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class IndividualWorkViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    individualWorksService: IndividualWorksService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const individualWorkId = tryParseInt(route.snapshot.paramMap.get('individualWorkId'));

    if (individualWorkId) {
      this.resolve(IndividualWorkViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        individualWork: individualWorksService.get({
          schoolYear,
          instId,
          classBookId,
          individualWorkId: individualWorkId
        })
      });
    } else {
      this.resolve(IndividualWorkViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        individualWork: null
      });
    }
  }
}

@Component({
  selector: 'sb-individual-work-view',
  templateUrl: './individual-work-view.component.html'
})
export class IndividualWorkViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    individualWork: IndividualWorks_Get | null;
  };
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    studentId: [null, Validators.required],
    date: [null, Validators.required],
    individualWorkActivity: [null, Validators.required]
  });

  removing = false;
  classBookStudentNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private individualWorksService: IndividualWorksService,
    classBookStudentNomsService: ClassBookStudentNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {
    this.classBookStudentNomsService = new NomServiceWithParams(classBookStudentNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    }));
  }

  ngOnInit() {
    if (this.data.individualWork != null) {
      this.form.setValue({
        studentId: this.data.individualWork.personId,
        date: this.data.individualWork.date,
        individualWorkActivity: this.data.individualWork.individualWorkActivity
      });

      this.canEdit =
        this.data.classBookInfo.bookAllowsModifications &&
        (this.data.classBookInfo.hasEditIndividualWorkAccess || this.data.individualWork.hasCreatorAccess);
      this.canRemove =
        this.data.classBookInfo.bookAllowsModifications &&
        (this.data.classBookInfo.hasRemoveIndividualWorkAccess || this.data.individualWork.hasCreatorAccess);
    }
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const individualWork = {
      personId: <number>value.studentId,
      date: <Date>value.date,
      individualWorkActivity: <string>value.individualWorkActivity
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.individualWork == null) {
            return this.individualWorksService
              .createIndividualWork({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                createIndividualWorkCommand: individualWork
              })
              .toPromise()
              .then((newIndividualWorkId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newIndividualWorkId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              individualWorkId: this.data.individualWork.individualWorkId
            };
            return this.individualWorksService
              .update({
                updateIndividualWorkCommand: {
                  date: individualWork.date,
                  individualWorkActivity: individualWork.individualWorkActivity
                },
                ...updateArgs
              })
              .toPromise()
              .then(() => this.individualWorksService.get(updateArgs).toPromise())
              .then((newIndividualWork) => {
                this.data.individualWork = newIndividualWork;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.individualWork) {
      throw new Error('onRemove requires a individualWork to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      individualWorkId: this.data.individualWork.individualWorkId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете индивидуалната работа?',
        errorsMessage: 'Не може да изтриете индивидуалната работа, защото:',
        httpAction: () => this.individualWorksService.remove(removeParams).toPromise()
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
}
