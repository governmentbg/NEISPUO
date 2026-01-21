import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  SkillsCheckExamResultProtocolsService,
  SkillsCheckExamResultProtocols_GetEvaluator
} from 'projects/sb-api-client/src/api/skillsCheckExamResultProtocols.service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';

export type SkillsCheckExamResultProtocolViewDialogData = {
  schoolYear: number;
  instId: number;
  skillsCheckExamResultProtocolId: number;
  skillsCheckExamResultProtocolEvaluatorId: number | null;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class SkillsCheckExamResultProtocolViewDialogSkeletonComponent extends SkeletonComponentBase {
  d!: SkillsCheckExamResultProtocolViewDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: SkillsCheckExamResultProtocolViewDialogData,
    skillsCheckExamResultProtocolsService: SkillsCheckExamResultProtocolsService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const skillsCheckExamResultProtocolId = data.skillsCheckExamResultProtocolId;
    const skillsCheckExamResultProtocolEvaluatorId = data.skillsCheckExamResultProtocolEvaluatorId;

    if (skillsCheckExamResultProtocolEvaluatorId) {
      this.resolve(SkillsCheckExamResultProtocolViewDialogComponent, {
        schoolYear,
        instId,
        skillsCheckExamResultProtocolId,
        skillsCheckExamResultProtocolEvaluatorId,
        evaluator: skillsCheckExamResultProtocolsService.getEvaluator({
          schoolYear,
          instId,
          skillsCheckExamResultProtocolId,
          skillsCheckExamResultProtocolEvaluatorId
        })
      });
    } else {
      this.resolve(SkillsCheckExamResultProtocolViewDialogComponent, {
        schoolYear,
        instId,
        skillsCheckExamResultProtocolId,
        skillsCheckExamResultProtocolEvaluatorId,
        evaluator: null
      });
    }
  }
}

@Component({
  selector: 'sb-skills-check-exam-result-protocol-evaluator-view-dialog',
  templateUrl: './skills-check-exam-result-protocol-evaluator-view-dialog.component.html'
})
export class SkillsCheckExamResultProtocolViewDialogComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    skillsCheckExamResultProtocolId: number;
    skillsCheckExamResultProtocolEvaluatorId: number | null;
    evaluator: SkillsCheckExamResultProtocols_GetEvaluator | null;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    name: [null, Validators.required]
  });

  saving = false;

  constructor(
    private fb: UntypedFormBuilder,
    private skillsCheckExamResultProtocolsService: SkillsCheckExamResultProtocolsService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<SkillsCheckExamResultProtocolViewDialogComponent>
  ) {}

  ngOnInit(): void {
    const evaluator = this.data.evaluator;
    if (evaluator) {
      this.form.setValue({
        name: evaluator.name
      });
    }
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }
    this.saving = true;

    const name = <string>this.form.value.name;

    const evaluator = {
      name
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.evaluator == null) {
            return this.skillsCheckExamResultProtocolsService
              .createEvaluator({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                skillsCheckExamResultProtocolId: this.data.skillsCheckExamResultProtocolId,
                createSkillsCheckExamResultProtocolEvaluatorCommand: evaluator
              })
              .toPromise()
              .then(() => {
                this.dialogRef.close(true);
              });
          } else {
            if (!this.data.skillsCheckExamResultProtocolEvaluatorId) {
              throw new Error('onUpdate requires an evaluator to have been loaded.');
            }

            return this.skillsCheckExamResultProtocolsService
              .updateEvaluator({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                skillsCheckExamResultProtocolId: this.data.skillsCheckExamResultProtocolId,
                skillsCheckExamResultProtocolEvaluatorId: this.data.skillsCheckExamResultProtocolEvaluatorId,
                updateSkillsCheckExamResultProtocolEvaluatorCommand: evaluator
              })
              .toPromise()
              .then(() => {
                this.dialogRef.close(true);
              });
          }
        }
      })
      .finally(() => {
        this.saving = false;
      });
  }
}
