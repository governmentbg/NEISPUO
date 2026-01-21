import { Component, ComponentRef, EventEmitter, HostListener, Input, Output, Type } from '@angular/core';
import { InputsType } from 'ng-dynamic-component';

const template = `
  <ndc-dynamic *ngIf="ready && !error" [ndcDynamicComponent]="component" [ndcDynamicInputs]="inputs" (ndcDynamicCreated)="onComponentCreated($event)"></ndc-dynamic>
  <div *ngIf="!ready || error" class="relative h-96">
    <sb-banner type="error" *ngIf="ready">{{ errorMessage }}</sb-banner>
    <mat-spinner *ngIf="!ready" [diameter]="80" class="absolute top-1/2 left-1/2 -mt-10 -ml-10"></mat-spinner>
  </div>
`;

@Component({
  selector: 'sb-simple-tab-skeleton-template',
  template
})
export class SimpleTabSkeletonTemplateComponent {
  @Input() component!: Type<any>;
  @Input() inputs!: InputsType;
  @Input() ready!: boolean;
  @Input() error!: boolean;
  @Input() errorMessage?: string | null;
  @Output() componentCreated = new EventEmitter<ComponentRef<any>>();
  private componentRef: ComponentRef<any> | undefined;

  //TODO: Update beforeunload event to be attached only when it's required to improve performance, when angular version is updated
  @HostListener('window:beforeunload', ['$event'])
  beforeUnloadHandler(event: BeforeUnloadEvent) {
    if (this.componentRef?.instance != null && typeof this.componentRef.instance.shouldPreventLeave === 'function') {
      const preventLeaveResponse = this.componentRef.instance.shouldPreventLeave();
      if (
        typeof preventLeaveResponse === 'string' ||
        (typeof preventLeaveResponse === 'boolean' && preventLeaveResponse)
      ) {
        event.returnValue = '';
      }
    }
  }

  onComponentCreated(compRef: ComponentRef<any>) {
    this.componentRef = compRef;
    this.componentCreated.emit(compRef);
  }
}
