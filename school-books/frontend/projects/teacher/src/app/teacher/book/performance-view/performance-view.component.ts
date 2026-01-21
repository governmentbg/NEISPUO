import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { PerformancesService, Performances_Get } from 'projects/sb-api-client/src/api/performances.service';
import { PerformanceTypeNomsService } from 'projects/sb-api-client/src/api/performanceTypeNoms.service';
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
export class PerformanceViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    performancesService: PerformancesService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const performanceId = tryParseInt(route.snapshot.paramMap.get('performanceId'));

    if (performanceId) {
      this.resolve(PerformanceViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        performance: performancesService.get({
          schoolYear,
          instId,
          classBookId,
          performanceId: performanceId
        })
      });
    } else {
      this.resolve(PerformanceViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        performance: null
      });
    }
  }
}

@Component({
  selector: 'sb-performance-view',
  templateUrl: './performance-view.component.html'
})
export class PerformanceViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    performance: Performances_Get | null;
  };
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    performanceTypeId: [null, Validators.required],
    name: [null, Validators.required],
    description: [null, Validators.required],
    startDate: [null, Validators.required],
    endDate: [null, Validators.required],
    location: [null, Validators.required],
    studentAwards: [null]
  });

  canEdit = false;
  canRemove = false;
  removing = false;
  performanceTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private performancesService: PerformancesService,
    performanceTypeNomsService: PerformanceTypeNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {
    this.performanceTypeNomsService = new NomServiceWithParams(performanceTypeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));
  }

  ngOnInit() {
    if (this.data.performance != null) {
      this.form.setValue({
        performanceTypeId: this.data.performance.performanceTypeId,
        name: this.data.performance.name,
        description: this.data.performance.description,
        startDate: this.data.performance.startDate,
        endDate: this.data.performance.endDate,
        location: this.data.performance.location,
        studentAwards: this.data.performance.studentAwards
      });
    }

    this.canEdit = this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasEditPerformanceAccess;
    this.canRemove =
      this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasRemovePerformanceAccess;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const performance = {
      performanceTypeId: <number>value.performanceTypeId,
      name: <string>value.name,
      description: <string>value.description,
      startDate: <Date>value.startDate,
      endDate: <Date>value.endDate,
      location: <string>value.location,
      studentAwards: <string>value.studentAwards
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.performance == null) {
            return this.performancesService
              .createPerformance({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                createPerformanceCommand: performance
              })
              .toPromise()
              .then((newPerformanceId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newPerformanceId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              performanceId: this.data.performance.performanceId
            };
            return this.performancesService
              .update({
                updatePerformanceCommand: performance,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.performancesService.get(updateArgs).toPromise())
              .then((newPerformance) => {
                this.data.performance = newPerformance;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.performance) {
      throw new Error('onRemove requires a performance to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      performanceId: this.data.performance.performanceId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете изявата?',
        errorsMessage: 'Не може да изтриете изявата, защото:',
        httpAction: () => this.performancesService.remove(removeParams).toPromise()
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
