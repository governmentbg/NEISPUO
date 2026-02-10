import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBookStudentNomsService } from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import { SanctionsService, Sanctions_Get } from 'projects/sb-api-client/src/api/sanctions.service';
import { SanctionTypeNomsService } from 'projects/sb-api-client/src/api/sanctionTypeNoms.service';
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
export class SanctionViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    sanctionsService: SanctionsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const sanctionId = tryParseInt(route.snapshot.paramMap.get('sanctionId'));

    if (sanctionId) {
      this.resolve(SanctionViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        sanction: sanctionsService.get({
          schoolYear,
          instId,
          classBookId,
          sanctionId: sanctionId
        })
      });
    } else {
      this.resolve(SanctionViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        sanction: null
      });
    }
  }
}

@Component({
  selector: 'sb-sanction-view',
  templateUrl: './sanction-view.component.html'
})
export class SanctionViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    sanction: Sanctions_Get | null;
  };
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    studentId: [null, Validators.required],
    sanctionTypeId: [null, Validators.required],
    orderNumber: [null, Validators.required],
    orderDate: [null, Validators.required],
    startDate: [null, Validators.required],
    endDate: [null],
    description: [null],
    cancelOrderNumber: [null],
    cancelOrderDate: [null],
    cancelReason: [null],
    ruoOrderNumber: [null],
    ruoOrderDate: [null]
  });

  canEdit = false;
  canRemove = false;
  removing = false;
  classBookStudentNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;
  sanctionTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private sanctionsService: SanctionsService,
    classBookStudentNomsService: ClassBookStudentNomsService,
    sanctionTypeNomsService: SanctionTypeNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {
    this.classBookStudentNomsService = new NomServiceWithParams(classBookStudentNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    }));

    this.sanctionTypeNomsService = new NomServiceWithParams(sanctionTypeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));
  }

  ngOnInit() {
    if (this.data.sanction != null) {
      this.form.setValue({
        studentId: this.data.sanction.personId,
        sanctionTypeId: this.data.sanction.sanctionTypeId,
        orderNumber: this.data.sanction.orderNumber,
        orderDate: this.data.sanction.orderDate,
        startDate: this.data.sanction.startDate,
        endDate: this.data.sanction.endDate,
        description: this.data.sanction.description,
        cancelOrderNumber: this.data.sanction.cancelOrderNumber,
        cancelOrderDate: this.data.sanction.cancelOrderDate,
        cancelReason: this.data.sanction.cancelReason,
        ruoOrderNumber: this.data.sanction.ruoOrderNumber,
        ruoOrderDate: this.data.sanction.ruoOrderDate
      });
    }

    this.canEdit = this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasEditSanctionAccess;
    this.canRemove = this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasRemoveSanctionAccess;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const sanction = {
      personId: <number>value.studentId,
      sanctionTypeId: <number>value.sanctionTypeId,
      orderNumber: <string>value.orderNumber,
      orderDate: <Date>value.orderDate,
      startDate: <Date>value.startDate,
      endDate: <Date>value.endDate,
      description: <string>value.description,
      cancelOrderNumber: <string>value.cancelOrderNumber,
      cancelOrderDate: <Date>value.cancelOrderDate,
      cancelReason: <string>value.cancelReason,
      ruoOrderNumber: <string>value.ruoOrderNumber,
      ruoOrderDate: <Date>value.ruoOrderDate
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.sanction == null) {
            return this.sanctionsService
              .createSanction({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                createSanctionCommand: sanction
              })
              .toPromise()
              .then((newSanctionId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newSanctionId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              sanctionId: this.data.sanction.sanctionId
            };
            return this.sanctionsService
              .update({
                updateSanctionCommand: sanction,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.sanctionsService.get(updateArgs).toPromise())
              .then((newSanction) => {
                this.data.sanction = newSanction;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.sanction) {
      throw new Error('onRemove requires a sanction to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      sanctionId: this.data.sanction.sanctionId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете санкцията?',
        errorsMessage: 'Не може да изтриете санкцията, защото:',
        httpAction: () => this.sanctionsService.remove(removeParams).toPromise()
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
