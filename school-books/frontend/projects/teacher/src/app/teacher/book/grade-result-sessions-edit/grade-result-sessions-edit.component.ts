import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, UntypedFormArray, UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTimes as fasTimes } from '@fortawesome/pro-solid-svg-icons/faTimes';
import {
  GradeResultsService,
  GradeResults_GetSessionAllEdit
} from 'projects/sb-api-client/src/api/gradeResults.service';
import { UpdateGradeResultSessionsCommand } from 'projects/sb-api-client/src/model/updateGradeResultSessionsCommand';
import { UpdateGradeResultSessionsCommandSession } from 'projects/sb-api-client/src/model/updateGradeResultSessionsCommandSession';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { assert } from 'projects/shared/utils/assert';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { distinctUntilChanged, takeUntil, tap } from 'rxjs/operators';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class GradeResultSessionsEditSkeletonComponent extends SkeletonComponentBase {
  constructor(gradeResultsService: GradeResultsService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.resolve(GradeResultSessionsEditComponent, {
      schoolYear,
      instId,
      classBookId,
      sessions: gradeResultsService.getSessionAllEdit({
        schoolYear,
        instId,
        classBookId
      })
    });
  }
}

@Component({
  selector: 'sb-grade-result-sessions-edit',
  templateUrl: './grade-result-sessions-edit.component.html',
  styleUrls: ['./grade-result-sessions-edit.component.scss']
})
export class GradeResultSessionsEditComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    sessions: GradeResults_GetSessionAllEdit;
  };

  readonly form = this.fb.group({
    sessions: this.fb.array([])
  });
  private readonly destroyed$ = new Subject<void>();
  readonly fasArrowLeft = fasArrowLeft;
  readonly fasCheck = fasCheck;
  readonly fasPlus = fasPlus;
  readonly fasTimes = fasTimes;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fadChevronCircleRight = fadChevronCircleRight;

  sessions!: ReturnType<typeof mapSessions>;

  saveBtnDisabled = false;
  session1Show = true;
  session2Show = true;
  session3Show = true;

  constructor(
    private fb: UntypedFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private gradeResultsService: GradeResultsService,
    private actionService: ActionService
  ) {}

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit(): void {
    this.sessions = mapSessions(this.data);

    const sessionsForm = this.form.get('sessions') as UntypedFormArray;
    for (const session of this.data.sessions) {
      sessionsForm.push(
        this.fb.group({
          personId: [session.personId],
          curriculumId: [session.curriculumId],
          session1Grade: [session.session1Grade],
          session1NoShow: [session.session1NoShow],
          session2Grade: [session.session2Grade],
          session2NoShow: [session.session2NoShow],
          session3Grade: [session.session3Grade],
          session3NoShow: [session.session3NoShow]
        })
      );
    }

    this.checkShowColumns(this.data.sessions);

    this.form.valueChanges
      .pipe(
        distinctUntilChanged((oldVal: UpdateGradeResultSessionsCommand, newVal: UpdateGradeResultSessionsCommand) => {
          const oldSessions = oldVal.sessions as UpdateGradeResultSessionsCommandSession[];

          const hasNoChange = (newVal.sessions as UpdateGradeResultSessionsCommandSession[]).reduce(
            (acc, newSession, i) =>
              acc &&
              oldSessions[i].session1Grade === newSession.session1Grade &&
              oldSessions[i].session1NoShow === newSession.session1NoShow &&
              oldSessions[i].session2Grade === newSession.session2Grade &&
              oldSessions[i].session2NoShow === newSession.session2NoShow &&
              oldSessions[i].session3Grade === newSession.session3Grade &&
              oldSessions[i].session3NoShow === newSession.session3NoShow,
            true
          );
          return hasNoChange;
        }),
        tap((val) => {
          const formVal = <UpdateGradeResultSessionsCommand>val;
          assert(formVal.sessions != null);
          this.checkShowColumns(formVal.sessions);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    for (let i = 0; i < this.data.sessions.length; i++) {
      const session = this.sessionForm(i);
      assert(session != null);
      session.valueChanges
        .pipe(
          distinctUntilChanged(
            (oldVal: UpdateGradeResultSessionsCommandSession, newVal: UpdateGradeResultSessionsCommandSession) =>
              oldVal.session1Grade === newVal.session1Grade &&
              oldVal.session1NoShow === newVal.session1NoShow &&
              oldVal.session2Grade === newVal.session2Grade &&
              oldVal.session2NoShow === newVal.session2NoShow &&
              oldVal.session3Grade === newVal.session3Grade &&
              oldVal.session3NoShow === newVal.session3NoShow
          ),
          tap((val) => {
            const formControls = this.getFormControlsAt(i);
            this.resetPassiveFormControls(formControls);
          }, takeUntil(this.destroyed$))
        )
        .subscribe();
    }
  }

  sessionForm(i: number) {
    return (this.form.get('sessions') as UntypedFormArray).at(i) as UntypedFormGroup;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave() {
    if (this.form.invalid || this.saveBtnDisabled) {
      return;
    }

    this.saveBtnDisabled = true;

    this.actionService
      .execute({
        httpAction: () =>
          this.gradeResultsService
            .updateSessions({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              updateGradeResultSessionsCommand: <UpdateGradeResultSessionsCommand>this.form.value
            })
            .toPromise()
      })
      .then((done) => {
        if (done) {
          this.form.markAsPristine();
          this.router.navigate(['../'], { relativeTo: this.route });
        }
      })
      .finally(() => (this.saveBtnDisabled = false));
  }

  getFormControlsAt(i: number) {
    return (
      <{ [key: string]: AbstractControl }>(
        ((this.form.get('sessions') as UntypedFormArray).at(i) as UntypedFormGroup).controls
      ) ?? throwError('FormGroup controls are missing')
    );
  }

  checkShowColumns(sessions: UpdateGradeResultSessionsCommandSession[]) {
    this.session1Show = sessions.filter((s) => s.session1NoShow === false).length > 0;
    this.session2Show = sessions.filter((s) => s.session2NoShow === false).length > 0;
    this.session3Show = sessions.filter((s) => s.session3NoShow === false).length > 0;
  }

  resetControl(control: AbstractControl) {
    control.reset(null, { emitEvent: false });
  }

  resetPassiveFormControls(controls: { [key: string]: AbstractControl }) {
    if (
      (controls['session1NoShow'].value === false && controls['session1Grade'].value !== 2) ||
      controls['session1NoShow'].value == null
    ) {
      this.resetControl(controls['session2NoShow']);
    }

    if (controls['session1NoShow'].value !== false) {
      this.resetControl(controls['session1Grade']);
    }

    if (
      (controls['session2NoShow'].value === false && controls['session2Grade'].value !== 2) ||
      controls['session2NoShow'].value == null
    ) {
      this.resetControl(controls['session3NoShow']);
    }

    if (controls['session2NoShow'].value !== false) {
      this.resetControl(controls['session2Grade']);
    }

    if (controls['session3NoShow'].value !== false) {
      this.resetControl(controls['session3Grade']);
    }
  }
}

function mapSessions(data: GradeResultSessionsEditComponent['data']) {
  return data.sessions.map((s) => ({
    ...s,
    abnormalStatus: s.isRemoved ? 'ИЗТРИТ' : s.isTransferred ? 'ОТПИСАН' : ''
  }));
}
