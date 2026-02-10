import { Directive, Output, EventEmitter, HostListener, ElementRef, Input } from '@angular/core';
import { ConfirmationService } from 'primeng/api';

@Directive({
  selector: '[appActionConfirmation]'
})
export class ActionConfirmationDirective {
  @Output() onAcceptConfirmation = new EventEmitter<any>();
  @Output() onRejectConfirmation = new EventEmitter<any>();
  @Input() disableConfirmation: boolean = false;

  /**
   * for custom confirmation, add <p-confirmDialog [key]="'customKey'">
   * and use [appActionConfirmation] [confirmationOutletKey] = 'customKey'
   */
  @Input() confirmationOutletKey = '';

  @HostListener('click', ['$event']) onClick($event) {
    if (this.confirmationOutletKey) {
      return this.confirmationService.confirm({
        key: this.confirmationOutletKey,
        accept: () => {
          this.onAcceptConfirmation.emit();
        },
        reject: () => {
          this.onRejectConfirmation.emit();
        }
      });
    } else {
      return {};
    }
  }

  constructor(private confirmationService: ConfirmationService) {}
}
