import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ParentMeetingsService, ParentMeetings_Get } from 'projects/sb-api-client/src/api/parentMeetings.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { getTimeFormatHint, hourRegex } from 'projects/shared/utils/date';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class ParentMeetingViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    parentMeetingsService: ParentMeetingsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const parentMeetingId = tryParseInt(route.snapshot.paramMap.get('parentMeetingId'));
    if (parentMeetingId) {
      this.resolve(ParentMeetingViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        parentMeeting: parentMeetingsService.get({
          schoolYear,
          instId,
          classBookId,
          parentMeetingId
        })
      });
    } else {
      this.resolve(ParentMeetingViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        parentMeeting: null
      });
    }
  }
}

@Component({
  selector: 'sb-parent-meeting-view',
  templateUrl: './parent-meeting-view.component.html'
})
export class ParentMeetingViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    parentMeeting: ParentMeetings_Get | null;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly timeFormatHint = getTimeFormatHint();

  readonly form = this.fb.group({
    date: [null, Validators.required],
    startTime: [null, [Validators.required, Validators.pattern(hourRegex)]],
    location: [null],
    title: [null, Validators.required],
    description: [null]
  });

  canEdit = false;
  canRemove = false;
  removing = false;

  constructor(
    private fb: UntypedFormBuilder,
    private parentMeetingsService: ParentMeetingsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {}

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    if (this.data.parentMeeting != null) {
      const { date, startTime, location, title, description } = this.data.parentMeeting;
      this.form.setValue({ date, startTime, location, title, description });
    }

    this.canEdit =
      this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasEditParentMeetingAccess;
    this.canRemove =
      this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasRemoveParentMeetingAccess;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const parentMeeting = {
      date: <Date>value.date,
      startTime: <string>value.startTime,
      location: <string | null>value.location,
      title: <string>value.title,
      description: <string | null>value.description
    };

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.parentMeeting == null) {
            return this.parentMeetingsService
              .createParentMeeting({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                createParentMeetingCommand: parentMeeting
              })
              .toPromise()
              .then((newParentMeetingId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newParentMeetingId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              parentMeetingId: this.data.parentMeeting.parentMeetingId
            };
            return this.parentMeetingsService
              .update({
                updateParentMeetingCommand: parentMeeting,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.parentMeetingsService.get(updateArgs).toPromise())
              .then((newParentMeeting) => {
                this.data.parentMeeting = newParentMeeting;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.parentMeeting) {
      throw new Error('onRemove requires a parentMeeting to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      parentMeetingId: this.data.parentMeeting.parentMeetingId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете родителската среща?',
        errorsMessage: 'Не може да изтриете родителската среща, защото:',
        httpAction: () => this.parentMeetingsService.remove(removeParams).toPromise()
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
