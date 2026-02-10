import { Injectable, Injector, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { startWith, takeUntil } from 'rxjs/operators';
import { ControlValueAccessorConnector } from './control-value-accessor-connector';

@Injectable()
export class BaseField extends ControlValueAccessorConnector implements OnInit, OnDestroy {
  protected readonly destroyed$ = new Subject<void>();

  label!: string;

  placeholder = '';

  validations?: {
    [key: string]: string;
  };

  errorMessage?: string;
  hint?: string;

  readonly = false;

  constructor(injector: Injector) {
    super(injector);
  }

  // eslint-disable-next-line @angular-eslint/contextual-lifecycle
  ngOnInit() {
    if (this.label === undefined) {
      throw new Error('Required Input label is null or undefined.');
    }

    super.ngOnInit();

    this.control.valueChanges.pipe(startWith(null), takeUntil(this.destroyed$)).subscribe(() => {
      this.errorMessage = undefined;

      if (this.control.hasError('required')) {
        this.errorMessage = 'Задължително поле';
      } else if (this.validations) {
        for (const [errorKey, errorMessage] of Object.entries(this.validations)) {
          if (this.control.hasError(errorKey)) {
            this.errorMessage = errorMessage;
            break;
          }
        }
      }
    });
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
