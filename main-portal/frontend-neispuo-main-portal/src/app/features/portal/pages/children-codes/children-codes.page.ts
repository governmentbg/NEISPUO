import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { ParentChildrenService } from '@portal/services/parent-children.service';
import { IPerson } from '@shared/modals/person.modal';
import { ToastService } from '@shared/services/toast.service';

@Component({
  selector: 'app-children-codes.page',
  templateUrl: './children-codes.page.html',
  styleUrls: ['./children-codes.page.scss']
})
export class ChildrenCodesPage implements OnInit {
  // get codes
  childrenCodesForm: FormGroup;
  children: IPerson[] = [];
  parentID: number;
  toastText: string = "";
  submitForm: boolean = false;

  constructor(
    private fb: FormBuilder,
    private parentChildrenService: ParentChildrenService,
    private authQuery: AuthQuery,
    private toast: ToastService
  ) {
    this.authQuery.select('selected_role').subscribe((r) => (this.parentID = r.PersonID));
  }

  ngOnInit(): void {
    this.init();
  }

  init() {
    this.initForm();
    this.getParentChildren();
  }

  initForm() {
    this.childrenCodesForm = this.fb.group({
      schoolBookCode: ['', [Validators.required]],
      // maybe add UCN validation
      personalID: ['', [Validators.required, Validators.pattern('[0-9]+')]]
    });
  }

  getParentChildren() {
    this.parentChildrenService.getParentChildren().subscribe((children) => {
      this.children = children;
    });
  }

  onDelistChild(child: IPerson) {
    this.parentChildrenService.delistChild(child, this.parentID).subscribe(
      (resp) => {
        this.toast.initiate({
          content: 'Успешно премахване на код.',
          style: 'success',
          sticky: true
        });
        this.init();
      },
      (err) => {
        this.toast.initiate({
          content: 'Неуспешно премахване.',
          style: 'error',
          sticky: true
        });
      }
    );
  }

  onSubmit(formDirective: FormGroupDirective) {
    this.submitForm = true;
    if (this.childrenCodesForm.valid)
      this.parentChildrenService.enrollChild(this.childrenCodesForm.value, this.parentID).subscribe(
        (resp) => {
          this.toast.initiate({
            content: 'Успешно добавяне на код.',
            style: 'success',
            sticky: true
          });
          this.childrenCodesForm.reset();
          formDirective.resetForm();
          this.init();
        },
        (err) => {
          if (err.status === 404) {
            this.toast.initiate({
              content: 'Невалидни данни за ученик.',
              style: 'error',
              sticky: true
            });
          }

          else {
            this.toast.initiate({
              content: 'Неуспешно добавяне. Моля опитайте отново.',
              style: 'error',
              sticky: true
            });
          }
        }
      );
  }
}
