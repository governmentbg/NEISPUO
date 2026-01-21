import { Component, Input, Type } from '@angular/core';
import { InputsType } from 'ng-dynamic-component';

const template = `
  <ndc-dynamic *ngIf="ready && !error" [ndcDynamicComponent]="component" [ndcDynamicInputs]="inputs"></ndc-dynamic>
  <div *ngIf="!ready || error" class="relative sm:w-96 h-96">
    <div class="sb-btn-wrapper sb-btn-wrapper-right">
      <button mat-raised-button mat-dialog-close>Затвори</button>
    </div>

    <sb-banner type="error" *ngIf="ready">{{ errorMessage }}</sb-banner>
    <mat-spinner *ngIf="!ready" [diameter]="80" class="absolute top-1/2 left-1/2 -mt-10 -ml-10"></mat-spinner>
  </div>
`;

@Component({
  selector: 'sb-simple-dialog-skeleton-template',
  template
})
export class SimpleDialogSkeletonTemplateComponent {
  @Input() component!: Type<any>;
  @Input() inputs!: InputsType;
  @Input() ready!: boolean;
  @Input() error!: boolean;
  @Input() errorMessage?: string | null;
}
