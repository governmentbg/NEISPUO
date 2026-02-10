import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { InstNomsService } from 'projects/sb-api-client/src/api/instNoms.service';
import {
  ReplrParticipationsService,
  ReplrParticipations_Get
} from 'projects/sb-api-client/src/api/replrParticipations.service';
import { ReplrParticipationTypeNomsService } from 'projects/sb-api-client/src/api/replrParticipationTypeNoms.service';
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
export class ReplrParticipationViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    replrParticipationsService: ReplrParticipationsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const replrParticipationId = tryParseInt(route.snapshot.paramMap.get('replrParticipationId'));

    if (replrParticipationId) {
      this.resolve(ReplrParticipationViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        replrParticipation: replrParticipationsService.get({
          schoolYear,
          instId,
          classBookId,
          replrParticipationId: replrParticipationId
        })
      });
    } else {
      this.resolve(ReplrParticipationViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        replrParticipation: null
      });
    }
  }
}

@Component({
  selector: 'sb-replr-participation-view',
  templateUrl: './replr-participation-view.component.html'
})
export class ReplrParticipationViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    replrParticipation: ReplrParticipations_Get | null;
  };
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    replrParticipationTypeId: [null, Validators.required],
    date: [null, Validators.required],
    topic: [null],
    institutionId: [null],
    attendees: [null, Validators.required]
  });

  canEdit = false;
  canRemove = false;
  removing = false;
  replrParticipationTypeNomsService: INomService<number, { instId: number; schoolYear: number }>;
  instNomsService: INomService<number, { instId: number; schoolYear: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private replrParticipationsService: ReplrParticipationsService,
    replrParticipationTypeNomsService: ReplrParticipationTypeNomsService,
    instNomsService: InstNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {
    this.replrParticipationTypeNomsService = new NomServiceWithParams(replrParticipationTypeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));
    this.instNomsService = new NomServiceWithParams(instNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));
  }

  ngOnInit() {
    if (this.data.replrParticipation != null) {
      this.form.setValue({
        replrParticipationTypeId: this.data.replrParticipation.replrParticipationTypeId,
        date: this.data.replrParticipation.date,
        topic: this.data.replrParticipation.topic,
        institutionId: this.data.replrParticipation.instId,
        attendees: this.data.replrParticipation.attendees
      });
    }

    this.canEdit =
      this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasEditReplrParticipationAccess;
    this.canRemove =
      this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasRemoveReplrParticipationAccess;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const replrParticipation = {
      replrParticipationTypeId: <number>value.replrParticipationTypeId,
      date: <Date>value.date,
      topic: <string>value.topic,
      institutionId: <number>value.institutionId,
      attendees: <string>value.attendees
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.replrParticipation == null) {
            return this.replrParticipationsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                createReplrParticipationCommand: replrParticipation
              })
              .toPromise()
              .then((newReplrParticipationId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newReplrParticipationId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              replrParticipationId: this.data.replrParticipation.replrParticipationId
            };
            return this.replrParticipationsService
              .update({
                updateReplrParticipationCommand: replrParticipation,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.replrParticipationsService.get(updateArgs).toPromise())
              .then((newReplrParticipation) => {
                this.data.replrParticipation = newReplrParticipation;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.replrParticipation) {
      throw new Error('onRemove requires a replrParticipation to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      replrParticipationId: this.data.replrParticipation.replrParticipationId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете участието?',
        errorsMessage: 'Не може да изтриете участието, защото:',
        httpAction: () => this.replrParticipationsService.remove(removeParams).toPromise()
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
