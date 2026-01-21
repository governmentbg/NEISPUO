import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormGroupDirective } from '@angular/forms';
import { NavigationExtras } from '@angular/router';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SaveToken } from '../editor-panel.component';

@Component({
  selector: 'sb-new-panel',
  templateUrl: './new-panel.component.html'
})
export class NewPanelComponent implements OnInit, OnDestroy {
  protected readonly destroyed$ = new Subject<void>();
  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasArrowLeft = fasArrowLeft;

  @Input() backBtnHidden = false;
  @Input() backBtnDisabled = false;
  @Input() backBtnRouteCommands: any[] = ['../'];
  @Input() backBtnRouteExtras: NavigationExtras | null = null;

  @Output() save = new EventEmitter<SaveToken>();

  saveBtnDisabled = false;

  constructor(private formGroupDirective: FormGroupDirective) {}

  ngOnInit() {
    this.formGroupDirective.ngSubmit.pipe(takeUntil(this.destroyed$)).subscribe(() => {
      if (this.formGroupDirective.invalid || this.saveBtnDisabled) {
        return;
      }

      this.saveBtnDisabled = true;
      this.save.emit({
        done: (success: boolean) => {
          this.saveBtnDisabled = false;
        }
      });
    });
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
