import { Component, Input, Type } from '@angular/core';
import { InputsType } from 'ng-dynamic-component';

const template = `
  <ndc-dynamic *ngIf="ready && !error" [ndcDynamicComponent]="component" [ndcDynamicInputs]="inputs"></ndc-dynamic>
  <sb-app-chrome *ngIf="ready && error">
    <sb-app-menu left-side [menuItems]="[
      {
        text: errorMessage ?? ''
      }
    ]"></sb-app-menu>
    <router-outlet main-content></router-outlet>
  </sb-app-chrome>
  <sb-app-chrome *ngIf="!ready">
    <sb-app-menu left-side [menuItems]="[
      {
        text: '',
        isSkeleton: true
      },
      {
        text: '',
        isSkeleton: true
      },
      {
        text: '',
        isSkeleton: true
      },
      {
        text: '',
        isSkeleton: true
      }
    ]"></sb-app-menu>
    <div main-content></div>
  </sb-app-chrome>
`;

@Component({
  selector: 'sb-app-chrome-skeleton-template',
  template
})
export class AppChromeSkeletonTemplateComponent {
  @Input() component!: Type<any>;
  @Input() inputs!: InputsType;
  @Input() ready!: boolean;
  @Input() error!: boolean;
  @Input() errorMessage?: string | null;
}
