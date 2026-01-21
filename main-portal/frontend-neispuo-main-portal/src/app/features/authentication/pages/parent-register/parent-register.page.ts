import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '@shared/services/api.service';
import { ToastService } from '@shared/services/toast.service';

function checkIfMatchingPasswords(passwordKey: string, passwordConfirmationKey: string) {
  return (group: FormGroup) => {
    let passwordInput = group.controls[passwordKey],
      passwordConfirmationInput = group.controls[passwordConfirmationKey];
    if (passwordInput.value !== passwordConfirmationInput.value) {
      return passwordConfirmationInput.setErrors({ notEquivalent: true });
    } else {
      return passwordConfirmationInput.setErrors(null);
    }
  };
}

function checkIfEmailDomainIsValid(control: FormControl) {
  let email = control.value;
  if (email && email.indexOf("@") !== -1) {
    let [_, domain] = email.split("@");
    if (domain === "edu.mon.bg" || domain === "mon.bg") {
      return {
        emailDomain: {
          parsedDomain: domain
        }
      }
    }
  }
  return null;
}

@Component({
  selector: 'app-parent-register',
  templateUrl: './parent-register.page.html',
  styleUrls: ['./parent-register.page.scss']
})
export class ParentRegisterPage implements OnInit {
  toastText: string = '';
  submitForm: boolean;
  parentRegisterForm: FormGroup;
  constructor(private fb: FormBuilder, private apiService: ApiService, private toast: ToastService) { }

  ngOnInit() {
    this.initForm();
  }

  private initForm() {
    const lettersPattern = '^[a-zA-Zа-яА-Я- ]+$';
    this.parentRegisterForm = this.fb.group(
      {
        firstName: ['', [Validators.required, Validators.pattern(lettersPattern)]],
        middleName: ['', Validators.pattern(lettersPattern)],
        lastName: ['', [Validators.required, Validators.pattern(lettersPattern)]],
        email: ['', [Validators.required, Validators.email, checkIfEmailDomainIsValid]],
        password: ['', [Validators.required]],
        confirmPassword: ['', [Validators.required]],
        childrenCodes: this.fb.array([])
      },
      { validator: checkIfMatchingPasswords('password', 'confirmPassword') }
    );

    this.parentRegisterForm.get('middleName').valueChanges.subscribe((value) => {
      // Update the validation based on whether the field is filled or not
      const validators = value ? [Validators.pattern(lettersPattern)] : [];
      this.parentRegisterForm.get('middleName').setValidators(validators);
      this.parentRegisterForm.get('middleName').updateValueAndValidity();
    });
  }

  createChildCodeFormGroup() {
    return this.fb.group({
      schoolBookCode: ['', [Validators.required]],
      // maybe add UCN validation
      personalID: ['', [Validators.required, Validators.pattern('[0-9]+')]]
    });
  }

  onAddChildCode() {
    (this.parentRegisterForm.get('childrenCodes') as FormArray).push(this.createChildCodeFormGroup());
  }

  onRemoveChildCode(index: number) {
    (this.parentRegisterForm.get('childrenCodes') as FormArray).removeAt(index);
  }

  onSubmit() {
    this.submitForm = true;
    if (!this.parentRegisterForm.valid) {
      this.submitForm = false;
      return;
    }
    this.apiService.post(`/v1/parent-register`, { ...this.parentRegisterForm.value, confirmPassword: undefined }).subscribe(
      (resp) => {
        this.toast.initiate({
          content:
            'Успешна регистрация. Моля, изчакайте до 10 минути, докато акаунта ви се синхронизира с активната директория.',
          style: 'success',
          sticky: false
        });
        this.parentRegisterForm.reset();
        this.submitForm = false;
      },
      (error) => {
        console.error(error);
        if (error.statusCode === 400) {
          this.toast.initiate({
            content: 'Невалидни данни за родител или деца.',
            style: 'error',
            sticky: false
          });
        } else if (error.statusCode === 409) {
          this.toast.initiate({
            content: 'Използваният от вас имейл вече е зает.',
            style: 'error',
            sticky: false
          });
        } else if (error.statusCode === 404) {
          this.toast.initiate({
            content: 'Невалидни код на дете или ЕГН.',
            style: 'error',
            sticky: false
          });
        } else {
          this.toast.initiate({
            content: 'Неуспешна регистрация. Моля опитайте по-късно.',
            style: 'error',
            sticky: false
          });
        }
        this.submitForm = false;
      }
    );
  }
}
