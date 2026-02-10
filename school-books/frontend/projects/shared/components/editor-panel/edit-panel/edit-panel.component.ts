import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormGroupDirective } from '@angular/forms';
import { ActivatedRoute, NavigationExtras, Router } from '@angular/router';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTimes as fasTimes } from '@fortawesome/pro-solid-svg-icons/faTimes';
import { reloadRoute } from 'projects/shared/utils/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SaveToken } from '../editor-panel.component';

@Component({
  selector: 'sb-edit-panel',
  templateUrl: './edit-panel.component.html'
})
export class EditPanelComponent implements OnChanges, OnInit, OnDestroy {
  protected readonly destroyed$ = new Subject<void>();
  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasTimes = fasTimes;
  readonly fasPencil = fasPencil;
  readonly fasArrowLeft = fasArrowLeft;

  @Input() editBtnHidden = false;
  @Input() editBtnDisabled = false;

  @Input() backBtnHidden = false;
  @Input() backBtnDisabled = false;
  @Input() backBtnRouteCommands: any[] = ['../'];
  @Input() backBtnRouteExtras: NavigationExtras | null = null;

  @Input() editable = false;
  @Output() editableChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<SaveToken>();

  saveBtnDisabled = false;

  constructor(private formGroupDirective: FormGroupDirective, private route: ActivatedRoute, private router: Router) {}

  ngOnChanges(changes: SimpleChanges): void {
    const editableChange = changes['editable'];

    if (!editableChange || editableChange.isFirstChange()) {
      // ngOnInit takes care of the initial
      // as it will account for the default as well
      return;
    }

    if (changes['editable'].currentValue) {
      this.formGroupDirective.control.enable({ emitEvent: false });
    } else {
      this.formGroupDirective.control.disable({ emitEvent: false });
    }
  }

  ngOnInit() {
    if (!this.editable) {
      setTimeout(() => {
        this.formGroupDirective.control.disable({ emitEvent: false });
      });
    }

    this.formGroupDirective.ngSubmit.pipe(takeUntil(this.destroyed$)).subscribe(() => {
      if (this.formGroupDirective.invalid || this.saveBtnDisabled || !this.editable) {
        return;
      }

      this.saveBtnDisabled = true;
      this.save.emit({
        done: (success: boolean) => {
          this.saveBtnDisabled = false;

          if (success) {
            this.formGroupDirective.control.markAsPristine();
            reloadRoute(this.router, this.route);
          }
        }
      });
    });
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  cancel() {
    reloadRoute(this.router, this.route);
  }

  edit() {
    this.formGroupDirective.control.enable({ emitEvent: false });
    this.setEditable(true);
  }

  private setEditable(editable: boolean) {
    this.editable = editable;
    this.editableChange.emit(editable);
  }
}
