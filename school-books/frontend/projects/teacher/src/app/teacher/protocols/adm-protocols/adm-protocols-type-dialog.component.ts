import { Component, Inject, Input } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { AdmProtocolTypeNomsService } from 'projects/sb-api-client/src/api/admProtocolTypeNoms.service';
import { AdmProtocolType } from 'projects/sb-api-client/src/model/admProtocolType';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_DIALOG_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';

export type AdmProtocolsTypeDialogComponentData = {
  schoolYear: number;
  instId: number;
};

@Component({
  template: SIMPLE_DIALOG_SKELETON_TEMPLATE
})
export class AdmProtocolsTypeDialogSkeletonComponent extends SkeletonComponentBase {
  d!: AdmProtocolsTypeDialogComponentData;
  r!: AdmProtocolType;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: AdmProtocolsTypeDialogComponentData
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;

    this.resolve(AdmProtocolsTypeDialogComponent, { schoolYear, instId });
  }
}

@Component({
  selector: 'sb-adm-protocols-type-dialog',
  templateUrl: './adm-protocols-type-dialog.component.html'
})
export class AdmProtocolsTypeDialogComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
  };

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    admProtocolType: [null, Validators.required]
  });

  admProtocolTypeNomsService: INomService<AdmProtocolType, { instId: number; schoolYear: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    admProtocolTypeNomsService: AdmProtocolTypeNomsService,
    private dialogRef: MatDialogRef<AdmProtocolsTypeDialogComponent>
  ) {
    this.admProtocolTypeNomsService = new NomServiceWithParams(admProtocolTypeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));
  }

  onSave() {
    if (this.form.invalid) {
      return;
    }

    const value = this.form.value;
    this.dialogRef.close(<AdmProtocolType>value.admProtocolType);
  }
}
