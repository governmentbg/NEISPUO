import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { NeispuoModuleQuery } from '@portal/neispuo-modules/neispuo-module.query';
import { NeispuoUserGuide } from '@portal/neispuo-modules/neispuo-user-guide.interface';
import { UserGuideManagementService } from '@portal/services/user-guide-management.service';
import { ToastService } from '@shared/services/toast.service';
import { SelectItem } from 'primeng/api';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { SubSink } from 'subsink';
import { finalize } from 'rxjs/operators';
import { NeispuoModuleService } from '@portal/neispuo-modules/neispuo-module.service';
import { NeispuoCategory } from '@portal/neispuo-modules/neispuo-category.interface';

@Component({
  selector: 'app-user-guide-addition-modal',
  templateUrl: './user-guide-addition-modal.component.html',
  styleUrls: ['./user-guide-addition-modal.component.scss']
})
export class UserGuideAdditionModalComponent implements OnInit {
  saveUserGuideForm!: FormGroup;
  selectableCategories: SelectItem[];
  categories: NeispuoCategory[];
  subs = new SubSink();
  userGuide: NeispuoUserGuide;
  uploadedFile: File;
  isFileChanged: boolean = false;
  loading: boolean = false;

  constructor(
    public userGuideManagementService: UserGuideManagementService,
    private formBuilder: FormBuilder,
    private toastService: ToastService,
    public neispuoModuleQuery: NeispuoModuleQuery,
    private nmService: NeispuoModuleService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {}

  ngOnInit(): void {
    this.loadCategoriesAndUserGuides();
    this.userGuide = this.config.data.userGuide;

    if (this.userGuide) {
      this.initForm(this.userGuide);
      this.getExistingUserGuideData();
    } else {
      this.initForm();
    }
  }

  private linkOrFileValidator(): (AbstractControl) => ValidationErrors | null {
    return (formGroup: AbstractControl): ValidationErrors | null => {
      const urlOverride = formGroup.get('URLOverride')?.value;
      const filename = formGroup.get('filename')?.value;

      const hasUrl = urlOverride && urlOverride.trim() !== '';
      const hasFile = filename && filename.trim() !== '';

      if (!hasUrl && !hasFile) {
        return { linkOrFileRequired: true };
      }

      return null;
    };
  }

  clearFile() {
    this.uploadedFile = null;
    this.isFileChanged = true;
    this.saveUserGuideForm.get('file')?.setValue('');
    this.saveUserGuideForm.get('filename')?.setValue('');
    this.saveUserGuideForm.updateValueAndValidity();
  }

  private initForm(userGuide?: any) {
    this.saveUserGuideForm = this.formBuilder.group(
      {
        name: [userGuide?.name, [Validators.required, Validators.minLength(2)]],
        category: [userGuide?.category?.id, Validators.required],
        file: [userGuide?.file || ''],
        filename: [userGuide?.filename || ''],
        URLOverride: [userGuide?.URLOverride || '']
      },
      { validators: this.linkOrFileValidator() }
    );
  }

  onUpload($event: any, fileUpload: any) {
    this.isFileChanged = true;
    this.uploadedFile = $event.files[0];
    fileUpload.clear();

    this.saveUserGuideForm.get('file')?.setValue(this.uploadedFile);
    this.saveUserGuideForm.get('filename')?.setValue(this.uploadedFile.name);

    this.saveUserGuideForm.updateValueAndValidity();
  }

  saveUserGuide() {
    if (this.uploadedFile) {
      this.saveUserGuideForm.get('file').setValue(this.uploadedFile);
      this.saveUserGuideForm.get('filename').setValue(this.uploadedFile.name);
    }

    if (this.saveUserGuideForm.valid) {
      this.loading = true;
      let formData = new FormData();
      formData.append('name', this.saveUserGuideForm.get('name').value);
      formData.append('category', this.saveUserGuideForm.get('category').value.toString());
      formData.append('URLOverride', this.saveUserGuideForm.get('URLOverride').value);

      if (this.uploadedFile && this.uploadedFile.size > 0) {
        formData.append('file', this.uploadedFile);
        formData.append('filename', this.uploadedFile.name);
      } else if (this.isFileChanged) {
        // file was cleared
        formData.append('filename', '__REMOVE_FILE__');
      }

      if (this.userGuide) {
        this.updateUserGuide(formData);
      } else {
        this.addUserGuide(formData);
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched() {
    Object.keys(this.saveUserGuideForm.controls).forEach((key) => {
      const control = this.saveUserGuideForm.get(key);
      control?.markAsTouched();
    });
  }

  getNameErrorMessage(): string {
    const nameControl = this.saveUserGuideForm.get('name');
    if (nameControl?.errors && nameControl.touched) {
      if (nameControl.errors['required']) {
        return 'Името на ръководството е задължително.';
      }
      if (nameControl.errors['minlength']) {
        return 'Името трябва да бъде поне 2 символа.';
      }
    }
    return '';
  }

  getCategoryErrorMessage(): string {
    const categoryControl = this.saveUserGuideForm.get('category');
    if (categoryControl?.errors && categoryControl.touched) {
      if (categoryControl.errors['required']) {
        return 'Категорията е задължителна.';
      }
    }
    return '';
  }

  getLinkOrFileErrorMessage(): string {
    if (
      this.saveUserGuideForm.errors &&
      this.saveUserGuideForm.errors['linkOrFileRequired'] &&
      (this.saveUserGuideForm.get('URLOverride')?.touched || this.saveUserGuideForm.get('filename')?.touched)
    ) {
      return 'Трябва да предоставите линк или прикачен файл.';
    }
    return '';
  }

  loadSelectableCategories() {
    this.selectableCategories = this.categories.map((category) => ({
      label: category.name,
      value: category.id
    }));
  }

  getExistingUserGuideData() {
    // we already have the filename, so we can just create a placeholder file
    // the actual file content will only be downloaded if the user wants to download it
    if (this.userGuide.filename) {
      this.uploadedFile = new File([], this.userGuide.filename);
      this.saveUserGuideForm.get('file').setValue(this.uploadedFile);
      this.saveUserGuideForm.get('filename').setValue(this.userGuide.filename);

      this.saveUserGuideForm.updateValueAndValidity();
    }
  }

  updateUserGuide(formData: FormData) {
    this.userGuideManagementService
      .updateUserGuideByID(this.userGuide.id, formData)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: () => {
          this.toastService.initiate({
            content: 'Успешнo редактиране на ръководство.',
            style: 'success',
            sticky: false,
            position: 'bottom-right'
          });
          this.ref.close(true);
        },
        error: () => {
          this.toastService.initiate({
            content: 'Грешка при редактиране на ръководство.',
            style: 'error',
            sticky: false,
            position: 'bottom-right'
          });
          this.ref.close();
        }
      });
  }

  addUserGuide(formData: FormData) {
    this.userGuideManagementService
      .addUserGuide(formData)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: () => {
          this.toastService.initiate({
            content: 'Успешнo добавяне на ръководство.',
            style: 'success',
            sticky: false,
            position: 'bottom-right'
          });
          this.ref.close(true);
        },
        error: () => {
          this.toastService.initiate({
            content: 'Грешка при добавяне на ръководство.',
            style: 'error',
            sticky: false,
            position: 'bottom-right'
          });
          this.ref.close();
        }
      });
  }

  loadCategoriesAndUserGuides() {
    this.nmService.getCategories().subscribe((res) => {
      this.categories = res;
      this.userGuideManagementService.loadUserGuides(res);
      this.loadSelectableCategories();
    });
  }
}
