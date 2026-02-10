import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faChalkboardTeacher as fadChalkboardTeacher } from '@fortawesome/pro-duotone-svg-icons/faChalkboardTeacher';
import { faCog as fadCog } from '@fortawesome/pro-duotone-svg-icons/faCog';
import { faAlignJustify as fasAlignJustify } from '@fortawesome/pro-solid-svg-icons/faAlignJustify';
import { faAlignSlash as fasAlignSlash } from '@fortawesome/pro-solid-svg-icons/faAlignSlash';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { isValid, parse } from 'date-fns';
import { MyHourService, MyHour_GetTeacherLesson } from 'projects/sb-api-client/src/api/myHour.service';
import { TopicsService } from 'projects/sb-api-client/src/api/topics.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { formatDate } from 'projects/shared/utils/date';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class AddTopicSkeletonComponent extends SkeletonComponentBase {
  constructor(route: ActivatedRoute, myHourService: MyHourService) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const scheduleLessonId =
      tryParseInt(route.snapshot.paramMap.get('scheduleLessonId')) ?? throwParamError('scheduleLessonId');
    const date = parse(route.snapshot.paramMap.get('date') ?? throwParamError('date'), 'yyyy-MM-dd', new Date(), {});
    if (!isValid(date)) {
      throwParamError('date');
    }

    this.resolve(AddTopicComponent, {
      schoolYear,
      instId,
      classBookId,
      date,
      scheduleLessonId,
      teacherLesson: myHourService.getTeacherLesson({
        schoolYear,
        instId,
        classBookId,
        scheduleLessonId
      })
    });
  }
}

@Component({
  selector: 'sb-add-topic',
  templateUrl: './add-topic.component.html'
})
export class AddTopicComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    date: Date;
    scheduleLessonId: number;
    teacherLesson: MyHour_GetTeacherLesson;
  };

  readonly fadChalkboardTeacher = fadChalkboardTeacher;
  readonly fadCog = fadCog;
  readonly fasAlignJustify = fasAlignJustify;
  readonly fasAlignSlash = fasAlignSlash;
  readonly fasCheck = fasCheck;

  readonly form = this.fb.group({
    title: ['', Validators.required],
    classBookTopicPlanItemIds: this.fb.control<number[] | null>(null, {
      validators: [Validators.required]
    })
  });

  useTopicPlan!: boolean;
  topicPlanItems$: Observable<Array<{ classBookTopicPlanItemId: number; title: string; taken: boolean }>> | null = null;
  topicPlanItemsLoading = false;
  hourDescriptionShort = '';
  hourDescription = '';

  constructor(
    public errorStateMatcher: ErrorStateMatcher,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private topicsService: TopicsService
  ) {}

  ngOnInit() {
    this.hourDescriptionShort = `${formatDate(this.data.date)} час #${this.data.teacherLesson.hourNumber}`;
    this.hourDescription = `${this.hourDescriptionShort} (${this.data.teacherLesson.startTime} - ${this.data.teacherLesson.endTime})`;

    this.useTopicPlan = this.data.teacherLesson.hasTopicPlan;
    this.setupFormControls();
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  get titleFormControl() {
    return this.form.get('title') ?? throwError();
  }

  get classBookTopicPlanItemIdsFormControl() {
    return this.form.get('classBookTopicPlanItemIds') ?? throwError();
  }

  toggleUseTopicPlan() {
    this.useTopicPlan = !this.useTopicPlan;
    this.setupFormControls();
  }

  setupFormControls() {
    if (this.useTopicPlan) {
      this.setupClassBookTopicPlanItemIdsFormControl();
    } else {
      this.setupTitleFormControl();
    }
  }

  setupTitleFormControl() {
    this.titleFormControl.enable();
    this.classBookTopicPlanItemIdsFormControl.disable();
  }

  setupClassBookTopicPlanItemIdsFormControl() {
    this.topicPlanItemsLoading = true;
    this.topicPlanItems$ = this.topicsService
      .getCurriculumTopiPlan({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        curriculumId: this.data.teacherLesson.curriculumId
      })
      .pipe(finalize(() => (this.topicPlanItemsLoading = false)));

    this.titleFormControl.disable();
    this.classBookTopicPlanItemIdsFormControl.enable();
  }

  onTopicPlanSelectOpened() {
    // the dropdown is not rendered yet
    setTimeout(() => {
      const takenIconElements = document.querySelectorAll('#ng-select-dropdown-default .ng-fa-icon.icon-taken');
      if (!takenIconElements.length) {
        return;
      }

      const lastTakenIconElement = takenIconElements[takenIconElements.length - 1];
      const lastTakenOptionOffsetTop = lastTakenIconElement.parentElement!.offsetTop;

      const scrollHost = document.querySelector('#ng-select-dropdown-default .ng-dropdown-panel-items.scroll-host');
      if (!scrollHost) {
        return;
      }

      scrollHost.scrollTo({ top: lastTakenOptionOffsetTop, behavior: 'smooth' });
    }, 0);
  }

  onSave(save: SaveToken) {
    const createTopicsCommand = {
      topics: [
        {
          ...this.form.value,
          date: this.data.date,
          scheduleLessonId: this.data.scheduleLessonId,
          teacherAbsenceId: this.data.teacherLesson.teacherAbsenceId
        }
      ]
    };

    this.actionService
      .execute({
        httpAction: () => {
          return this.topicsService
            .createTopics({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              createTopicsCommand
            })
            .toPromise()
            .then(() => {
              this.form.markAsPristine();
              this.router.navigate(['../'], { relativeTo: this.route });
            });
        }
      })
      .then((success) => save.done(success));
  }
}
